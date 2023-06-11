using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PluginCore.IPlugins;
using PluginStore.Utils;
using System.Text;
using System.Collections.Generic;
using PluginCore;
using System.IO;
using System.Linq;

namespace PluginStore
{
    public class PluginStore : BasePlugin
    {
        public override (bool IsSuccess, string Message) AfterEnable()
        {
            Console.WriteLine($"{nameof(PluginStore)}: {nameof(AfterEnable)}");
            return base.AfterEnable();
        }

        public override (bool IsSuccess, string Message) BeforeDisable()
        {
            Console.WriteLine($"{nameof(PluginStore)}: {nameof(BeforeDisable)}");
            return base.BeforeDisable();
        }

    }
}
