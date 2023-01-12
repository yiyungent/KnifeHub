using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PluginCore.IPlugins;
using MoLi4QQChannelPlugin.Utils;
using System.Text;
using QQChannelPlugin.IPlugins;
using QQChannelFramework.Models.MessageModels;
using QQChannelFramework.Api;

namespace MoLi4QQChannelPlugin
{
    public class MoLi4QQChannelPlugin : BasePlugin, IQQChannelPlugin
    {
        public override (bool IsSuccess, string Message) AfterEnable()
        {
            Console.WriteLine($"{nameof(MoLi4QQChannelPlugin)}: {nameof(AfterEnable)}");
            return base.AfterEnable();
        }

        public override (bool IsSuccess, string Message) BeforeDisable()
        {
            Console.WriteLine($"{nameof(MoLi4QQChannelPlugin)}: {nameof(BeforeDisable)}");
            return base.BeforeDisable();
        }

        #region QQChannelBot

        public void OnConnected(string botAppId)
        {

        }

        public void AuthenticationSuccess(string botAppId)
        {

        }

        public void OnError(string botAppId, Exception ex)
        {

        }

        public void ReceivedAtMessage(string botAppId, Message message, QQChannelApi qChannelApi)
        {
            SettingsModel settingsModel = PluginCore.PluginSettingsModelFactory.Create<SettingsModel>(nameof(MoLi4QQChannelPlugin));
            Console.WriteLine($"茉莉: 来自: {message.ChannelId}-{message.Author.UserName}");

            Console.WriteLine("茉莉准备回复: ");
            MoLiApiResponseModel resModel = new MoLiApiResponseModel();
            try
            {
                // 去除 @机器人 字符
                string text = message.Content.Replace($"<@!{qChannelApi.GetUserApi().GetCurrentUserAsync().Result.Id}>", "");
                resModel = Utils.MoLiApiUtil.Reply(new MoLiApiRequestModel
                {
                    content = text,
                    type = "2",
                    from = message.Author.Id,
                    fromName = message.Author.UserName,
                    to = message.ChannelId,
                    toName = message.ChannelId
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
                        qChannelApi.GetMessageApi().SendTextMessageAsync(channelId: message.ChannelId, content: item.content, passiveReference: message.Id);
                    }
                }
            }
        }

        public void ReceivedDirectMessage(string botAppId, Message message, QQChannelApi qChannelApi)
        {
            // 私信
        }

        public void ReceivedUserMessage(string botAppId, Message message, QQChannelApi qChannelApi)
        {
            SettingsModel settingsModel = PluginCore.PluginSettingsModelFactory.Create<SettingsModel>(nameof(MoLi4QQChannelPlugin));
            Console.WriteLine($"茉莉: 来自: {message.ChannelId}-{message.Author.UserName}");

            // 排除 at
            if (settingsModel.AtEnable)
            {
                return;
            }
            // 排除启用前缀且不满足前缀
            if (!string.IsNullOrEmpty(settingsModel.Prefix) && !message.Content.Trim().StartsWith(settingsModel.Prefix))
            {
                return;
            }

            Console.WriteLine("茉莉准备回复: ");
            MoLiApiResponseModel resModel = new MoLiApiResponseModel();
            try
            {
                string text = message.Content;
                resModel = Utils.MoLiApiUtil.Reply(new MoLiApiRequestModel
                {
                    content = text,
                    type = "2",
                    from = message.Author.Id,
                    fromName = message.Author.UserName,
                    to = message.ChannelId,
                    toName = message.ChannelId
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
                        qChannelApi.GetMessageApi().SendTextMessageAsync(channelId: message.ChannelId, content: item.content, passiveReference: message.Id);
                    }
                }
            }
        }
        #endregion


    }
}
