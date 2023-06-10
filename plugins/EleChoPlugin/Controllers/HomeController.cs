using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PluginCore;
using PluginCore.Interfaces;
using EleChoPlugin.Utils;
using PluginCore.IPlugins;
using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Post;
using EleCho.GoCqHttpSdk.Message;

namespace EleChoPlugin.Controllers
{
    /// <summary>
    /// 其实也可以不写这个, 直接访问 Plugins/EleChoPlugin/index.html
    /// 
    /// 下面的方法, 是去掉 index.html
    /// 
    /// 若 wwwroot 下有其它需要访问的文件, 如何 css, js, 而你又不想每次新增 action 指定返回, 则 Route 必须 Plugins/{PluginId},
    /// 这样访问 Plugins/EleChoPlugin/css/main.css 就会访问到你插件下的 wwwroot/css/main.css
    /// </summary>
    [Route($"Plugins/{nameof(EleChoPlugin)}")]
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

        #region Actions
        public async Task<ActionResult> Get()
        {
            string indexFilePath = System.IO.Path.Combine(PluginPathProvider.PluginWwwRootDir(nameof(EleChoPlugin)), "index.html");

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
            SettingsModel settingsModel = PluginCore.PluginSettingsModelFactory.Create<SettingsModel>(nameof(EleChoPlugin));

            // 确保以前的都释放
            #region 确保以前的都释放
            if (EleChoBotStore.Bots != null && EleChoBotStore.Bots.Count >= 1)
            {
                foreach (var bot in EleChoBotStore.Bots)
                {
                    try
                    {
                        var botConfig = settingsModel.EleChoConfigs.FirstOrDefault(m => m.ConfigId == bot.ConfigId);
                        if (botConfig != null)
                        {
                            // TODO: 关闭并释放
                            switch (botConfig.Mode)
                            {
                                case "http":
                                    bot.CqHttpSession.Dispose();
                                    await bot.CqRHttpSession.StopAsync();
                                    bot.CqRHttpSession.Dispose();
                                    break;
                                case "ws":
                                    // TODO: ws
                                    break;
                                case "ws reverse":
                                    // TODO: ws reverse
                                    break;
                                default:
                                    // TODO: default
                                    break;
                            }
                            bot.CqHttpSession = null;
                            bot.CqRHttpSession = null;
                            bot.CqWsSession = null;
                            bot.CqRWsSession = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("确保以前的都释放: ");
                        Console.WriteLine(ex.ToString());
                    }
                }
                EleChoBotStore.Bots.Clear();
            }
            #endregion

            foreach (var botConfig in settingsModel.EleChoConfigs)
            {
                if (botConfig.Enable)
                {
                    BotItem(botConfig, settingsModel);
                    //await BotItem(botConfig, settingsModel);
                }
            }

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

            return File(fileStream: fileStream, contentType: "application/x-sqlite3", fileDownloadName: $"{nameof(EleChoPlugin)}.sqlite", enableRangeProcessing: true);
        }
        #endregion

        #region NonActions
        [NonAction]
        public async Task BotItem(SettingsModel.EleChoConfigItemModel botConfig, SettingsModel settings)
        {
            var bot = new EleChoBotStore.BotItemModel();
            bot.ConfigId = botConfig.ConfigId;
            bot.Mode = botConfig.Mode;

            #region 配置并启动
            switch (botConfig.Mode)
            {
                case "http":
                    bot.CqHttpSession = new CqHttpSession(new CqHttpSessionOptions
                    {
                        AccessToken = botConfig.CqHttpSession.BaseUri,
                        BaseUri = new Uri(botConfig.CqHttpSession.BaseUri)
                    });
                    bot.CqRHttpSession = new CqRHttpSession(new CqRHttpSessionOptions
                    {
                        BaseUri = new Uri(botConfig.CqRHttpSession.BaseUri),
                        Secret = botConfig.CqRHttpSession.Secret
                    });

                    // 启动
                    await bot.CqRHttpSession.StartAsync();

                    #region 注册事件
                    // 使用 EleCho 特性: 主动使用插件
                    bot.CqRHttpSession.UsePlugin(new DemoPlugin(bot));

                    #region 插件事件派发: UsePlugin
                    Utils.LogUtil.Info($"{botConfig.ConfigId} 开始配置");
                    var plugins = _pluginFinder.EnablePlugins<IEleChoPlugin>().ToList();
                    Utils.LogUtil.Info($"响应: {plugins?.Count.ToString()} 个插件:");
                    foreach (var plugin in plugins)
                    {
                        Utils.LogUtil.Info($"插件: {plugin.GetType().ToString()}");
                        try
                        {
                            var cqPostPlugins = plugin.UseCqPostPlugins(bot);
                            if (cqPostPlugins != null && cqPostPlugins.Count >= 1)
                            {
                                foreach (var item in cqPostPlugins)
                                {
                                    bot.CqRHttpSession.UsePlugin(item);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogUtil.Exception(ex);
                        }
                    }
                    #endregion

                    #endregion
                    break;
                case "ws":
                    // TODO: websocket
                    break;
                case "ws reverse":
                    // TODO: websocket reverse
                    break;
                default:
                    // TODO: 默认
                    break;
            }
            #endregion

            // 保存起来
            EleChoBotStore.Bots.Add(bot);
        }
        #endregion
    }

    public class DemoPlugin : CqPostPlugin
    {
        public EleChoBotStore.BotItemModel Bot { get; set; }

        public ICqActionSession CqActionSession { get; set; }

        public DemoPlugin(EleChoBotStore.BotItemModel bot)
        {
            this.Bot = bot;
            switch (bot.Mode)
            {
                case "http":
                    this.CqActionSession = this.Bot.CqHttpSession;
                    break;
                case "ws":
                    this.CqActionSession = this.Bot.CqWsSession;
                    break;
                case "ws reverse":
                    this.CqActionSession = this.Bot.CqRWsSession;
                    break;
                default:
                    break;
            }
        }

        public override async Task OnGroupMessageReceivedAsync(CqGroupMessagePostContext context)
        {
            SettingsModel settingsModel = PluginCore.PluginSettingsModelFactory.Create<SettingsModel>(nameof(EleChoPlugin));
            if (!settingsModel.UseDemoModel)
            {
                return;
            }

            string text = context.Message.Text;
            if (text.StartsWith("TTS:", StringComparison.InvariantCultureIgnoreCase))
            {
                await this.CqActionSession.SendGroupMessageAsync(context.GroupId, new CqMessage(new CqTtsMsg(text[4..])));
            }
            else if (text.StartsWith("ToFace:", StringComparison.InvariantCultureIgnoreCase))
            {
                if (CqFaceMsg.FromName(text[7..]) is CqFaceMsg face)
                {
                    await this.CqActionSession.SendGroupMessageAsync(context.GroupId, new CqMessage(face));
                }
            }
        }

        public override async Task OnGroupMessageRecalledAsync(CqGroupMessageRecalledPostContext context)
        {
            SettingsModel settingsModel = PluginCore.PluginSettingsModelFactory.Create<SettingsModel>(nameof(EleChoPlugin));
            if (!settingsModel.UseDemoModel)
            {
                return;
            }

            var msg = (await this.CqActionSession.GetMessageAsync(context.MessageId));
            if (msg.Message.Text.StartsWith("test:", StringComparison.InvariantCultureIgnoreCase))
            {
                await this.CqActionSession.SendGroupMessageAsync(context.GroupId, new CqMessage("让我康康你撤回了什么: ", msg.Message));
            }
        }
    }
}
