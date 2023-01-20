using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PluginCore.IPlugins;
using DuplicatiPlugin.Utils;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace DuplicatiPlugin
{
    public class DuplicatiPlugin : BasePlugin
    {
        public override (bool IsSuccess, string Message) AfterEnable()
        {
            Console.WriteLine($"{nameof(DuplicatiPlugin)}: {nameof(AfterEnable)}");
            return base.AfterEnable();
        }

        public override (bool IsSuccess, string Message) BeforeDisable()
        {
            Console.WriteLine($"{nameof(DuplicatiPlugin)}: {nameof(BeforeDisable)}");
            return base.BeforeDisable();
        }

    }
}
