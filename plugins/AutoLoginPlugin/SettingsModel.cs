using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using PluginCore.Models;

namespace AutoLoginPlugin
{
    public class SettingsModel : PluginSettingsModel
    {
        /// <summary>
        /// AdminQQ 在任何情况都拥有权限
        /// </summary>
        public string AdminQQ { get; set; }

    }
}
