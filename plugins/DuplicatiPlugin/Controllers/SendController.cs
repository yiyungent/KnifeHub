using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DuplicatiPlugin.RequestModel;
using Telegram.Bot;

namespace DuplicatiPlugin.Controllers
{
    [ApiController]
    [Route("api/Duplicati")]
    public class SendController : ControllerBase
    {
        #region Actions

        [HttpGet, HttpPost]
        [Route("to/TgChatId/{chatId}/apply")]
        public async Task<ActionResult> Telegram([FromRoute] long chatId, [FromBody] DuplicatiRequestModel duplicatiRequestModel)
        {
            // TODO: 使用设置里的 Telegram.BotToken

            return Ok("Ok");
        }

        [HttpGet, HttpPost]
        [Route("TgBotToken/{botToken}/to/TgChatId/{chatId}/apply")]
        public async Task<ActionResult> TelegramBot([FromRoute] string botToken, [FromRoute] long chatId, [FromForm] string message)
        {
            SettingsModel settings = PluginCore.PluginSettingsModelFactory.Create<SettingsModel>(nameof(DuplicatiPlugin));

            #region TODO: 解析数据: Duplicati
            // 目前 --send-http-result-output-format=Json 有问题, 收不到, 于是采用默认格式, 手动获取
            //DuplicatiRequestModel duplicatiRequestModel = new DuplicatiRequestModel();
            //message.Split()
            #endregion

            try
            {
                #region Telegram
                if (settings.Telegram != null && settings.Telegram.Enable)
                {
                    var botClient = new TelegramBotClient(botToken);

                    #region TODO: 解析后发送
                    //string text = $"[{duplicatiRequestModel.BackupName}] {duplicatiRequestModel.MainOperation}";
                    //string badge = "🔴";
                    //switch (duplicatiRequestModel.ParsedResult)
                    //{
                    //    case "Success":
                    //        badge = "✅";
                    //        break;
                    //    default:
                    //        badge = "🔴";
                    //        break;
                    //}
                    //text += "\n" + badge;
                    //text += $"\n WarningsActualLength: {duplicatiRequestModel.WarningsActualLength}";
                    //text += $"\n ErrorsActualLength: {duplicatiRequestModel.ErrorsActualLength}";

                    //await botClient.SendTextMessageAsync(
                    //    chatId: chatId,
                    //    text: text); 
                    #endregion

                    // 发送原生格式
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: message);
                }
                #endregion
            }
            catch (Exception ex)
            {
                Utils.LogUtil.Exception(ex);

                return Ok("fail");
            }

            return Ok("Ok");
        }

        #endregion


    }
}
