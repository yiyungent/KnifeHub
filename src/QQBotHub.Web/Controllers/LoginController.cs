using Konata.Core.Interfaces;
using Konata.Core.Interfaces.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PluginCore.Interfaces;
using PluginCore.IPlugins;
using QQBotHub.Web.ResponseModels;
using System.Threading.Tasks;
using static Konata.Core.Events.Model.CaptchaEvent;

namespace QQBotHub.Web.Controllers
{
    [Route("api/[controller]")]
    [Authorize("PluginCoreAdmin")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        public static Konata.Core.Events.Model.CaptchaEvent.CaptchaType CaptchaType { get; set; }

        public static string CaptchaMessage { get; set; }

        private readonly IPluginFinder _pluginFinder;

        public LoginController(IPluginFinder pluginFinder)
        {
            if (QQBotStore.Bot == null)
            {
                InitQQBot();
            }
            _pluginFinder = pluginFinder;
        }

        /// <summary>
        /// 登录后，定时访问此api, 获取验证
        /// </summary>
        /// <returns></returns>
        [Route(nameof(Captcha))]
        [HttpGet]
        public async Task<BaseResponseModel> Captcha()
        {
            BaseResponseModel responseModel = new BaseResponseModel();

            if (QQBotStore.Bot.IsOnline())
            {
                responseModel.Code = 1;
                responseModel.Message = "已处于登录状态, 无需验证";

                return responseModel;
            }

            responseModel.Code = 2;
            responseModel.Data = CaptchaMessage;
            switch (CaptchaType)
            {
                case Konata.Core.Events.Model.CaptchaEvent.CaptchaType.Sms:
                    responseModel.Message = "短信验证";
                    break;
                case Konata.Core.Events.Model.CaptchaEvent.CaptchaType.Slider:
                    responseModel.Message = "滑块验证";
                    break;
                case Konata.Core.Events.Model.CaptchaEvent.CaptchaType.Unknown:
                    responseModel.Message = "未知验证";
                    break;
                default:
                    responseModel.Message = "未知验证-不匹配的验证类型";
                    break;
            }

            return await Task.FromResult(responseModel);
        }

        /// <summary>
        /// 提交验证信息
        /// </summary>
        /// <param name="captcha"></param>
        /// <returns></returns>
        [Route(nameof(SubmitCaptcha))]
        [HttpPost, HttpGet]
        public async Task<BaseResponseModel> SubmitCaptcha([FromQuery] string captcha)
        {
            BaseResponseModel responseModel = new BaseResponseModel();
            if (string.IsNullOrEmpty(captcha))
            {
                responseModel.Code = -1;
                responseModel.Message = "captcha 不能为空";

                return responseModel;
            }
            try
            {
                switch (CaptchaType)
                {
                    case Konata.Core.Events.Model.CaptchaEvent.CaptchaType.Sms:
                        QQBotStore.Bot.SubmitSmsCode(captcha);
                        break;
                    case Konata.Core.Events.Model.CaptchaEvent.CaptchaType.Slider:
                        QQBotStore.Bot.SubmitSliderTicket(captcha);
                        break;
                    case Konata.Core.Events.Model.CaptchaEvent.CaptchaType.Unknown:
                        break;
                    default:
                        break;
                }

                responseModel.Code = 1;
                responseModel.Message = "提交验证成功";
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

        [Route(nameof(IsOnline))]
        [HttpGet]
        public async Task<BaseResponseModel> IsOnline()
        {
            BaseResponseModel responseModel = new BaseResponseModel();
            bool isOnline = QQBotStore.Bot.IsOnline();
            responseModel.Data = isOnline;

            return await Task.FromResult(responseModel);
        }


        [Route("/Login")]
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            if (!QQBotStore.Bot.IsOnline())
            {
                Task<bool> taskLogin = QQBotStore.Bot.Login();
            }

            string indexFilePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "login.html");

            return PhysicalFile(indexFilePath, "text/html");
        }


        public void InitQQBot()
        {
            #region Bot

            // Create a bot instance
            var bot = BotFather.Create(QQBotStore.BotConfig, QQBotStore.BotDevice, QQBotStore.BotKeyStore);
            {
                // Print the log
                bot.OnLog += (s, e) =>
                {
                    //Utils.LogUtil.Info(e.EventMessage);
                };

                // Handle the captcha
                bot.OnCaptcha += (s, e) =>
                {
                    Utils.LogUtil.Info("QQ 登录验证:");
                    if (e.Type == CaptchaType.Slider)
                    {
                        Utils.LogUtil.Info(e.SliderUrl);
                        //((Bot)s).SubmitSliderTicket(Console.ReadLine());

                        Controllers.LoginController.CaptchaType = CaptchaType.Slider;
                        Controllers.LoginController.CaptchaMessage = $"{e.SliderUrl}";
                    }
                    else if (e.Type == CaptchaType.Sms)
                    {
                        Utils.LogUtil.Info(e.Phone);
                        //((Bot)s).SubmitSmsCode(Console.ReadLine());

                        Controllers.LoginController.CaptchaType = CaptchaType.Sms;
                        Controllers.LoginController.CaptchaMessage = $"{e.Phone}";
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

                    var plugins = _pluginFinder.EnablePlugins<IQQBotPlugin>();
                    foreach (var plugin in plugins)
                    {
                        plugin.OnBotOnline((s, e), s.Name, s.Uin);
                    }
                };

                bot.OnBotOffline += (s, e) =>
                {
                    Utils.LogUtil.Info($"{s.Name} 离线");

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

    }
}
