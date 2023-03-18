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
        public string ApiId { get; set; } = "";

        public string ApiHash { get; set; } = "";

        public string PhoneNumber { get; set; } = "";

        public ProxyModel Proxy { get; set; }

        public class ProxyModel
        {
            public bool ProxyEnabled { get; set; }

            public string ProxyHost { get; set; }
            public string ProxyPort { get; set; }
        }
    }
}
