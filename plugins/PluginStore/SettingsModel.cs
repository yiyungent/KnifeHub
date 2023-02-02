using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using PluginCore.Models;

namespace PluginStore
{
    public class SettingsModel : PluginSettingsModel
    {
        public long SecondsPeriod { get; set; }

        public int LocalBackupsMaxNum { get; set; }

        public TelegramModel Telegram { get; set; }

        public class TelegramModel
        {
            public bool Enable { get; set; }

            public string Token { get; set; }

            public string ChatId { get; set; }
        }

    }
}
