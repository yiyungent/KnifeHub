using Konata.Core.Common;
using Konata.Core.Interfaces;
using Konata.Core.Interfaces.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PluginCore.Interfaces;
using PluginCore.IPlugins;
using QQBotHub.Web.RequestModels;
using QQBotHub.Web.ResponseModels;
using System.Threading.Tasks;
using static Konata.Core.Events.Model.CaptchaEvent;

namespace QQBotHub.Web.Controllers
{
    [Route("api/[action]")]
    [Authorize("PluginCoreAdmin")]
    [ApiController]
    public class HomeController : ControllerBase
    {

        #region Fields

        private readonly IPluginFinder _pluginFinder;

        private readonly bool _debug;

        #endregion

        #region Ctor
        public HomeController(IPluginFinder pluginFinder)
        {
            _pluginFinder = pluginFinder;
            string debugStr = Utils.EnvUtil.GetEnv("DEBUG");
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

        [Route("/")]
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            string indexFilePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html");

            return PhysicalFile(indexFilePath, "text/html");
        }


        [HttpGet]
        public async Task<BaseResponseModel> Uin()
        {
            BaseResponseModel responseModel = new BaseResponseModel();
            try
            {
                var settings = Utils.SettingsUtil.Get();
                string uin = settings?.Uin ?? "";
                if (!string.IsNullOrEmpty(QQBotStore.Bot?.Uin.ToString()))
                {
                    uin = QQBotStore.Bot?.Uin.ToString() ?? uin;
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
        public async Task<BaseResponseModel> Info()
        {
            BaseResponseModel responseModel = new BaseResponseModel();
            InfoResponseDataModel dataModel = new InfoResponseDataModel();

            if (QQBotStore.Bot == null)
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
                        isSuccessCaptcha = QQBotStore.Bot.SubmitSmsCode(captcha);
                        break;
                    case Konata.Core.Events.Model.CaptchaEvent.CaptchaType.Slider:
                        isSuccessCaptcha = QQBotStore.Bot.SubmitSliderTicket(captcha);
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
        public async Task<BaseResponseModel> Login(LoginRequestModel requestModel)
        {
            BaseResponseModel responseModel = new BaseResponseModel();
            try
            {
                var oldSettings = Utils.SettingsUtil.Get();
                SettingsModel newSettings = new SettingsModel();
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

                    Utils.SettingsUtil.Set(newSettings);
                }
                else if (requestModel.LoginType == "config")
                {
                    newSettings = new SettingsModel();
                    if (!string.IsNullOrEmpty(requestModel.BotKeyStore?.Trim()))
                    {
                        try
                        {
                            newSettings.BotKeyStore = Utils.JsonUtil.JsonStr2Obj<BotKeyStore>(requestModel.BotKeyStore.Trim());
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

                    Utils.SettingsUtil.Set(newSettings);
                }
                else
                {
                    responseModel.Code = -2;
                    responseModel.Message = "未知登录方式";

                    return await Task.FromResult(responseModel);
                }

                Task<bool> taskLogin = QQBotStore.Bot.Login();

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
        public async Task<BaseResponseModel> Logout()
        {
            BaseResponseModel responseModel = new BaseResponseModel();
            try
            {
                Task<bool> taskLogout = QQBotStore.Bot.Logout();

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
            string botConfigJsonStr = Utils.EnvUtil.GetEnv("BOT_CONFIG");
            if (!string.IsNullOrEmpty(botConfigJsonStr))
            {
                botConfig = Utils.JsonUtil.JsonStr2Obj<BotConfig>(botConfigJsonStr);
            }

            BotDevice botDevice = BotDevice.Default();
            string botDeviceJsonStr = Utils.EnvUtil.GetEnv("BOT_DEVICE");
            if (!string.IsNullOrEmpty(botDeviceJsonStr))
            {
                botDevice = Utils.JsonUtil.JsonStr2Obj<BotDevice>(botDeviceJsonStr);
            }

            string botKeyStoreJsonStr = Utils.EnvUtil.GetEnv("BOT_KEYSTORE");
            if (!string.IsNullOrEmpty(botKeyStoreJsonStr))
            {
                botKeyStore = Utils.JsonUtil.JsonStr2Obj<BotKeyStore>(botKeyStoreJsonStr);
            }
            #endregion

            var bot = BotFather.Create(botConfig, botDevice, botKeyStore);
            {
                // Print the log
                bot.OnLog += (s, e) =>
                {
#if DEBUG
                    Utils.LogUtil.Info(e.EventMessage);
#endif
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
                    Utils.LogUtil.Info($"群消息: {e.Message.Chain?.FirstOrDefault()?.ToString() ?? ""}");

                    var plugins = _pluginFinder.EnablePlugins<IQQBotPlugin>();
                    foreach (var plugin in plugins)
                    {
                        plugin.OnGroupMessage((s, e), e.Message.Chain?.FirstOrDefault()?.ToString() ?? "", e.GroupName, e.GroupUin, e.MemberUin);
                    }
                };

                // Handle messages from friend
                bot.OnFriendMessage += (s, e) =>
                {
                    Utils.LogUtil.Info($"好友消息: {e.Message.Chain?.FirstOrDefault()?.ToString() ?? ""}");

                    var plugins = _pluginFinder.EnablePlugins<IQQBotPlugin>();
                    foreach (var plugin in plugins)
                    {
                        plugin.OnFriendMessage((s, e), e.Message.Chain?.FirstOrDefault()?.ToString() ?? "", e.FriendUin);
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
            QQBotStore.Bot = bot;
        }

        #endregion

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
