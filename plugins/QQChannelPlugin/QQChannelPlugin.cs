using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PluginCore.IPlugins;
using QQChannelPlugin.Utils;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace QQChannelPlugin
{
    public class QQChannelPlugin : BasePlugin
    {
        public override (bool IsSuccess, string Message) AfterEnable()
        {
            Console.WriteLine($"{nameof(QQChannelPlugin)}: {nameof(AfterEnable)}");
            return base.AfterEnable();
        }

        public override (bool IsSuccess, string Message) BeforeDisable()
        {
            Console.WriteLine($"{nameof(QQChannelPlugin)}: {nameof(BeforeDisable)}");
            return base.BeforeDisable();
        }

    }
}
