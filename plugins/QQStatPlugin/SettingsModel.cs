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

    }
}
