using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PluginCore;
using PluginCore.Interfaces;
using CocoaPlugin.IPlugins;
using CocoaPlugin.Utils;
using Maila.Cocoa.Framework;

namespace CocoaPlugin.Controllers
{
    /// <summary>
    /// 其实也可以不写这个, 直接访问 Plugins/ZhiDaoPlugin/index.html
    /// 
    /// 下面的方法, 是去掉 index.html
    /// 
    /// 若 wwwroot 下有其它需要访问的文件, 如何 css, js, 而你又不想每次新增 action 指定返回, 则 Route 必须 Plugins/{PluginId},
    /// 这样访问 Plugins/HelloWorldPlugin/css/main.css 就会访问到你插件下的 wwwroot/css/main.css
    /// </summary>
    [Route($"Plugins/{nameof(CocoaPlugin)}")]
    public class HomeController : Controller
    {
        #region Fields

        private readonly IPluginFinder _pluginFinder;

        private readonly bool _debug;

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

        public async Task<ActionResult> Get()
        {
            string indexFilePath = System.IO.Path.Combine(PluginPathProvider.PluginWwwRootDir(nameof(CocoaPlugin)), "index.html");

            return PhysicalFile(indexFilePath, "text/html");
        }

        /// <summary>
        /// 将所有 settings.json 中的 bot 尝试登录
        /// </summary>
        /// <returns></returns>
        [Route(nameof(Start))]
        [Authorize("PluginCore.Admin")]
        public async Task<ActionResult> Start()
        {
            SettingsModel settingsModel = PluginCore.PluginSettingsModelFactory.Create<SettingsModel>(nameof(CocoaPlugin));

            // 确保以前的都取消
            #region 确保以前的都取消

            #endregion

            BotStartupConfig config = new(verifyKey: settingsModel.VerifyKey, qqId: settingsModel.BotQQ, host: settingsModel.Host, port: settingsModel.Port);
            CocoaBotStore.Bot.BotStartupConfig = config;
            var succeed = await BotStartup.ConnectAndInit(config); // 连接 Mirai 并初始化
            string tip = succeed ? "CocoaPlugin 启动成功" : "CocoaPlugin 启动失败";

            Console.WriteLine(tip);

            // TODO: 暂时这么做, 以后优化界面
            return Content(tip);
        }

        [Route(nameof(Download))]
        [Authorize("PluginCore.Admin")]
        public async Task<ActionResult> Download()
        {
            string dbFilePath = DbContext.DbFilePath;
            var fileStream = System.IO.File.OpenRead(dbFilePath);
            //System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();

            return File(fileStream: fileStream, contentType: "application/x-sqlite3", fileDownloadName: $"{nameof(CocoaPlugin)}.sqlite", enableRangeProcessing: true);
        }
    }
}
