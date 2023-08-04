using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PluginCore;
using PluginCore.Interfaces;
using TgBotStatPlugin.Utils;
using TgBotStatPlugin.ResponseModels;
using TgBotStatPlugin.RequestModels;
using PluginCore.IPlugins;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TgBotStatPlugin.Controllers
{
    /// <summary>
    /// 其实也可以不写这个, 直接访问 Plugins/TgBotStatPlugin/index.html
    /// 
    /// 下面的方法, 是去掉 index.html
    /// 
    /// 若 wwwroot 下有其它需要访问的文件, 如何 css, js, 而你又不想每次新增 action 指定返回, 则 Route 必须 Plugins/{PluginId},
    /// 这样访问 Plugins/TgBotStatPlugin/css/main.css 就会访问到你插件下的 wwwroot/css/main.css
    /// </summary>
    [Route($"api/Plugins/{nameof(TgBotStatPlugin)}")]
    [Authorize("PluginCore.Admin")]
    public class HomeController : Controller
    {
        #region Fields

        private readonly IPluginFinder _pluginFinder;
        private readonly bool _debug;

        #endregion

        #region Propertities

        #endregion

        #region Ctor
        public HomeController(IPluginFinder pluginFinder)
        {
            _pluginFinder = pluginFinder;
            string debugStr = EnvUtil.GetEnv("DEBUG");
            if (!string.IsNullOrEmpty(debugStr) && bool.TryParse(debugStr, out bool debug))
            {
                _debug = debug;
            }
            else
            {
                _debug = false;
            }
        }
        #endregion


        #region Actions

        [Route($"/Plugins/{nameof(TgBotStatPlugin)}")]
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            string indexFilePath = System.IO.Path.Combine(PluginPathProvider.PluginWwwRootDir(nameof(TgBotStatPlugin)), "index.html");

            return PhysicalFile(indexFilePath, "text/html");
        }

        [Route(nameof(Download))]
        public async Task<ActionResult> Download()
        {
            string dbFilePath = DbContext.DbFilePath;
            var fileStream = System.IO.File.OpenRead(dbFilePath);
            //System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();

            return File(fileStream: fileStream, contentType: "application/x-sqlite3", fileDownloadName: $"{nameof(TgBotStatPlugin)}.sqlite", enableRangeProcessing: true);
        }
        
        #endregion
    }
}
