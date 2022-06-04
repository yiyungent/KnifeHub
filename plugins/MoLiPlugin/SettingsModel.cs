using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using PluginCore.Models;

namespace MoLiPlugin
{
    public class SettingsModel : PluginSettingsModel
    {
        public string ApiKey { get; set; }

        public string ApiSecret { get; set; }

        public bool AtEnable { get; set; }

        public string Prefix { get; set; }

        public List<string> AllowGroup { get; set; }

        public List<string> AllowFriends { get; set; }
    }
}
