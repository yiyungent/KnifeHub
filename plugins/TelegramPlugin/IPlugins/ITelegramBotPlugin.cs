using PluginCore.IPlugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace PluginCore.IPlugins
{
    public interface ITelegramBotPlugin : IPlugin
    {
        /// <summary>
        /// 注意: 默认你的 TgBot 在 Group 中只能收到 /command@YourBotName 指令消息, 
        /// 若需要接收所有消息, 需要关闭 privacy mode, 然后重新邀请 你的 TgBot 到 Group
        /// /setprivacy — Set which messages your bot will receive when added to a group. With privacy mode disabled, the bot will receive all messages.
        /// 参考:   
        /// https://stackoverflow.com/questions/44751015/receive-messages-from-groups-or-channels-using-telegram-bot-api
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="update"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="botToken"></param>
        void HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, string botToken);

        void HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken, string botToken);
    }
}
