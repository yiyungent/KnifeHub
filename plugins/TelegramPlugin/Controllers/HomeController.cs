using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PluginCore;
using PluginCore.Interfaces;
using PluginCore.IPlugins;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramPlugin.Utils;

namespace TelegramPlugin.Controllers
{
    /// <summary>
    /// 其实也可以不写这个, 直接访问 Plugins/ZhiDaoPlugin/index.html
    /// 
    /// 下面的方法, 是去掉 index.html
    /// 
    /// 若 wwwroot 下有其它需要访问的文件, 如何 css, js, 而你又不想每次新增 action 指定返回, 则 Route 必须 Plugins/{PluginId},
    /// 这样访问 Plugins/HelloWorldPlugin/css/main.css 就会访问到你插件下的 wwwroot/css/main.css
    /// </summary>
    [Route($"Plugins/{nameof(TelegramPlugin)}")]
    public class HomeController : Controller
    {
        #region Fields

        private readonly IPluginFinder _pluginFinder;

        private readonly bool _debug;

        private readonly IServiceProvider _serviceProvider;

        private readonly ILogger<HomeController> _logger;

        #endregion

        #region Ctor
        public HomeController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _pluginFinder = serviceProvider.GetService<IPluginFinder>();
            _logger = serviceProvider.GetService<ILogger<HomeController>>();

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
            string indexFilePath = System.IO.Path.Combine(PluginPathProvider.PluginWwwRootDir(nameof(TelegramPlugin)), "index.html");

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
            SettingsModel settingsModel = PluginCore.PluginSettingsModelFactory.Create<SettingsModel>(nameof(TelegramPlugin));
            // 确保以前的都取消
            #region 确保以前的都取消
            foreach (var item in TelegramBotStore.Bots)
            {
                try
                {
                    if (item.CancellationTokenSource != null)
                    {
                        try
                        {
                            // Send cancellation request to stop bot
                            item.CancellationTokenSource.Cancel();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                            _logger.LogError(ex, $"{nameof(TelegramPlugin)}.Controllers.HomeController.Start()");
                        }
                        try
                        {
                            await item.TelegramBotClient.LogOutAsync();
                        }
                        catch (System.Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                            _logger.LogError(ex, $"{nameof(TelegramPlugin)}.Controllers.HomeController.Start()");
                        }
                        try
                        {
                            await item.TelegramBotClient.CloseAsync();
                        }
                        catch (System.Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                            _logger.LogError(ex, $"{nameof(TelegramPlugin)}.Controllers.HomeController.Start()");
                        }
                    }
                    item.TelegramBotClient = null;
                    item.CancellationTokenSource = null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            TelegramBotStore.Bots.Clear();
            #endregion

            foreach (var botConfig in settingsModel.Bots)
            {
                if (botConfig.Enable)
                {
                    try
                    {
                        TelegramBotItem(botConfig, settingsModel);
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        _logger.LogError(ex, $"{nameof(TelegramPlugin)}.Controllers.HomeController.Start()");
                    }
                }
            }

            // TODO: 暂时这么做, 以后优化界面
            return Content("尝试启动设置里的 TelegramBot 中, 请耐性等待!注意查看控制台!出现本页面可能已经启动成功, 刷新即重新启动");
        }

        [Route(nameof(Download))]
        [Authorize("PluginCore.Admin")]
        public async Task<ActionResult> Download()
        {
            string dbFilePath = DbContext.DbFilePath;
            var fileStream = System.IO.File.OpenRead(dbFilePath);
            //System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();

            return File(fileStream: fileStream, contentType: "application/x-sqlite3", fileDownloadName: $"{nameof(TelegramPlugin)}.sqlite", enableRangeProcessing: true);
        }
        #endregion

        #region NonActions
        [NonAction]
        private async void TelegramBotItem(SettingsModel.BotDevItemModel botConfig, SettingsModel settings)
        {
            // https://telegrambots.github.io/book/1/example-bot.html

            var botClient = new TelegramBotClient(botConfig.BotToken);
            using CancellationTokenSource cts = new();

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
            };

            // 保存起来
            TelegramBotStore.Bots.Add(new TelegramBotStore.BotItemModel()
            {
                CancellationTokenSource = cts,
                TelegramBotClient = botClient
            });

            try
            {
                // TODO: 可能放在这里上线不合适
                // 完成以上配置后将机器人上线
                botClient.StartReceiving(
                    updateHandler: async (ITelegramBotClient botClient, Update update, CancellationToken cancellationToken) =>
                    {
                        await HandleUpdateAsync(botClient, update, cancellationToken, botConfig.BotToken);
                    },
                    pollingErrorHandler: async (ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken) =>
                    {
                        await HandlePollingErrorAsync(botClient, exception, cancellationToken, botConfig.BotToken);
                    },
                    receiverOptions: receiverOptions,
                    cancellationToken: cts.Token
                );

                var me = await botClient.GetMeAsync();

                Console.WriteLine($"Start listening for @{me.Username}");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
                _logger.LogError(ex, $"{nameof(TelegramPlugin)}.Controllers.HomeController.TelegramBotItem()");
            }

        }

