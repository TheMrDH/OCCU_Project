using Json.Patch;

namespace OCCU_Project.Helpers
{
    public class HashedJsonPatch
    {
        public string RecordHash { get; set; } = "";
        public JsonPatch Operations { get; set; }
    }
}
