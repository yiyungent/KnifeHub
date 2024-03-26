using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PluginCore.Interfaces;
using PluginCore.IPlugins;
using System.Threading.Tasks;

namespace KnifeHub.Web.Controllers
{
    [Route("")]
    //[Authorize("PluginCore.Admin")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            string indexFilePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html");

            return PhysicalFile(indexFilePath, "text/html");
        }


    }

}
