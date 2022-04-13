using Konata.Core.Interfaces.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PluginCore;
using QQBotPlugin.ResponseModels;
using System.Threading.Tasks;

namespace QQBotPlugin.Controllers
{
    [Route("api/plugins/QQBotPlugin/{controller}")]
    [Authorize("PluginCoreAdmin")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        public static Konata.Core.Events.Model.CaptchaEvent.CaptchaType CaptchaType { get; set; }

        public static string CaptchaMessage { get; set; }

        /// <summary>
        /// 登录后，定时访问此api, 获取验证
        /// </summary>
        /// <returns></returns>
        [Route(nameof(Captcha))]
        [HttpGet]
        public async Task<BaseResponseModel> Captcha()
        {
            BaseResponseModel responseModel = new BaseResponseModel();

            if (BotPluginStore.Bot.IsOnline())
            {
                responseModel.Code = 1;
                responseModel.Message = "已处于登录状态, 无需验证";

                return responseModel;
            }

            responseModel.Code = 1;
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
                        BotPluginStore.Bot.SubmitSmsCode(captcha);
                        break;
                    case Konata.Core.Events.Model.CaptchaEvent.CaptchaType.Slider:
                        BotPluginStore.Bot.SubmitSliderTicket(captcha);
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
        public async Task<BaseResponseModel> IsOnline()
        {
            BaseResponseModel responseModel = new BaseResponseModel();
            bool isOnline = BotPluginStore.Bot.IsOnline();
            responseModel.Data = isOnline;

            return await Task.FromResult(responseModel);
        }


        [Route("/plugins/QQBotPlugin/Login")]
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            if (!BotPluginStore.Bot.IsOnline())
            {
                Task<bool> taskLogin = BotPluginStore.Bot.Login();
            }

            string indexFilePath = System.IO.Path.Combine(PluginPathProvider.PluginWwwRootDir(nameof(QQBotPlugin)), "login.html");
            // 1. 定时获取验证信息

            return PhysicalFile(indexFilePath, "text/html");
        }

    }
}
