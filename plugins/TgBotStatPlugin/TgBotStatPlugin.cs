using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PluginCore.IPlugins;
using TgBotStatPlugin.Utils;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TgBotStatPlugin
{
    public class TgBotStatPlugin : BasePlugin, ITelegramBotPlugin
    {
        public override (bool IsSuccess, string Message) AfterEnable()
        {
            Console.WriteLine($"{nameof(TgBotStatPlugin)}: {nameof(AfterEnable)}");
            return base.AfterEnable();
        }

        public override (bool IsSuccess, string Message) BeforeDisable()
        {
            Console.WriteLine($"{nameof(TgBotStatPlugin)}: {nameof(BeforeDisable)}");
            return base.BeforeDisable();
        }

        public void HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken, string botToken)
        {

        }

        public async void HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, string botToken)
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Message is not { } message)
                return;

            var chatId = message.Chat.Id;

            if (message.Chat.Type == Telegram.Bot.Types.Enums.ChatType.Group)
            {
                // 只在群聊有效

                #region 收集群聊记录
                string messageText = message.Text ?? "";
                try
                {
                    DbContext.InsertIntoMessage(new Models.Message()
                    {
                        UId = message.From?.Id.ToString() ?? "",
                        UName = message.From?.Username ?? "",
                        GroupId = message.Chat.Id.ToString(),
                        GroupName = message.Chat.Title ?? "",
                        Content = messageText,
                        CreateTime = message.Date.ToTimeStamp13()
                    });
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine(ex.ToString());
                }
                #endregion

                #region /export
                // 导出当前群聊记录 (分片导出: 多个文件)

                long count = await DbContext.CountByGroupId(groupId: chatId.ToString());
                for (int i = 1; i <= Math.Floor((decimal)count / (decimal)50); i++)
                {
                    List<Models.Message> tempList = (await DbContext.QueryByGroupId(groupId: chatId.ToString(), new Pager(page: i, pageSize: 50))).ToList();
                    string tempJsonStr = Utils.JsonUtil.Obj2JsonStr(tempList);

                    try
                    {
                        Stream fileStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(tempJsonStr));
                        Message newMessage = await botClient.SendDocumentAsync(
                            chatId: chatId,
                            document: new Telegram.Bot.Types.InputFiles.InputOnlineFile(content: fileStream, fileName: $"export-{i}.json"),
                            caption: $"export-{i}.json");
                    }
                    catch (System.Exception ex)
                    {
                        System.Console.WriteLine(ex.ToString());
                    }
                }
                #endregion
            }

        }
    }
}
