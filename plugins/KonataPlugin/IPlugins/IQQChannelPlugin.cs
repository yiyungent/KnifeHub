using PluginCore.IPlugins;
using QQChannelFramework.Api;
using QQChannelFramework.Models.MessageModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQChannelPlugin.IPlugins
{
    public interface IQQChannelPlugin : IPlugin
    {
        void OnConnected(string botAppId);

        void AuthenticationSuccess(string botAppId);

        void OnError(string botAppId, Exception ex);

        /// <summary>
        /// 注意: message.Content 中包含 <@!1094789961239705377> 其中 1094789961239705377 为@的人
        /// </summary>
        /// <param name="botAppId"></param>
        /// <param name="message"></param>
        /// <param name="qChannelApi"></param>
        void ReceivedAtMessage(string botAppId, Message message, QQChannelApi qChannelApi);

        void ReceivedDirectMessage(string botAppId, Message message, QQChannelApi qChannelApi);

        void ReceivedUserMessage(string botAppId, Message message, QQChannelApi qChannelApi);
    }
}
