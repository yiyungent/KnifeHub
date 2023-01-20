using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;

namespace DuplicatiPlugin
{
    public static class TelegramBotStore
    {
        public static BotItemModel Bot { get; set; }

        static TelegramBotStore()
        {
            Bot = new BotItemModel();
        }

        public sealed class BotItemModel
        {
            public TelegramBotClient TelegramBotClient { get; set; }

            public CancellationTokenSource CancellationTokenSource { get; set; }
        }
    }
}
