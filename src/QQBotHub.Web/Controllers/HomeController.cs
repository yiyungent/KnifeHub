using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PluginCore.Interfaces;
using PluginCore.IPlugins;
using System.Threading.Tasks;

namespace QQBotHub.Web.Controllers
{
    [Route("")]
    //[Authorize("PluginCore.Admin")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [Route("")]
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            string indexFilePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html");

            return PhysicalFile(indexFilePath, "text/html");
        }


    }

}
