using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using PluginCore.Models;

namespace DuplicatiPlugin
{
    public class SettingsModel : PluginSettingsModel
    {
        public bool UseDebugModel { get; set; }

        public TelegramModel Telegram { get; set; }

        public sealed class TelegramModel
        {
            /// <summary>
            /// 机器人 ACCESS_TOKEN
            /// </summary>
            public string BotToken { get; set; }

            public string ChatId { get; set; }

            public bool Enable { get; set; }
        }

    }
}
