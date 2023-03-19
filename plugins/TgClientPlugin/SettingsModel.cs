using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using PluginCore.Models;

namespace TgClientPlugin
{
    public class SettingsModel : PluginSettingsModel
    {
        // 使用 = "" , 确保不被 json 化为 null
        public int ApiId { get; set; } = 0;

        public string ApiHash { get; set; } = "";

        public ProxyModel Proxy { get; set; }

        public AutoLoginModel AutoLogin { get; set; }

        public class ProxyModel
        {
            public bool ProxyEnabled { get; set; }
            public string ProxyHost { get; set; }
            public int ProxyPort { get; set; }
        }

        public class AutoLoginModel
        {
            public bool Enabled { get; set; }
            public string Phone { get; set; }
        }
    }
}
