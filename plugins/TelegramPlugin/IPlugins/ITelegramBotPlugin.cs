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
        void HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, string botToken);

        void HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken, string botToken);
    }
}
