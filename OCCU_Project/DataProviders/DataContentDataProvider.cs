using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Json.Patch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OCCU_Project.Data;
using OCCU_Project.Helpers;
using OCCU_Project.Models;

namespace OCCU_Project.DataProviders
{
    [NonViewComponent]
    public class DataContentDataProvider: IDataContentDataProvider
    {
        DataContext _context;
        private const int numResults = 20;
        public DataContentDataProvider(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets data from datacontent table.
        /// <para>Utilizing LINQ and dynamically built expressions to help keep structure clean and safer.</para>
        /// </summary>
        public async Task<DataContentList> GetDataContent(int page = 0, string name = "", string field1 = "", string field2 = "", string field3 = "")
        {
            DataContentList dataContentList = new DataContentList();
            int offset = page > 1 ? ((page - 1) * numResults) : 0; //Calculate offset with hardcoded length from numResults.

            if (!string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(field1) || !string.IsNullOrEmpty(field2) || !string.IsNullOrEmpty(field3))
            {
                //Get all string properties for desired model
                PropertyInfo[] propertyInfos = typeof(DataContentObject).GetProperties().Where(m => m.PropertyType == typeof(string)).ToArray();

                //Create param object for access later
                ParameterExpression parameterExpression = Expression.Parameter(typeof(DataContentObject));

                //Get reference to needed string methods
                MethodInfo containsMethod = typeof(string).GetMethod("Contains", [typeof(string)])!;
                MethodInfo tolowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes)!;


                //Builds our LINQ expressions so we are make our SQL purely through code.
                //Next step would be to dynamically get the method parameters and values to prevent duplication below.
                List<Expression> expressions = new List<Expression>();
                if (!string.IsNullOrEmpty(name))
                    expressions.Add(Expression.Call(Expression.Call(Expression.Property(parameterExpression, propertyInfos.First(o => o.Name.ToLower() == "name")), tolowerMethod),
                    containsMethod,
                    Expression.Constant(name.ToLower())));

                if (!string.IsNullOrEmpty(field1))
                    expressions.Add(Expression.Call(Expression.Call(Expression.Property(parameterExpression, propertyInfos.First(o => o.Name.ToLower() == "field1")), tolowerMethod),
                    containsMethod,
                    Expression.Constant(field1.ToLower())));

                if (!string.IsNullOrEmpty(field2.ToLower()))
                    expressions.Add(Expression.Call(Expression.Call(Expression.Property(parameterExpression, propertyInfos.First(o => o.Name.ToLower() == "field2")), tolowerMethod),
                    containsMethod,
                    Expression.Constant(field2)));

                if (!string.IsNullOrEmpty(field3))
                    expressions.Add(Expression.Call(Expression.Call(Expression.Property(parameterExpression, propertyInfos.First(o => o.Name.ToLower() == "field3")), tolowerMethod),
                    containsMethod,
                    Expression.Constant(field3.ToLower())));

                //Builds "OR" list.
                Expression primaryExpression = expressions.Aggregate((prev, current) => Expression.Or(prev, current));

                //Assemble entire expression.
                Expression<Func<DataContentObject, bool>> lambda = Expression.Lambda<Func<DataContentObject, bool>>(primaryExpression, parameterExpression);

                //Also returns page count for pagination control.
                //Doing it like so allow us to only need one pull from the db.
                var results = await _context.DataContentObjects.Where(lambda).Select(p => new
                {
                    Data = p,
                    PageCount = _context.DataContentObjects.Where(lambda).Count()
                }).Skip(offset).Take(numResults).ToArrayAsync();
               
                if (results.Any())
                {
                    //Round up to ensure the correct number of page buttons exist.
                    dataContentList.Pages = (int)Math.Ceiling((decimal)results.FirstOrDefault().PageCount / (decimal)numResults);
                    dataContentList.DataContent = results.Select(p => p.Data).ToList();
                }

            }
            else
            {

                //Also returns page count for pagination control.
                //Doing it like so allow us to only need one pull from the db.
                var results = await _context.DataContentObjects.Select(p => new
                {
                    Data = p,
                    PageCount = _context.DataContentObjects.Count()
                }).Skip(offset).Take(numResults).ToArrayAsync();

                if (results.Any())
                {
                    //Round up to ensure the correct number of page buttons exist.
                    dataContentList.Pages = (int)Math.Ceiling((decimal)results.FirstOrDefault().PageCount / (decimal)numResults);
                    dataContentList.DataContent = results.Select(p => p.Data).ToList();
                }
                //Build hash to determine if the records are the same);
            }
            dataContentList.DataContent.ForEach(o => o.RecordHash = GetStringHash(o.Name + o.Field1 + o.Field2 + o.Field3 + o.LastUpdateDate));
            _context.Dispose(); //Be sure to clean up connection else they can build up.
                                //Old habit from working with OpenEdge databases.
            return dataContentList;
        }

        public DataContentList CreateDataContent(DataContentObject dataContentObject)
        {

            if(_context.DataContentObjects.Find(dataContentObject.Name) == null)
            {
                dataContentObject.LastUpdateDate = DateTime.Now.ToString();
                _context.DataContentObjects.Add(dataContentObject);
                _context.SaveChanges();
            }
            else
            {
                throw new Exception("Duplicate record");
            }

            _context.Dispose();
            DataContentList created = new DataContentList() { DataContent = new List<DataContentObject> { dataContentObject } };
            return created;

        }

        public DataContentList UpdateDataContent(HashedJsonPatch objectPatch, string name = "")
        {

            DataContentObject? dataContentObjectTarget = _context.DataContentObjects.Find(name);
            if (dataContentObjectTarget == null)
            {
                throw new Exception("Record not found");

            }

            //Check for stale record
            dataContentObjectTarget.RecordHash = GetStringHash(dataContentObjectTarget.Name + dataContentObjectTarget.Field1 + dataContentObjectTarget.Field2 + dataContentObjectTarget.Field3 + dataContentObjectTarget.LastUpdateDate);
            if (!objectPatch.RecordHash.Equals(dataContentObjectTarget.RecordHash))
            {
                throw new Exception("Stale record");
            }
            else
            {
                _context.Entry(dataContentObjectTarget).State = EntityState.Detached;
                dataContentObjectTarget = objectPatch.Operations.Apply(dataContentObjectTarget, JsonSerializerOptions.Web);
                _context.Entry(dataContentObjectTarget).State = EntityState.Modified;
                dataContentObjectTarget.LastUpdateDate = DateTime.Now.ToString();
                _context.DataContentObjects.Update(dataContentObjectTarget);
                _context.SaveChanges();
            }

            _context.Dispose();
            return new DataContentList() { DataContent = new List<DataContentObject> { dataContentObjectTarget } };

        }

        public void DeleteDataContent(string name)
        {

            DataContentObject? dataContentObject = _context.DataContentObjects.Find(name);
            if ( dataContentObject == null)
            {
                throw new Exception("Record not found");
                
            }
            else
            {
                _context.DataContentObjects.Remove(dataContentObject);
                _context.SaveChanges();
            }

            _context.Dispose();
            return;

        }

        private string GetStringHash(string input)
        {
            //Don't need complex check
            using (MD5 hashAlg = MD5.Create())
            {
                byte[] bytes = hashAlg.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sb = new StringBuilder();
                foreach (byte i in bytes)
                {
                    sb.Append(i.ToString("x2"));
                }

                return sb.ToString();
            }
        }
    }
}
