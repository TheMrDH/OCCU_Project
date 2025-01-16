using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using OCCU_Project.DataProviders;
using OCCU_Project.Helpers;
using OCCU_Project.Models;
using Json.Patch;

namespace OCCU_Project.Controllers
{
    [ApiController]
    public class DataContentController : Controller
    {

        [HttpGet("DataContent", Name = "DataContentView")]
        public ActionResult DataContentView() { 

            return View(new DataContentViewModel());
        }

        public IDataContentDataProvider _dataContentDataProvider;

        public DataContentController(IDataContentDataProvider dataContentDataProvider)
        {
            _dataContentDataProvider = dataContentDataProvider;
        }

        [HttpGet("DataContent/GetListSearch")]
        public IActionResult GetList(int page = 0, string name = "", string field1 = "", string field2 = "", string field3 = "")
        {
            return ViewComponent("GetList", new { page, name, field1, field2, field3 });
        }

        [HttpDelete("DataContent/Delete/{name}")]
        public IActionResult Delete(string name = "")
        {
            try
            {
                _dataContentDataProvider.DeleteDataContent(name);
                return new OkResult();
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }

        [HttpPatch("DataContent/Edit/{name}")]
        public IActionResult Edit([FromBody] HashedJsonPatch objectPatch, string name = "")
        {
            try
            {
                _dataContentDataProvider.UpdateDataContent(objectPatch, name);
                return new OkResult();
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }

        [HttpPost("DataContent/Create")]
        public IActionResult Create([FromBody] DataContentObject newObject)
        {
            try
            {
                _dataContentDataProvider.CreateDataContent(newObject);
                return new OkResult();
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }

    }

    [ViewComponent(Name = "GetList")]
     public class GetList: ViewComponent
    {
        public IDataContentDataProvider _dataContentDataProvider;

        public GetList(IDataContentDataProvider dataContentDataProvider)
        {
            _dataContentDataProvider = dataContentDataProvider;
        }

        
        public async Task<IViewComponentResult> InvokeAsync(int page = 0, string name = "", string field1 = "", string field2 = "", string field3 = "")
        {
            try
            {
                var temp = await _dataContentDataProvider.GetDataContent(page, name, field1, field2, field3);
                return View("../GetList", temp);
            } catch (Exception e)
            {
                return View(e.Message);
            }

        }

     }

    [ViewComponent(Name = "Create")]
    public class Create : ViewComponent
    {
        public IDataContentDataProvider _dataContentDataProvider;

        public Create(IDataContentDataProvider dataContentDataProvider)
        {
            _dataContentDataProvider = dataContentDataProvider;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {

            return View("../Create", new DataContentObject());
        }
    }
}
