using Maila.Cocoa.Beans.Models.Messages;
using Maila.Cocoa.Framework;
using Maila.Cocoa.Framework.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CocoaPlugin
{
    public static class CocoaBotStore
    {
        public static BotItemModel Bot { get; set; }

        static CocoaBotStore()
        {
            Bot = new BotItemModel();
            //BotAPI.SendFriendMessage(13232, new PlainMessage("message"));
        }

        public sealed class BotItemModel
        {
            public BotStartupConfig BotStartupConfig { get; set; }
        }
    }
}
