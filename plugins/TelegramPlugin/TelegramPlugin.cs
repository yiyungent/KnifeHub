using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PluginCore.IPlugins;
using TelegramPlugin.Utils;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using PluginCore.Interfaces;

namespace TelegramPlugin
{
    public class TelegramPlugin : BasePlugin, ITimeJobPlugin
    {

        #region Fields

        /// <summary>
        /// 1min
        /// </summary>
        public long SecondsPeriod => 60;

        private readonly IServiceProvider _serviceProvider;

        private readonly bool _debug;

        #endregion

        #region Ctor
        public TelegramPlugin(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        #endregion

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

        public override void AppStart()
        {

        }

        #region 定时任务
        public async Task ExecuteAsync()
        {
            try
            {
                #region 由于内部自动断线重连, 因此断线只有可能是由于程序重启导致需要重新登录上线
                if (TelegramBotStore.Bots == null || TelegramBotStore.Bots.Count <= 0)
                {
                    try
                    {
                        var homeController = new Controllers.HomeController(_serviceProvider);
                        await homeController.Start();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("尝试 自动重新登录 TelegramPlugin 失败:");
                        Console.WriteLine(ex.ToString());
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine($"执行定时任务失败: {ex.ToString()}");
            }

            await Task.CompletedTask;
        }
        #endregion

    }
}
