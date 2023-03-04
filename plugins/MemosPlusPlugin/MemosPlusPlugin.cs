using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PluginCore.IPlugins;
using Konata.Core;
using Konata.Core.Events.Model;
using Konata.Core.Interfaces.Api;
using MemosPlusPlugin.Utils;
using System.Text;
using Octokit;
using System.Linq;
using Konata.Core.Message.Model;
using Konata.Core.Message;
using System.Collections.Generic;
using System.IO;

namespace MemosPlusPlugin
{
    public class MemosPlusPlugin : BasePlugin
    {
        public override (bool IsSuccess, string Message) AfterEnable()
        {
            Console.WriteLine($"{nameof(MemosPlusPlugin)}: {nameof(AfterEnable)}");
            return base.AfterEnable();
        }

        public override (bool IsSuccess, string Message) BeforeDisable()
        {
            Console.WriteLine($"{nameof(MemosPlusPlugin)}: {nameof(BeforeDisable)}");
            return base.BeforeDisable();
        }

    }
}
