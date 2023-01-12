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

        void ReceivedAtMessage(string botAppId, Message message, QQChannelApi qChannelApi);

        void ReceivedDirectMessage(string botAppId, Message message, QQChannelApi qChannelApi);

        void ReceivedUserMessage(string botAppId, Message message, QQChannelApi qChannelApi);
    }
}
