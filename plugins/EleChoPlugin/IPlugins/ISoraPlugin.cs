using PluginCore.IPlugins;
using Sora.EventArgs.SoraEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginCore.IPlugins
{
    public interface IEleChoPlugin : IPlugin
    {
        void OnGroupMessage(string msgType, GroupMessageEventArgs eventArgs);

        void OnPrivateMessage(string msgType, PrivateMessageEventArgs eventArgs);
    }
}
