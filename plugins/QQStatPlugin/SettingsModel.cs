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
        /// 这些群 必须是 管理员/群主 才可以 使用 图表
        /// </summary>
        public List<string> AdminGroups { get; set; }


        /// <summary>
        /// 这些群 群普通成员也可以使用 图表
        /// </summary>
        public List<string> ChartGroups { get; set; }

        public string BaseUrl { get; set; }

        public string ScreenshotUrl { get; set; }

    }
}
