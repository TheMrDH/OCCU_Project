using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OCCU_Project.Models
{
    [Table("DataContent")]
    public class DataContentObject
    {
        [Key]
        public string Name { get; set; } = "";
        
        public string Field1 { get; set; } = "";

        public string Field2 { get; set; } = "";

        public string Field3 { get; set; } = "";

        public string LastUpdateDate { get; set; } = DateTime.Now.ToString();

        [NotMapped]
        public string RecordHash { get; set; } = "";
            
    }

    public class DataContentList
    {
        public List<DataContentObject> DataContent { get; set; } = new List<DataContentObject>();
        public int Pages { get; set; } = 0;
    }
}
