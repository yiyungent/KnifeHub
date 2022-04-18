using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using PluginCore.Models;

namespace QQStatPlugin
{
    public class SettingsModel : PluginSettingsModel
    {
        /// <summary>
        /// AdminQQ 在任何情况都拥有权限
        /// </summary>
        public string AdminQQ { get; set; }

        /// <summary>
        /// 这些群 必须是 管理员/群主 才可以 使用 图表
        /// </summary>
        public List<string> AdminGroups { get; set; }


        /// <summary>
        /// 这些群 普通成员 也可以使用 图表
        /// </summary>
        public List<string> ChartGroups { get; set; }


        public string BaseUrl { get; set; }

        public string ScreenshotUrl { get; set; }

    }
}
