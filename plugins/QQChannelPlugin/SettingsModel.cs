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
            /// 使用 演示 模式, 方便测试/体验
            /// </summary>
            public bool UseDemoModel { get; set; }
        }

    }
}
