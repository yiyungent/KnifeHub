using Konata.Core;
using PluginCore.IPlugins;

namespace PluginCore.IPlugins
{
    public interface IQQBotPlugin : IPlugin
    {
        void OnGroupMessage((Bot s, Konata.Core.Events.Model.GroupMessageEvent e) obj, string message, string groupName, uint groupUin, uint memberUin);

        void OnFriendMessage((Bot s, Konata.Core.Events.Model.FriendMessageEvent e) obj, string message, uint friendUin);

        void OnBotOnline((Bot s, Konata.Core.Events.Model.BotOnlineEvent e) obj, string botName, uint botUin);

        void OnBotOffline((Bot s, Konata.Core.Events.Model.BotOfflineEvent e) obj, string botName, uint botUin);
    }
}
