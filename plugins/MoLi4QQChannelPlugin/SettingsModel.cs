using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using PluginCore.Models;

namespace MoLi4QQChannelPlugin
{
    public class SettingsModel : PluginSettingsModel
    {
        public string ApiKey { get; set; }

        public string ApiSecret { get; set; }

        public bool AtEnable { get; set; }

        public string Prefix { get; set; }
    }
}
