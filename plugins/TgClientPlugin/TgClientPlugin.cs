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
    public class TgClientPlugin : BasePlugin, ITimeJobPlugin
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

        public long SecondsPeriod => 60;
        public async Task ExecuteAsync()
        {
            var settings = Utils.SettingsUtil.Get(nameof(TgClientPlugin));
            if (settings.AutoLogin == null || !settings.AutoLogin.Enabled)
            {
                await Task.CompletedTask;
                return;
            }
            // TODO: 暂时没用到 IPluginFinder, 直接传递 null
            Controllers.HomeController homeController = new Controllers.HomeController(null);
            try
            {
                if (Controllers.HomeController.Client == null || Controllers.HomeController.Client.User == null)
                {
                    await homeController.Login(new RequestModels.LoginRequestModel
                    {
                        LoginInfo = settings.AutoLogin.Phone
                    });
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"{nameof(TgClientPlugin)}.{nameof(ExecuteAsync)}");
                System.Console.WriteLine(ex.ToString());
            }

            await Task.CompletedTask;
        }
    }
}
