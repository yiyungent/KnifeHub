using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using PluginCore.Models;

namespace SoraPlugin
{
    public class SettingsModel : PluginSettingsModel
    {
        /// <summary>
        /// server
        /// client
        /// </summary>
        public string Mode { get; set; }

        public ServerConfigModel ServerConfig { get; set; }

        public ClientConfigModel ClientConfig { get; set; }

        public long AdminQQ { get; set; }

        /// <summary>
        /// 使用 演示 模式, 方便测试/体验
        /// </summary>
        public bool UseDemoModel { get; set; } = true;

        public sealed class ClientConfigModel
        {
            public ushort Port { get; set; }
            public string AccessToken { get; set; }
            public bool AutoMarkMessageRead { get; set; }
            public long[] BlockUsers { get; set; }
            public bool EnableSocketMessage { get; set; }
            public bool EnableSoraCommandManager { get; set; }
            public string Host { get; set; }
            public bool SendCommandErrMsg { get; set; }
            public long[] SuperUsers { get; set; }
            public bool ThrowCommandException { get; set; }
            public string UniversalPath { get; set; }
        }

        public sealed class ServerConfigModel
        {
            public ushort Port { get; set; }
            public string AccessToken { get; set; }
            public bool AutoMarkMessageRead { get; set; }
            public long[] BlockUsers { get; set; }
            public bool EnableSocketMessage { get; set; }
            public bool EnableSoraCommandManager { get; set; }
            public string Host { get; set; }
            public bool SendCommandErrMsg { get; set; }
            public long[] SuperUsers { get; set; }
            public bool ThrowCommandException { get; set; }
            public string UniversalPath { get; set; }
        }
    }
}
