using Microsoft.AspNetCore.Mvc;
using OCCU_Project.DataProviders;
using OCCU_Project.Models;

namespace OCCU_Project.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StatusController : Controller
    {
        private IStatusDataProvider _statusDataProvider;

        public StatusController(IStatusDataProvider statusDataProvider) { 
        _statusDataProvider = statusDataProvider;
        }

        
        [HttpGet("getlist")]
        public ActionResult GetList()
        {
           //var statusObject = _statusDataProvider.GetStatusList();
            return View(_statusDataProvider.GetStatusList()); //Json( );
        }
    }
}
