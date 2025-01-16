using OCCU_Project.Models;

namespace OCCU_Project.DataProviders
{
    public interface IStatusDataProvider
    {
        StatusList GetStatusList();
    }
}
