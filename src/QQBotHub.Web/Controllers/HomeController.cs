using Konata.Core.Common;
using Konata.Core.Interfaces;
using Konata.Core.Interfaces.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PluginCore.Interfaces;
using PluginCore.IPlugins;
using System.Threading.Tasks;
using static Konata.Core.Events.Model.CaptchaEvent;

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
            return Content("正常运行中, 请前往 <a href='/PluginCore/Admin'>插件管理</a>");
        }
    }

}