        [NonAction]
        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, string botToken)
        {
            // 每次处理都获取对应Bot的最新设置, 免除每次重新启动 TgBot
            SettingsModel settingsModel = PluginCore.PluginSettingsModelFactory.Create<SettingsModel>(nameof(TelegramPlugin));
            var botConfig = settingsModel.Bots.FirstOrDefault(m => m.BotToken == botToken);
            if (botConfig == null || !botConfig.Enable)
            {
                return;
            }

            #region 插件事件派发
            Utils.LogUtil.Info($"{botToken} 机器人收到消息");

            var plugins = _pluginFinder.EnablePlugins<ITelegramBotPlugin>().ToList();
            Utils.LogUtil.Info($"响应: {plugins?.Count.ToString()} 个插件:");
            foreach (var plugin in plugins)
            {
                Utils.LogUtil.Info($"插件: {plugin.GetType().ToString()}");

                try
                {
                    plugin.HandleUpdateAsync(botClient, update, cancellationToken, botToken);
                }
                catch (Exception ex)
                {
                    LogUtil.Exception(ex);
                    _logger.LogError(ex, $"{nameof(TelegramPlugin)}.Controllers.HomeController.HandleUpdateAsync()");
                }
            }
            #endregion

            if (botConfig.UseDemoModel)
            {
                // Only process Message updates: https://core.telegram.org/bots/api#message
                if (update.Message is not { } message)
                    return;
                // Only process text messages
                if (message.Text is not { } messageText)
                    return;

                var chatId = message.Chat.Id;

                Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

                // Echo received message text
                Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "You said:\n" + messageText,
                    cancellationToken: cancellationToken);

                //await botClient.SetMyCommandsAsync(commands: new BotCommand[] {
                //    new BotCommand(){
                //        Command = "help",
                //        Description = ""
                //    }
                //});
                //await botClient.SetChatMenuButtonAsync(menuButton: new MenuButtonDefault());
            }
        }

        [NonAction]
        Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken, string botToken)
        {
            // 每次处理都获取对应Bot的最新设置, 免除每次重新启动 TgBot
            SettingsModel settingsModel = PluginCore.PluginSettingsModelFactory.Create<SettingsModel>(nameof(TelegramPlugin));
            var botConfig = settingsModel.Bots.FirstOrDefault(m => m.BotToken == botToken);
            if (botConfig == null || !botConfig.Enable)
            {
                return Task.CompletedTask;
            }

            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"[{botToken}]: Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);

            #region 插件事件派发
            var plugins = _pluginFinder.EnablePlugins<ITelegramBotPlugin>().ToList();
            Utils.LogUtil.Info($"响应: {plugins?.Count.ToString()} 个插件:");
            foreach (var plugin in plugins)
            {
                Utils.LogUtil.Info($"插件: {plugin.GetType().ToString()}");

                try
                {
                    plugin.HandlePollingErrorAsync(botClient, exception, cancellationToken, botToken);
                }
                catch (Exception ex)
                {
                    LogUtil.Exception(ex);
                    _logger.LogError(ex, $"{nameof(TelegramPlugin)}.Controllers.HomeController.HandlePollingErrorAsync()");
                }
            }
            #endregion

            return Task.CompletedTask;
        }
        #endregion
    }
}
