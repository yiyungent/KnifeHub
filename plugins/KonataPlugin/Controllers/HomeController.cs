using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PluginCore;
using PluginCore.Interfaces;
using KonataPlugin.Utils;
using KonataPlugin.ResponseModels;
using KonataPlugin.RequestModels;
using Konata.Core.Common;
using Konata.Core.Interfaces;
using static Konata.Core.Events.Model.CaptchaEvent;
using PluginCore.IPlugins;
using Konata.Core.Interfaces.Api;
using Konata.Core.Message;
using Konata.Core.Message.Model;

namespace KonataPlugin.Controllers
{
    /// <summary>
    /// 其实也可以不写这个, 直接访问 Plugins/ZhiDaoPlugin/index.html
    /// 
    /// 下面的方法, 是去掉 index.html
    /// 
    /// 若 wwwroot 下有其它需要访问的文件, 如何 css, js, 而你又不想每次新增 action 指定返回, 则 Route 必须 Plugins/{PluginId},
    /// 这样访问 Plugins/HelloWorldPlugin/css/main.css 就会访问到你插件下的 wwwroot/css/main.css
    /// </summary>
    [Route($"api/Plugins/{nameof(KonataPlugin)}")]
    [Authorize("PluginCore.Admin")]
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

        [Route($"/Plugins/{nameof(KonataPlugin)}")]
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            string indexFilePath = System.IO.Path.Combine(PluginPathProvider.PluginWwwRootDir(nameof(KonataPlugin)), "index.html");

