using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using PluginCore.Models;

namespace TelegramPlugin
{
    public class SettingsModel : PluginSettingsModel
    {
        public List<BotDevItemModel> Bots { get; set; }

        public sealed class BotDevItemModel
        {
            /// <summary>
            /// 机器人 ACCESS_TOKEN
            /// </summary>
            public string BotToken { get; set; }

            /// <summary>
            /// 机器人的超级管理员 的 ChatId 
            /// </summary>
            public string AdminChatId { get; set; }

            /// <summary>
            /// 使用 演示 模式, 方便测试/体验
            /// </summary>
            public bool UseDemoModel { get; set; }

            public bool Enable { get; set; }
        }

    }
}
