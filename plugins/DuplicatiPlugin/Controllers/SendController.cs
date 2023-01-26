using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DuplicatiPlugin.RequestModel;
using Telegram.Bot;
using DuplicatiPlugin.Models;
using System.Text.Json;

namespace DuplicatiPlugin.Controllers
{
    [ApiController]
    [Route("api/Duplicati")]
    public class SendController : ControllerBase
    {
        #region Actions

        [HttpGet, HttpPost]
        [Route("{key}/apply")]
        public async Task<ActionResult> Apply([FromRoute] string key, [FromForm] string message)
        {
            SettingsModel settings = PluginCore.PluginSettingsModelFactory.Create<SettingsModel>(nameof(DuplicatiPlugin));

            #region 数据预处理
            // 切割 message
            string splitLineStr = "<--DuplicatiPlugin-->";
            int splitLineFirstIndex = message.IndexOf(splitLineStr);
            string jsonStr = message.Substring(startIndex: 0, length: splitLineFirstIndex).Trim();
            string duplicatiStr = message.Substring(startIndex: splitLineFirstIndex + splitLineStr.Length).Trim();
            if (settings.UseDebugModel && settings.Telegram != null && settings.Telegram.Enable)
            {
                try
                {
                    var botClient = new TelegramBotClient(settings.Telegram.BotToken);
                    await botClient.SendTextMessageAsync(
                           chatId: settings.Telegram.ChatId,
                           text: $"message:\r\n" + message);
                    await botClient.SendTextMessageAsync(
                            chatId: settings.Telegram.ChatId,
                            text: $"jsonStr:\r\n" + jsonStr);
                    await botClient.SendTextMessageAsync(
                           chatId: settings.Telegram.ChatId,
                           text: $"duplicatiStr:\r\n" + duplicatiStr);
                }
                catch (Exception ex)
                {
                    Utils.LogUtil.Exception(ex);
                }

            }

            // 解析 message
            DuplicatiMessageModel jsonModel = JsonSerializer.Deserialize<DuplicatiMessageModel>(jsonStr);
            // TODO: 解析 duplicatiStr 
            #endregion

            // 使用设置里的 Telegram
            #region Telegram 通知
            try
            {
                #region Telegram
                if (settings.Telegram != null && settings.Telegram.Enable)
                {
                    var botClient = new TelegramBotClient(settings.Telegram.BotToken);
                    string badge = "🔴";
                    switch (jsonModel.ParsedResult)
                    {
                        case "Success":
                            badge = "✅";
                            break;
                        default:
                            badge = "🔴";
                            break;
                    }
                    string temp = $"Duplicati: {jsonModel.OperationName} \r\n"
                                  + $"{jsonModel.BackupName} \r\n"
                                  + $"{badge}";
                    // 发送
                    await botClient.SendTextMessageAsync(
                        chatId: settings.Telegram.ChatId,
                        text: temp);
                }
                #endregion
            }
            catch (Exception ex)
            {
                Utils.LogUtil.Exception(ex);

                return Ok("fail");
            } 
            #endregion

            #region 保存数据库, key 做来源区分

            #endregion

            return Ok("Ok");
        }

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
