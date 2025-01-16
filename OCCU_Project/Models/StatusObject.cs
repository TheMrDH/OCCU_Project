using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OCCU_Project.Models
{
    [Table("Status")]
    public class StatusObject
    {
        [Key]
        public long Id { get; set; }
        public string Status { get; set; } = "";

    }

    public class StatusList
    {
        public List<StatusObject> Status { get; set; } = new List<StatusObject>();
    }
}
