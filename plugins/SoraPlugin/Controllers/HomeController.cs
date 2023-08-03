using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PluginCore;
using PluginCore.Interfaces;
using SoraPlugin.Utils;
using Sora;
using Sora.Interfaces;
using Sora.Net.Config;
using Sora.Util;
using YukariToolBox.LightLog;
using PluginCore.IPlugins;

namespace SoraPlugin.Controllers
{
    /// <summary>
    /// 其实也可以不写这个, 直接访问 Plugins/SoraPlugin/index.html
    /// 
    /// 下面的方法, 是去掉 index.html
    /// 
    /// 若 wwwroot 下有其它需要访问的文件, 如何 css, js, 而你又不想每次新增 action 指定返回, 则 Route 必须 Plugins/{PluginId},
    /// 这样访问 Plugins/SoraPlugin/css/main.css 就会访问到你插件下的 wwwroot/css/main.css
    /// </summary>
    [Route($"Plugins/{nameof(SoraPlugin)}")]
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
            string indexFilePath = System.IO.Path.Combine(PluginPathProvider.PluginWwwRootDir(nameof(SoraPlugin)), "index.html");

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
            SettingsModel settingsModel = PluginCore.PluginSettingsModelFactory.Create<SettingsModel>(nameof(SoraPlugin));

            // 确保以前的都取消
            #region 确保以前的都取消
            if (SoraBotStore.Bot.SoraService != null)
            {
                try
                {
                    await SoraBotStore.Bot.SoraService.StopService();
                    SoraBotStore.Bot.SoraService = null;
                }
                catch (Exception ex)
                {
                }
            }
            #endregion

            #region 日志
            //设置log等级
            Log.LogConfiguration
               .EnableConsoleOutput()
               .SetLogLevel(LogLevel.Info);
            #endregion

            #region 配置
            // 默认端口为8080
            // 实例化Sora服务器
            ISoraConfig soraConfig = null;
            if (settingsModel.Mode == "server")
            {
                // 反向 Websocket 连接
                soraConfig = new ServerConfig()
                {
                    Port = settingsModel.ServerConfig.Port,
                    AccessToken = settingsModel.ServerConfig.AccessToken,
                    AutoMarkMessageRead = settingsModel.ServerConfig.AutoMarkMessageRead,
                    BlockUsers = settingsModel.ServerConfig.BlockUsers,
                    EnableSocketMessage = settingsModel.ServerConfig.EnableSocketMessage,
                    EnableSoraCommandManager = settingsModel.ServerConfig.EnableSoraCommandManager,
                    Host = settingsModel.ServerConfig.Host,
                    SendCommandErrMsg = settingsModel.ServerConfig.SendCommandErrMsg,
                    SuperUsers = settingsModel.ServerConfig.SuperUsers,
                    ThrowCommandException = settingsModel.ServerConfig.ThrowCommandException,
                    UniversalPath = settingsModel.ServerConfig.UniversalPath
                };
            }
            else if (settingsModel.Mode == "client")
            {
                // 正向 Websocket 连接
                soraConfig = new ClientConfig()
                {
                    Port = settingsModel.ClientConfig.Port,
                    AccessToken = settingsModel.ClientConfig.AccessToken,
                    AutoMarkMessageRead = settingsModel.ClientConfig.AutoMarkMessageRead,
                    BlockUsers = settingsModel.ClientConfig.BlockUsers,
                    EnableSocketMessage = settingsModel.ClientConfig.EnableSocketMessage,
                    EnableSoraCommandManager = settingsModel.ClientConfig.EnableSoraCommandManager,
                    Host = settingsModel.ClientConfig.Host,
                    SendCommandErrMsg = settingsModel.ClientConfig.SendCommandErrMsg,
                    SuperUsers = settingsModel.ClientConfig.SuperUsers,
                    ThrowCommandException = settingsModel.ClientConfig.ThrowCommandException,
                    UniversalPath = settingsModel.ClientConfig.UniversalPath,
                };
            }
            else
            {
                // 非法 mode
                return Content("[Error]: 设置中的 Mode 无法识别");
            }
            var service = SoraServiceFactory.CreateService(soraConfig);
            #endregion

            #region 注册事件
            service.Event.OnGroupMessage += async (msgType, eventArgs) =>
            {
                var plugins = _pluginFinder.EnablePlugins<ISoraPlugin>().ToList();
                Utils.LogUtil.Info($"响应: {plugins?.Count.ToString()} 个插件:");
                foreach (var plugin in plugins)
                {
                    Utils.LogUtil.Info($"插件: {plugin.GetType().ToString()}");

                    plugin.OnGroupMessage(msgType, eventArgs);
                }

                #region 演示模式
                if (settingsModel.UseDemoModel)
                {
                    if (eventArgs.Message.GetText().Contains("hello"))
                    {
                        //发送群消息(List消息段)
                        await eventArgs.SourceGroup.SendGroupMessage(eventArgs.Message.MessageBody);
                    }
                }
                #endregion
            };
            service.Event.OnPrivateMessage += async (msgType, eventArgs) =>
            {
                var plugins = _pluginFinder.EnablePlugins<ISoraPlugin>().ToList();
                Utils.LogUtil.Info($"响应: {plugins?.Count.ToString()} 个插件:");
                foreach (var plugin in plugins)
                {
                    Utils.LogUtil.Info($"插件: {plugin.GetType().ToString()}");

                    plugin.OnPrivateMessage(msgType, eventArgs);
                }
            };
            #endregion

            #region 启动
            //启动服务并捕捉错误
            service.StartService()
                         .RunCatch(e => Log.Error("Sora Service", Log.ErrorLogBuilder(e)));
            #endregion

            // 保存起来
            SoraBotStore.Bot.SoraService = service;

            // TODO: 暂时这么做, 以后优化界面
            return Content("尝试启动中, 请耐心等待, 出现本页面也有可能已启动完成");
        }

        [Route(nameof(Download))]
        [Authorize("PluginCore.Admin")]
        public async Task<ActionResult> Download()
        {
            string dbFilePath = DbContext.DbFilePath;
            var fileStream = System.IO.File.OpenRead(dbFilePath);
            //System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();

            return File(fileStream: fileStream, contentType: "application/x-sqlite3", fileDownloadName: $"{nameof(SoraPlugin)}.sqlite", enableRangeProcessing: true);
        }
    }
}
