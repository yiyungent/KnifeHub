using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PluginCore.IPlugins;
using CocoaPlugin.Utils;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace CocoaPlugin
{
    public class CocoaPlugin : BasePlugin
    {
        public override (bool IsSuccess, string Message) AfterEnable()
        {
            Console.WriteLine($"{nameof(CocoaPlugin)}: {nameof(AfterEnable)}");
            return base.AfterEnable();
        }

        public override (bool IsSuccess, string Message) BeforeDisable()
        {
            Console.WriteLine($"{nameof(CocoaPlugin)}: {nameof(BeforeDisable)}");
            return base.BeforeDisable();
        }

    }
}
