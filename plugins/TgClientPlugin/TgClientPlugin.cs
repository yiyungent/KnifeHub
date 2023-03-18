using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PluginCore.IPlugins;
using TgClientPlugin.Utils;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace TgClientPlugin
{
    public class TgClientPlugin : BasePlugin
    {
        public override (bool IsSuccess, string Message) AfterEnable()
        {
            Console.WriteLine($"{nameof(TgClientPlugin)}: {nameof(AfterEnable)}");
            return base.AfterEnable();
        }

        public override (bool IsSuccess, string Message) BeforeDisable()
        {
            Console.WriteLine($"{nameof(TgClientPlugin)}: {nameof(BeforeDisable)}");
            return base.BeforeDisable();
        }

    }
}
