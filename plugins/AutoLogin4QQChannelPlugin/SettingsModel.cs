using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using PluginCore.Models;

namespace AutoLogin4QQChannelPlugin
{
    public class SettingsModel : PluginSettingsModel
    {
        public long SecondsPeriod { get; set; }
    }
}
