using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using OCCU_Project.Models;
using OCCU_Project.Data;

namespace OCCU_Project.DataProviders
{

    public class StatusDataProvider: IStatusDataProvider
    {
        DataContext _context;

        public StatusDataProvider(DataContext context){
            _context = context;
        }

        public StatusList GetStatusList()
        {
            StatusList statusList = new StatusList();
            statusList.Status = _context.StatusObjects.ToList();
            _context.Dispose(); //Be sure to clean up connection else they can build up.
                                //Old habit from working with OpenEdge databases.
            return statusList;
        }

    }

}
