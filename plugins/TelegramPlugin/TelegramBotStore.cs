using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;

namespace TelegramPlugin
{
    public static class TelegramBotStore
    {
        public static List<BotItemModel> Bots { get; set; }

        static TelegramBotStore()
        {
            Bots = new List<BotItemModel>();
        }

        public sealed class BotItemModel
        {
            public TelegramBotClient TelegramBotClient { get; set; }

            public CancellationTokenSource CancellationTokenSource { get; set; }
        }
    }
}