            return PhysicalFile(indexFilePath, "text/html");
        }


        [HttpGet]
        [Route(nameof(Uin))]
        public async Task<BaseResponseModel> Uin()
        {
            BaseResponseModel responseModel = new BaseResponseModel();
            try
            {
                var settings = Utils.SettingsUtil.Get(nameof(KonataPlugin));
                string uin = settings?.Uin ?? "";
                if (!string.IsNullOrEmpty(KonataBotStore.Bot?.Uin.ToString()))
                {
                    uin = KonataBotStore.Bot?.Uin.ToString() ?? uin;
                }

                responseModel.Code = 1;
                responseModel.Message = "获取成功";
                responseModel.Data = uin;
            }
            catch (Exception ex)
            {
                responseModel.Code = -1;
                responseModel.Message = "获取失败";
                responseModel.Data = ex.ToString();
            }

            return await Task.FromResult(responseModel);
        }

        /// <summary>
        /// 登录后，定时访问此api, 获取验证等信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route(nameof(Info))]
        public async Task<BaseResponseModel> Info()
        {
            BaseResponseModel responseModel = new BaseResponseModel();
            InfoResponseDataModel dataModel = new InfoResponseDataModel();

            if (KonataBotStore.Bot == null)
            {
                // 未点击登录过
                responseModel.Code = 1;
                responseModel.Message = "未登录";

                dataModel.CaptchaTip = "";
                dataModel.IsOnline = false;
                dataModel.CaptchaType = "";
                dataModel.CaptchaUpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                responseModel.Data = dataModel;

                return responseModel;
            }

            //if (QQBotStore.Bot.IsOnline())
            if (CaptchaStore.IsOnline)
            {
                responseModel.Code = 2;
                responseModel.Message = "已处于登录状态, 无需验证";

                dataModel.CaptchaTip = "";
                dataModel.IsOnline = true;
                dataModel.CaptchaType = "";
                dataModel.CaptchaUpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                responseModel.Data = dataModel;

                return responseModel;
            }

            // 已经点击登录过一次
            responseModel.Code = 3;
            responseModel.Message = "需要验证";
            switch (CaptchaStore.CaptchaType)
            {
                case Konata.Core.Events.Model.CaptchaEvent.CaptchaType.Sms:
                    dataModel.CaptchaType = "短信验证";
                    break;
                case Konata.Core.Events.Model.CaptchaEvent.CaptchaType.Slider:
                    dataModel.CaptchaType = "滑块验证";
                    break;
                case Konata.Core.Events.Model.CaptchaEvent.CaptchaType.Unknown:
                    dataModel.CaptchaType = "未知验证";
                    break;
                default:
                    dataModel.CaptchaType = "未知验证-不匹配的验证类型";
                    break;
            }
            dataModel.IsOnline = false;
            dataModel.CaptchaTip = CaptchaStore.CaptchaTip;
            dataModel.CaptchaUpdateTime = CaptchaStore.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss");

            responseModel.Data = dataModel;

            return await Task.FromResult(responseModel);
        }

        /// <summary>
        /// 提交验证信息
        /// </summary>
        /// <param name="captcha"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(nameof(SubmitCaptcha))]
        public async Task<BaseResponseModel> SubmitCaptcha([FromBody] SubmitCaptchaRequestModel requestModel)
        {
            BaseResponseModel responseModel = new BaseResponseModel();
            string captcha = requestModel.Captcha;
            if (string.IsNullOrEmpty(captcha))
            {
                responseModel.Code = -1;
                responseModel.Message = "captcha 不能为空";

                return responseModel;
            }
            bool isSuccessCaptcha = false;
            try
            {
                switch (CaptchaStore.CaptchaType)
                {
                    case Konata.Core.Events.Model.CaptchaEvent.CaptchaType.Sms:
                        isSuccessCaptcha = KonataBotStore.Bot.SubmitSmsCode(captcha);
                        break;
                    case Konata.Core.Events.Model.CaptchaEvent.CaptchaType.Slider:
                        isSuccessCaptcha = KonataBotStore.Bot.SubmitSliderTicket(captcha);
                        break;
                    case Konata.Core.Events.Model.CaptchaEvent.CaptchaType.Unknown:
                        break;
                    default:
                        break;
                }

                responseModel.Code = 1;
                responseModel.Message = $"{(isSuccessCaptcha ? "验证通过" : "验证失败, 请重新验证")}";
            }
            catch (System.Exception ex)
            {
                responseModel.Code = -3;
                responseModel.Message = "提交验证失败";
                responseModel.Data = ex.ToString();

                Utils.LogUtil.Exception(ex);
            }

            return await Task.FromResult(responseModel);
        }

        [HttpPost]
        [Route(nameof(Login))]
        public async Task<BaseResponseModel> Login([FromBody] LoginRequestModel requestModel)
        {
            BaseResponseModel responseModel = new BaseResponseModel();
            try
            {
                var oldSettings = Utils.SettingsUtil.Get(nameof(KonataPlugin));
                SettingsModel newSettings = new SettingsModel();
                newSettings.UseDemoModel = oldSettings.UseDemoModel;
                newSettings.AdminQQ = oldSettings.AdminQQ;
                if (requestModel.LoginType == "password")
                {
                    newSettings = new SettingsModel
                    {
                        Uin = requestModel.Uin,
                        Password = requestModel.Password,
                    };
                    if (string.IsNullOrEmpty(requestModel.Password?.Trim()))
                    {
                        newSettings.Password = oldSettings.Password;
                    }

                    InitQQBot(botKeyStore: new BotKeyStore(uin: newSettings.Uin, password: newSettings.Password));

                    Utils.SettingsUtil.Set(nameof(KonataPlugin), newSettings);
                }
                else if (requestModel.LoginType == "config")
                {
                    newSettings = new SettingsModel();
                    if (!string.IsNullOrEmpty(requestModel.BotKeyStore?.Trim()))
                    {
                        try
                        {
                            newSettings.BotKeyStore = JsonUtil.JsonStr2Obj<BotKeyStore>(requestModel.BotKeyStore.Trim());
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("requestModel.BotKeyStore 转换失败:");
                            Console.WriteLine(ex.ToString());
                        }
                    }

                    if (string.IsNullOrEmpty(requestModel.BotKeyStore?.Trim()))
                    {
                        newSettings.BotKeyStore = oldSettings.BotKeyStore;
                    }

                    InitQQBot(botKeyStore: newSettings.BotKeyStore);

                    Utils.SettingsUtil.Set(nameof(KonataPlugin), newSettings);
                }
                else
                {
                    responseModel.Code = -2;
                    responseModel.Message = "未知登录方式";

                    return await Task.FromResult(responseModel);
                }

                Task<bool> taskLogin = KonataBotStore.Bot.Login();

                responseModel.Code = 1;
                responseModel.Message = "提交登录成功";
            }
            catch (Exception ex)
            {
                responseModel.Code = -1;
                responseModel.Message = "提交登录失败";
                responseModel.Data = ex.ToString();
            }

            return await Task.FromResult(responseModel);
        }

        [HttpPost]
        [Route(nameof(Logout))]
        public async Task<BaseResponseModel> Logout()
        {
            BaseResponseModel responseModel = new BaseResponseModel();
            try
            {
                Task<bool> taskLogout = KonataBotStore.Bot.Logout();

                responseModel.Code = 1;
                responseModel.Message = "提交退出成功";
            }
            catch (Exception ex)
            {
                responseModel.Code = -1;
                responseModel.Message = "提交退出失败";
                responseModel.Data = ex.ToString();
            }

            return await Task.FromResult(responseModel);
        }

        #endregion

        #region Helpers

        [NonAction]
        public void InitQQBot(BotKeyStore botKeyStore)
        {
            #region Bot

            // Create a bot instance
            #region 准备数据
            // 优先从 环境变量 中获取
            BotConfig botConfig = BotConfig.Default();
            string botConfigJsonStr = EnvUtil.GetEnv("BOT_CONFIG");
            if (!string.IsNullOrEmpty(botConfigJsonStr))
            {
                botConfig = JsonUtil.JsonStr2Obj<BotConfig>(botConfigJsonStr);
            }

            BotDevice botDevice = BotDevice.Default();
            string botDeviceJsonStr = EnvUtil.GetEnv("BOT_DEVICE");
            if (!string.IsNullOrEmpty(botDeviceJsonStr))
            {
                botDevice = JsonUtil.JsonStr2Obj<BotDevice>(botDeviceJsonStr);
            }

            string botKeyStoreJsonStr = EnvUtil.GetEnv("BOT_KEYSTORE");
            if (!string.IsNullOrEmpty(botKeyStoreJsonStr))
            {
                botKeyStore = JsonUtil.JsonStr2Obj<BotKeyStore>(botKeyStoreJsonStr);
            }
            var settings = Utils.SettingsUtil.Get(nameof(KonataPlugin));
            #endregion

            var bot = BotFather.Create(botConfig, botDevice, botKeyStore);
            {
                // Print the log
                bot.OnLog += (s, e) =>
                {
                    //#if DEBUG
                    //                    Utils.LogUtil.Info(e.EventMessage);
                    //#endif
                    if (_debug)
                    {
                        Utils.LogUtil.Info(e.EventMessage);
                    }
                };

                // Handle the captcha
                bot.OnCaptcha += (s, e) =>
                {
                    Utils.LogUtil.Info("QQ 登录验证:");
                    CaptchaStore.UpdateTime = DateTime.Now;
                    CaptchaStore.CaptchaType = e.Type;
                    if (e.Type == CaptchaType.Slider)
                    {
                        Utils.LogUtil.Info(e.SliderUrl);
                        //((Bot)s).SubmitSliderTicket(Console.ReadLine());
                        CaptchaStore.CaptchaTip = $"{e.SliderUrl}";
                    }
                    else if (e.Type == CaptchaType.Sms)
                    {
                        Utils.LogUtil.Info(e.Phone);
                        //((Bot)s).SubmitSmsCode(Console.ReadLine());
                        CaptchaStore.CaptchaTip = $"{e.Phone}";
                    }
                };

                // Handle messages from group
                bot.OnGroupMessage += (s, e) =>
                {
                    Utils.LogUtil.Info($"群消息: {DateTime.Now.ToString()}: {e.Message.Chain?.FirstOrDefault()?.ToString() ?? ""}");

                    var plugins = _pluginFinder.EnablePlugins<IQQBotPlugin>().ToList();
                    Utils.LogUtil.Info($"响应: {plugins?.Count.ToString()} 个插件:");
                    foreach (var plugin in plugins)
                    {
                        Utils.LogUtil.Info($"插件: {plugin.GetType().ToString()}");
                        if (e.Message.Sender.Uin != s.Uin)
                        {
                            // 排除机器人自己
                            plugin.OnGroupMessage((s, e), e.Message.Chain?.FirstOrDefault()?.ToString() ?? "", e.GroupName, e.GroupUin, e.MemberUin);
                        }
                    }

                    // 演示模式
                    if (settings.UseDemoModel)
                    {
                        if (e.Message.Sender.Uin != s.Uin)
                        {
                            // 排除机器人自己
                            // 只回应 @机器人
                            if (IsAtBot(e.Message.Chain, s.Uin))
                            {
                                bot.SendGroupMessage(e.GroupUin, $"收到消息啦！您发送的消息为 -> {ConvertToString(e.Message.Chain)}");
                            }
                        }
                    }
                };

                // Handle messages from friend
                bot.OnFriendMessage += (s, e) =>
                {
                    Utils.LogUtil.Info($"好友消息: {DateTime.Now.ToString()}: {e.Message.Chain?.FirstOrDefault()?.ToString() ?? ""}");

                    // 在获取插件这步正常, 没有触发 bot.OnFriendMessage 
                    var plugins = _pluginFinder.EnablePlugins<IQQBotPlugin>().ToList();
                    Utils.LogUtil.Info($"响应: {plugins?.Count.ToString()} 个插件:");
                    //Utils.LogUtil.Info($"响应: {plugins?.Count.ToString()} 个插件: {e.Message.Chain?.FirstOrDefault()?.ToString() ?? ""}");
                    foreach (var plugin in plugins)
                    {
                        Utils.LogUtil.Info($"插件: {plugin.GetType().ToString()}");
                        if (e.Message.Sender.Uin != s.Uin)
                        {
                            // 排除机器人自己
                            plugin.OnFriendMessage((s, e), e.Message.Chain?.FirstOrDefault()?.ToString() ?? "", e.FriendUin);
                        }
                    }
                };

                bot.OnBotOnline += (s, e) =>
                {
                    Utils.LogUtil.Info($"{s.Name} 上线");

                    CaptchaStore.IsOnline = true;

                    var plugins = _pluginFinder.EnablePlugins<IQQBotPlugin>();
                    foreach (var plugin in plugins)
                    {
                        plugin.OnBotOnline((s, e), s.Name, s.Uin);
                    }
                };

                bot.OnBotOffline += (s, e) =>
                {
                    Utils.LogUtil.Info($"{s.Name} 离线");

                    CaptchaStore.IsOnline = false;

                    var plugins = _pluginFinder.EnablePlugins<IQQBotPlugin>();
                    foreach (var plugin in plugins)
                    {
                        plugin.OnBotOffline((s, e), s.Name, s.Uin);
                    }
                };
                // ... More handlers
            }

            // Do login
            //Task<bool> loginTask = bot.Login();

            // 下方操作会阻塞, 并且是阻塞到过登录验证
            //if (!bot.Login().Result)
            //{
            //    Utils.LogUtil.Info($"{nameof(QQBotPlugin)} 启用后 QQ 自动登录失败");
            //    return base.AfterEnable();
            //}
            //else
            //{
            //    Utils.LogUtil.Info($"{nameof(QQBotPlugin)} 启用后 QQ 自动登录成功");
            //}

            #endregion

            // 登录成功, 保存起来
            KonataBotStore.Bot = bot;
        }

        #endregion

        [Route(nameof(Download))]
        public async Task<ActionResult> Download()
        {
            string dbFilePath = DbContext.DbFilePath;
            var fileStream = System.IO.File.OpenRead(dbFilePath);
            //System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();

            return File(fileStream: fileStream, contentType: "application/x-sqlite3", fileDownloadName: $"{nameof(KonataPlugin)}.sqlite", enableRangeProcessing: true);
        }

        [NonAction]
        private string ConvertToString(MessageChain chains)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in chains)
            {
                switch (item.Type)
                {
                    case BaseChain.ChainType.At:
                        break;
                    case BaseChain.ChainType.Reply:
                        break;
                    case BaseChain.ChainType.Text:
                        sb.AppendLine(item.ToString());
                        break;
                    case BaseChain.ChainType.Image:
                        break;
                    case BaseChain.ChainType.Flash:
                        break;
                    case BaseChain.ChainType.Record:
                        break;
                    case BaseChain.ChainType.Video:
                        break;
                    case BaseChain.ChainType.QFace:
                        break;
                    case BaseChain.ChainType.BFace:
                        break;
                    case BaseChain.ChainType.Xml:
                        break;
                    case BaseChain.ChainType.MultiMsg:
                        break;
                    case BaseChain.ChainType.Json:
                        break;
                    default:
                        break;
                }
            }

            return sb.ToString();
        }

        [NonAction]
        private bool IsAtBot(MessageChain baseChains, uint botUin)
        {
            bool isAtBot = false;
            foreach (var item in baseChains)
            {
                switch (item.Type)
                {
                    case BaseChain.ChainType.At:
                        AtChain atChain = (AtChain)item;
                        isAtBot = atChain.AtUin == botUin;
                        //if (isAtBot = atChain.AtUin == botUin)
                        //{
                        //    break;
                        //}
                        break;
                    case BaseChain.ChainType.Reply:
                        //ReplyChain replyChain = (ReplyChain)item;
                        //isAtBot = atChain.AtUin == botUin;
                        break;
                    case BaseChain.ChainType.Text:
                        break;
                    case BaseChain.ChainType.Image:
                        break;
                    case BaseChain.ChainType.Flash:
                        break;
                    case BaseChain.ChainType.Record:
                        break;
                    case BaseChain.ChainType.Video:
                        break;
                    case BaseChain.ChainType.QFace:
                        break;
                    case BaseChain.ChainType.BFace:
                        break;
                    case BaseChain.ChainType.Xml:
                        break;
                    case BaseChain.ChainType.MultiMsg:
                        break;
                    case BaseChain.ChainType.Json:
                        break;
                    default:
                        break;
                }
            }

            return isAtBot;
        }
    }

    #region More

    public static class CaptchaStore
    {
        public static Konata.Core.Events.Model.CaptchaEvent.CaptchaType CaptchaType { get; set; }

        public static string CaptchaTip { get; set; }

        public static DateTime UpdateTime { get; set; }

        public static bool IsOnline { get; set; }
    }

    #endregion

}
