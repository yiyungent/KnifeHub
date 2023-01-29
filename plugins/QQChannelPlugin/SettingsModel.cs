using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using PluginCore.Models;

namespace QQChannelPlugin
{
    public class SettingsModel : PluginSettingsModel
    {
        public List<BotDevItemModel> Bots { get; set; }

        public sealed class BotDevItemModel
        {
            /// <summary>
            /// 平台 BotAppId
            /// </summary>
            public string BotAppId { get; set; }

            /// <summary>
            /// 平台 BotToken
            /// </summary>
            public string BotToken { get; set; }

            /// <summary>
            /// 平台 BotSecret
            /// </summary>
            public string BotSecret { get; set; }

            /// <summary>
            /// 指定Api通道模式为沙盒模式 (测试时使用)  
            /// 不指定的情况下默认是正式模式
            /// </summary>
            public bool UseSandBoxMode { get; set; }

            /// <summary>
            /// 指定为私域机器人
            /// https://www.yuque.com/chianne1025/mybot/rexa9h
            /// 在想要使用一些私域机器人功能时，需要通过以下方法指定机器人为私域，否则无法正常使用。
            /// 例如： 无需 @机器人 可收到频道内用户消息
            /// </summary>
            public bool UsePrivateBot { get; set; }

            /// <summary>
            /// 启用无须@ 触发指令功能 (私域机器人可用)
            /// https://www.yuque.com/chianne1025/mybot/rexa9h
            /// 启用后，频道内触发机器人指令 无需 @机器人
            /// </summary>
            public bool EnableUserMessageTriggerCommand { get; set; }

            /// <summary>
            /// 使用 演示 模式, 方便测试/体验
            /// </summary>
            public bool UseDemoModel { get; set; }
        }

    }
}
