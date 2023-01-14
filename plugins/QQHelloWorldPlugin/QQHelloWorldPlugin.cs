using System;
using System.Threading.Tasks;
using QQHelloWorldPlugin.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PluginCore.IPlugins;
using Konata.Core;
using Konata.Core.Events.Model;
using Konata.Core.Interfaces.Api;

namespace QQHelloWorldPlugin
{
    public class QQHelloWorldPlugin : BasePlugin, IStartupXPlugin, IWidgetPlugin, IQQBotPlugin
    {
        public override (bool IsSuccess, string Message) AfterEnable()
        {
            Console.WriteLine($"{nameof(QQHelloWorldPlugin)}: {nameof(AfterEnable)}");
            return base.AfterEnable();
        }

        public override (bool IsSuccess, string Message) BeforeDisable()
        {
            Console.WriteLine($"{nameof(QQHelloWorldPlugin)}: {nameof(BeforeDisable)}");
            return base.BeforeDisable();
        }

        public void ConfigureServices(IServiceCollection services)
        {

        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<SayHelloMiddleware>();
        }

        public int ConfigureOrder
        {
            get
            {
                return 2;
            }
        }


        public int ConfigureServicesOrder
        {
            get
            {
                return 2;
            }
        }

        public async Task<string> Widget(string widgetKey, params string[] extraPars)
        {
            string rtnStr = null;
            if (widgetKey == "PluginCore.Admin.Footer")
            {
                if (extraPars != null)
                {
                    Console.WriteLine(string.Join(",", extraPars));
                }
                rtnStr = @"<div style=""border:1px solid green;width:300px;"">
                                <h3>QQHelloWorldPlugin 注入</h3>
                                <div>QQHelloWorldPlugin 挂件</div>
                           </div>";

            }

            return await Task.FromResult(rtnStr);
        }



        #region QQBot
        public void OnGroupMessage((Bot s, GroupMessageEvent e) obj, string message, string groupName, uint groupUin, uint memberUin)
        {
            //obj.s.SendGroupMessage(groupUin, $"复读: {message}");
        }

        public void OnFriendMessage((Bot s, FriendMessageEvent e) obj, string message, uint friendUin)
        {
            obj.s.SendFriendMessage(friendUin, $"复读: {message}");
        }

        public void OnBotOnline((Bot s, BotOnlineEvent e) obj, string botName, uint botUin)
        {
            SettingsModel settingsModel = PluginCore.PluginSettingsModelFactory.Create<SettingsModel>(nameof(QQHelloWorldPlugin));

            if (settingsModel != null && !string.IsNullOrEmpty(settingsModel.AdminQQ))
            {
                obj.s.SendFriendMessage(Convert.ToUInt32(settingsModel.AdminQQ), $"{obj.s.Name}({obj.s.Uin}) 上线啦");
            }
        }

        public void OnBotOffline((Bot s, BotOfflineEvent e) obj, string botName, uint botUin)
        {
            SettingsModel settingsModel = PluginCore.PluginSettingsModelFactory.Create<SettingsModel>(nameof(QQHelloWorldPlugin));

            if (settingsModel != null && !string.IsNullOrEmpty(settingsModel.AdminQQ))
            {
                obj.s.SendFriendMessage(Convert.ToUInt32(settingsModel.AdminQQ), $"{obj.s.Name}({obj.s.Uin}) 离线啦");
            }
        }
        #endregion


    }
}
