using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PluginCore.IPlugins;
using Konata.Core;
using Konata.Core.Events.Model;
using Konata.Core.Interfaces.Api;
using QQBotHub.Sdk.IPlugins;
using QQStatPlugin.Utils;

namespace QQStatPlugin
{
    public class QQStatPlugin : BasePlugin, IQQBotPlugin
    {
        public override (bool IsSuccess, string Message) AfterEnable()
        {
            Console.WriteLine($"{nameof(QQStatPlugin)}: {nameof(AfterEnable)}");
            return base.AfterEnable();
        }

        public override (bool IsSuccess, string Message) BeforeDisable()
        {
            Console.WriteLine($"{nameof(QQStatPlugin)}: {nameof(BeforeDisable)}");
            return base.BeforeDisable();
        }

        #region QQBot
        public void OnGroupMessage((Bot s, GroupMessageEvent e) obj, string message, string groupName, uint groupUin, uint memberUin)
        {
            //obj.s.SendGroupMessage(groupUin, $"复读: {message}");

            // 保存数据库
            int successRow = DbContext.Instance.Insertable(new Models.Message()
            {
                Content = message,
                CreateTime = DateTime.Now.ToTimeStamp13(),
                GroupName = groupName,
                GroupUin = groupUin.ToString(),
                QQName = obj.e.Message.Sender.Name,
                QQUin = memberUin.ToString()
            }).ExecuteCommand();
            Console.WriteLine($"成功插入 {successRow} 行");
        }

        public void OnFriendMessage((Bot s, FriendMessageEvent e) obj, string message, uint friendUin)
        {
            //obj.s.SendFriendMessage(friendUin, $"复读: {message}");

            // 保存数据库
            int successRow = DbContext.Instance.Insertable(new Models.Message()
            {
                Content = message,
                CreateTime = DateTime.Now.ToTimeStamp13(),
                GroupName = null,
                GroupUin = null,
                QQName = obj.e.Message.Sender.Name,
                QQUin = friendUin.ToString()
            }).ExecuteCommand();
            Console.WriteLine($"成功插入 {successRow} 行");
        }

        public void OnBotOnline((Bot s, BotOnlineEvent e) obj, string botName, uint botUin)
        {
            //SettingsModel settingsModel = PluginCore.PluginSettingsModelFactory.Create<SettingsModel>(nameof(QQStatPlugin));

            //if (settingsModel != null && !string.IsNullOrEmpty(settingsModel.AdminQQ))
            //{
            //    obj.s.SendFriendMessage(Convert.ToUInt32(settingsModel.AdminQQ), $"{obj.s.Name}({obj.s.Uin}) 上线啦");
            //}
        }

        public void OnBotOffline((Bot s, BotOfflineEvent e) obj, string botName, uint botUin)
        {
            //SettingsModel settingsModel = PluginCore.PluginSettingsModelFactory.Create<SettingsModel>(nameof(QQStatPlugin));

            //if (settingsModel != null && !string.IsNullOrEmpty(settingsModel.AdminQQ))
            //{
            //    obj.s.SendFriendMessage(Convert.ToUInt32(settingsModel.AdminQQ), $"{obj.s.Name}({obj.s.Uin}) 离线啦");
            //}
        }
        #endregion


    }
}
