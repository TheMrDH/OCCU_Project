using Json.Patch;
using OCCU_Project.Helpers;
using OCCU_Project.Models;

namespace OCCU_Project.DataProviders
{
    public interface IDataContentDataProvider
    {
        Task<DataContentList> GetDataContent(int page = 0, string name = "", string field1 = "", string field2 = "", string field3 = "");
        DataContentList UpdateDataContent(HashedJsonPatch objectPatch, string name = "");
        DataContentList CreateDataContent(DataContentObject dataContentObject);
        void DeleteDataContent(string name);
    }
}
