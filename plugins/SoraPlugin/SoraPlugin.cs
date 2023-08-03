using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PluginCore.IPlugins;
using SoraPlugin.Utils;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using PluginCore.Interfaces;

namespace SoraPlugin
{
    public class SoraPlugin : BasePlugin
    {
        #region Fields

        private readonly IPluginFinder _pluginFinder;

        private readonly bool _debug;

        #endregion

        #region Ctor
        public SoraPlugin(IPluginFinder pluginFinder)
        {
            _pluginFinder = pluginFinder;
        }
        #endregion

        public override (bool IsSuccess, string Message) AfterEnable()
        {
            Console.WriteLine($"{nameof(SoraPlugin)}: {nameof(AfterEnable)}");
            return base.AfterEnable();
        }

        public override (bool IsSuccess, string Message) BeforeDisable()
        {
            Console.WriteLine($"{nameof(SoraPlugin)}: {nameof(BeforeDisable)}");
            return base.BeforeDisable();
        }

        public override void AppStart()
        {
            try
            {
                var homeController = new Controllers.HomeController(_pluginFinder);
                homeController.Start();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
            }
        }

    }
}
