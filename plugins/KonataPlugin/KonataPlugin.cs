using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PluginCore.IPlugins;
using KonataPlugin.Utils;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace KonataPlugin
{
    public class KonataPlugin : BasePlugin
    {
        public override (bool IsSuccess, string Message) AfterEnable()
        {
            Console.WriteLine($"{nameof(KonataPlugin)}: {nameof(AfterEnable)}");
            return base.AfterEnable();
        }

        public override (bool IsSuccess, string Message) BeforeDisable()
        {
            Console.WriteLine($"{nameof(KonataPlugin)}: {nameof(BeforeDisable)}");
            return base.BeforeDisable();
        }

    }
}
