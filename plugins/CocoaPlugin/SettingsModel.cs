using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using PluginCore.Models;

namespace CocoaPlugin
{
    public class SettingsModel : PluginSettingsModel
    {
        public string VerifyKey { get; set; }

        public long BotQQ { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }

        public long AdminQQ { get; set; }

        /// <summary>
        /// 使用 演示 模式, 方便测试/体验
        /// </summary>
        public bool UseDemoModel { get; set; } = true;
    }
}
