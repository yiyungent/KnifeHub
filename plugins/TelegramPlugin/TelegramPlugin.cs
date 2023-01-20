using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PluginCore.IPlugins;
using TelegramPlugin.Utils;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace TelegramPlugin
{
    public class TelegramPlugin : BasePlugin
    {
        public override (bool IsSuccess, string Message) AfterEnable()
        {
            Console.WriteLine($"{nameof(TelegramPlugin)}: {nameof(AfterEnable)}");
            return base.AfterEnable();
        }

        public override (bool IsSuccess, string Message) BeforeDisable()
        {
            Console.WriteLine($"{nameof(TelegramPlugin)}: {nameof(BeforeDisable)}");
            return base.BeforeDisable();
        }

    }
}
