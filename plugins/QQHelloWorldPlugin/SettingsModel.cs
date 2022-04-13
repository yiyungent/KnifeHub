using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using PluginCore.Models;

namespace QQHelloWorldPlugin
{
    public class SettingsModel : PluginSettingsModel
    {
        public string Hello { get; set; }

        public string AdminQQ { get; set; }
    }
}
