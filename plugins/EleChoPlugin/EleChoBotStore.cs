using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Message;

namespace EleChoPlugin
{
    public static class EleChoBotStore
    {
        public static List<BotItemModel> Bots { get; set; }

        static EleChoBotStore()
        {
            Bots = new List<BotItemModel>();
        }

        public sealed class BotItemModel
        {
            /// <summary>
            /// 唯一标识, 用于与设置中一一对应上
            /// </summary>
            public string ConfigId { get; set; }

            public string Mode { get; set; }

            public CqHttpSession CqHttpSession { get; set; }

            public CqRHttpSession CqRHttpSession { get; set; }

            public CqWsSession CqWsSession { get; set; }

            public CqRWsSession CqRWsSession { get; set; }
        }
    }
}
