using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PluginCore.IPlugins;
using MemosPlusPlugin.Utils;
using System.Text;
using Octokit;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace MemosPlusPlugin
{
    public class MemosPlusPlugin : BasePlugin, IWidgetPlugin, IStartupPlugin
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

        public async Task<string> Widget(string widgetKey, params string[] extraPars)
        {
            string rtnStr = null;
            if (widgetKey == "memos")
            {
                if (extraPars != null)
                {
                    Console.WriteLine(string.Join(",", extraPars));
                }
                rtnStr = @"<script>
                           console.log(""测试"");
                           </script>";

            }

            return await Task.FromResult(rtnStr);
        }

        public void Configure(IApplicationBuilder app)
        {
            SettingsModel settingsModel = PluginCore.PluginSettingsModelFactory.Create<SettingsModel>(nameof(MemosPlusPlugin));
            
        }

        public void ConfigureServices(IServiceCollection services)
        {
            SettingsModel settingsModel = PluginCore.PluginSettingsModelFactory.Create<SettingsModel>(nameof(MemosPlusPlugin));
            
        }

        public int ConfigureServicesOrder => 0;

        public int ConfigureOrder => 0;

    }
}
