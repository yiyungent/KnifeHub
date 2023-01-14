using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QQChannelFramework.Api;
using QQChannelFramework.Expansions.Bot;

namespace QQChannelPlugin
{
    public static class QQChannelBotStore
    {
        public static List<BotItemModel> Bots { get; set; }

        static QQChannelBotStore()
        {
            Bots = new List<BotItemModel>();
        }

        public sealed class BotItemModel
        {
            public OpenApiAccessInfo OpenApiAccessInfo { get; set; }

            public QQChannelApi QQChannelApi { get; set; }

            public ChannelBot ChannelBot { get; set; }
        }
    }
}
