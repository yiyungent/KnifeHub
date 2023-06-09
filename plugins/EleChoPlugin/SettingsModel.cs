using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using PluginCore.Models;

namespace EleChoPlugin
{
    public class SettingsModel : PluginSettingsModel
    {
        public long AdminQQ { get; set; }

        /// <summary>
        /// 使用 演示 模式, 方便测试/体验
        /// </summary>
        public bool UseDemoModel { get; set; } = true;

        public List<EleChoConfigItemModel> EleChoConfigs { get; set; }

        public class EleChoConfigItemModel
        {
            /// <summary>
            /// 用于唯一标识此配置, 将被用于路径前缀 /EleChoPlugin-{ConfigId}
            /// </summary>
            /// <value></value>
            public string ConfigId { get; set; }

            /// <summary>
            /// 反向 WebSocket: ws
            /// 反向 HTTP: http
            /// </summary>
            /// <value></value>
            public string SessionMode { get; set; }

            public string CqWsSession { get; set; }

            public class CqWsSessionModel
            {
                public string AccessToken { get; set; }
                public bool UseGroupMessage { get; set; }
            }
        }
    }
}
