using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PluginCore.IPlugins;
using EleChoPlugin.Utils;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace EleChoPlugin
{
    public class EleChoPlugin : BasePlugin
    {
        public override (bool IsSuccess, string Message) AfterEnable()
        {
            Console.WriteLine($"{nameof(EleChoPlugin)}: {nameof(AfterEnable)}");
            return base.AfterEnable();
        }

        public override (bool IsSuccess, string Message) BeforeDisable()
        {
            Console.WriteLine($"{nameof(EleChoPlugin)}: {nameof(BeforeDisable)}");
            return base.BeforeDisable();
        }

        public override void AppStart()
        {
            base.AppStart();
        }
    }
}
