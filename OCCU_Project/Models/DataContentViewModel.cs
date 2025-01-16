using OCCU_Project.Helpers;

namespace OCCU_Project.Models
{
    public class DataContentViewModel
    {
        public DataContentList DataContentList { get; set; } = new DataContentList();
        public HashedJsonPatch HashedJsonPatch { get; set; } = new HashedJsonPatch();
    }
}
