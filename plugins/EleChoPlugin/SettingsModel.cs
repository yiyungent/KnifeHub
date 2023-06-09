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

            public bool Enable { get; set; }

            public string Mode { get; set; }

            public CqHttpSessionModel CqHttpSession { get; set; }

            public CqRHttpSessionModel CqRHttpSession { get; set; }

            public class CqHttpSessionModel
            {
                public string BaseUri { get; set; }
                public string AccessToken { get; set; }
            }

            public class CqRHttpSessionModel
            {
                public string BaseUri { get; set; }
                public string Secret { get; set; }
            }
        }
    }
}
