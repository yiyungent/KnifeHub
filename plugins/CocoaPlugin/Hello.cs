using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Maila.Cocoa.Framework;

namespace CocoaPlugin
{
    [BotModule]
    public class Hello : BotModuleBase
    {
        [TextRoute("hello cocoa")] // 收到 “hello cocoa”时调用此方法
        public static void Run(MessageSource src)
        {
            SettingsModel settingsModel = PluginCore.PluginSettingsModelFactory.Create<SettingsModel>(nameof(CocoaPlugin));

            if (settingsModel.UseDemoModel)
            {
                src.Send($"Hi I'm {nameof(CocoaPlugin)}"); // 向消息来源发送“Hi!”
            }
        }
    }
}
