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
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace TgBotStatPlugin
{
    public class TgBotStatPlugin : BasePlugin, ITimeJobPlugin
    {
        private static bool _isInit = false;

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

        public override void AppStart()
        {

        }

        public override List<string> AppStartOrderDependPlugins
        {
            get
            {
                return new List<string>(){
                  "TelegramPlugin",
                };
            }
        }

        public long SecondsPeriod => 180;

        public async Task ExecuteAsync()
        {
            try
            {
                #region 初始化
                if (!_isInit)
                {
                    if (TelegramPlugin.TelegramBotStore.Bots != null && TelegramPlugin.TelegramBotStore.Bots.Count >= 1)
                    {
                        foreach (var item in TelegramPlugin.TelegramBotStore.Bots)
                        {
                            if (item.TelegramBotClient != null)
                            {
                                // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
                                ReceiverOptions receiverOptions = new()
                                {
                                    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
                                };

                                item.TelegramBotClient.StartReceiving(
                                    updateHandler: async (ITelegramBotClient botClient, Update update, CancellationToken cancellationToken) =>
                                    {
                                        await HandleUpdateAsync(botClient, update, cancellationToken, "");
                                    },
                                    pollingErrorHandler: async (ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken) =>
                                    {
                                        await HandlePollingErrorAsync(botClient, exception, cancellationToken, "");
                                    },
                                    receiverOptions: receiverOptions
                                );

                            }
                        }
                    }
                    _isInit = true;
                }
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine($"执行定时任务失败: {ex.ToString()}");
            }

            await Task.CompletedTask;
        }

        public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken, string botToken)
        {

        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, string botToken)
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Message is not { } message)
                return;

            var chatId = message.Chat.Id;

            if (message.Chat.Type != Telegram.Bot.Types.Enums.ChatType.Private)
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
                if (messageText.Trim().StartsWith("/export"))
                {
                    long count = await DbContext.CountByGroupId(groupId: chatId.ToString());
                    for (int i = 1; i <= (int)Math.Ceiling((decimal)count / (decimal)50); i++)
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
                }
                #endregion
            }

        }
    }
}
