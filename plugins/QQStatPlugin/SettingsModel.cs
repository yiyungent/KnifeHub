using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using PluginCore.Models;

namespace QQStatPlugin
{
    public class SettingsModel : PluginSettingsModel
    {
        public string AdminQQ { get; set; }

        public List<string> Groups { get; set; }

        /// <summary>
        /// 这些群的管理员拥有权限
        /// </summary>
        public List<string> AdminGroups { get; set; }

        public string BaseUrl { get; set; }

        public string ScreenshotUrl { get; set; }

    }
}
