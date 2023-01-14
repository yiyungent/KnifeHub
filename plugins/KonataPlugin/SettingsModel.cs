using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Konata.Core.Common;
using PluginCore.Models;

namespace KonataPlugin
{
    public class SettingsModel : PluginSettingsModel
    {
        // 使用 = "" , 确保不被 json 化为 null
        public string Uin { get; set; } = "";

        public string Password { get; set; } = "";

        public bool UseDemoModel { get; set; } = true;

        public string AdminQQ { get; set; } = "";

        public BotKeyStore BotKeyStore { get; set; }

    }
}
