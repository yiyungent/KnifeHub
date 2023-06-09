using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PluginCore.IPlugins;
using EleChoPlugin.Utils;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using PluginCore.Interfaces;
using EleChoPlugin.Controllers;

namespace EleChoPlugin
{
    public class EleChoPlugin : BasePlugin, ITimeJobPlugin
    {
        #region Fields
        private readonly IPluginFinder _pluginFinder;
        #endregion

        #region Props
        public long SecondsPeriod => 60;
        #endregion

        #region Ctor
        public EleChoPlugin(IPluginFinder pluginFinder)
        {
            _pluginFinder = pluginFinder;
        }
        #endregion

        #region Methods
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

        public async Task ExecuteAsync()
        {
            if (EleChoBotStore.Bots == null)
            {
                EleChoBotStore.Bots = new List<EleChoBotStore.BotItemModel>();
            }
            // 主程序意外崩溃后, 重启后, 自动重新连接
            if (EleChoBotStore.Bots.Count == 0)
            {
                SettingsModel settings = PluginCore.PluginSettingsModelFactory.Create<SettingsModel>(nameof(EleChoPlugin));
                if (settings.EleChoConfigs != null && settings.EleChoConfigs.Count >= 1)
                {
                    #region Temp
                    //var configs = settings.EleChoConfigs.Where(m => m.Enable).ToList();
                    //foreach (var config in configs)
                    //{
                    //    try
                    //    {
                    //        var homeController = new HomeController(_pluginFinder);
                    //        homeController.BotItem(config, settings);
                    //        Console.WriteLine($"EleChoPlugin: {config.ConfigId}: 尝试 自动重新连接 成功");
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        Console.WriteLine($"EleChoPlugin: {config.ConfigId}: 尝试 自动重新连接 失败:");
                    //        Console.WriteLine(ex.ToString());
                    //    }
                    //} 
                    #endregion

                    var homeController = new HomeController(_pluginFinder);
                    await homeController.Start();
                }
            }
        }
        #endregion
    }
}
