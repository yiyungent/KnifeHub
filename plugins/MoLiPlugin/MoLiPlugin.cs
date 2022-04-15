using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PluginCore.IPlugins;
using Konata.Core;
using Konata.Core.Events.Model;
using Konata.Core.Interfaces.Api;
using QQBotHub.Sdk.IPlugins;
using MoLiPlugin.Utils;

namespace MoLiPlugin
{
    public class MoLiPlugin : BasePlugin, IQQBotPlugin
    {
        public override (bool IsSuccess, string Message) AfterEnable()
        {
            Console.WriteLine($"{nameof(MoLiPlugin)}: {nameof(AfterEnable)}");
            return base.AfterEnable();
        }

        public override (bool IsSuccess, string Message) BeforeDisable()
        {
            Console.WriteLine($"{nameof(MoLiPlugin)}: {nameof(BeforeDisable)}");
            return base.BeforeDisable();
        }

        #region QQBot
        public void OnGroupMessage((Bot s, GroupMessageEvent e) obj, string message, string groupName, uint groupUin, uint memberUin)
        {
            SettingsModel settingsModel = PluginCore.PluginSettingsModelFactory.Create<SettingsModel>(nameof(MoLiPlugin));

            Console.WriteLine($"茉莉: 来自: {groupUin}-{memberUin}");
            if (settingsModel.AllowGroup != null && settingsModel.AllowGroup.Count >= 1 && settingsModel.AllowGroup.Contains(groupUin.ToString()))
            {
                Console.WriteLine("茉莉准备回复: ");
                MoLiApiResponseModel resModel = new MoLiApiResponseModel();
                try
                {
                    resModel = Utils.MoLiApiUtil.Reply(new MoLiApiRequestModel
                    {
                        content = message,
                        type = "2",
                        from = memberUin.ToString(),
                        fromName = obj.e.Message.Sender.Name,
                        to = groupUin.ToString(),
                        toName = groupName
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine("茉莉回复出错: ");
                    Console.WriteLine(ex.ToString());
                }

                if (resModel != null && resModel.code == "00000")
                {
                    if (resModel.data != null && resModel.data.Count >= 1)
                    {
                        foreach (var item in resModel.data)
                        {
                            obj.s.SendGroupMessage(groupUin: groupUin, message: item.content);
                        }
                    }
                }
            }
        }

        public void OnFriendMessage((Bot s, FriendMessageEvent e) obj, string message, uint friendUin)
        {
            SettingsModel settingsModel = PluginCore.PluginSettingsModelFactory.Create<SettingsModel>(nameof(MoLiPlugin));

            Console.WriteLine($"茉莉: 来自: {friendUin}");
            if (settingsModel.AllowFriends != null && settingsModel.AllowFriends.Count >= 1 && settingsModel.AllowFriends.Contains(friendUin.ToString()))
            {
                Console.WriteLine("茉莉准备回复: ");
                MoLiApiResponseModel resModel = new MoLiApiResponseModel();
                try
                {
                    resModel = Utils.MoLiApiUtil.Reply(new MoLiApiRequestModel
                    {
                        content = message,
                        type = "1",
                        from = friendUin.ToString(),
                        fromName = obj.e.Message.Sender.Name,
                        to = "",
                        toName = ""
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine("茉莉回复出错: ");
                    Console.WriteLine(ex.ToString());
                }

                if (resModel != null && resModel.code == "00000")
                {
                    if (resModel.data != null && resModel.data.Count >= 1)
                    {
                        foreach (var item in resModel.data)
                        {
                            obj.s.SendFriendMessage(friendUin: friendUin, message: item.content);
                        }
                    }
                }
            }
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
