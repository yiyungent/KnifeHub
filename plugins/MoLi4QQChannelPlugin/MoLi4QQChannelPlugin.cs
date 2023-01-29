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
using System.Text.RegularExpressions;

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

            string text = message.Content;

            #region 去除 @
            try
            {
                // 在吗 <@!13214513123523> 你还好吗  <@!131245451434>
                // 去除 @ 字符
                //string text = message.Content.Replace($"<@!{qChannelApi.GetUserApi().GetCurrentUserAsync().Result.Id}>", "");
                Regex atRegex = new Regex(@"<@\![0-9]*>");
                //var atMatches = atRegex.Matches(message.Content);
                text = atRegex.Replace(message.Content, "");
            }
            catch (Exception ex)
            { } 
            #endregion

            // 排除启用前缀且不满足前缀
            if (!string.IsNullOrEmpty(settingsModel.Prefix) && !text.Trim().StartsWith(settingsModel.Prefix))
            {
                return;
            }

            Console.WriteLine("茉莉准备回复: ");
            MoLiApiResponseModel resModel = new MoLiApiResponseModel();
            try
            {

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

        /// <summary>
        /// 无 @
        /// </summary>
        /// <param name="botAppId"></param>
        /// <param name="message"></param>
        /// <param name="qChannelApi"></param>
        public void ReceivedUserMessage(string botAppId, Message message, QQChannelApi qChannelApi)
        {
            SettingsModel settingsModel = PluginCore.PluginSettingsModelFactory.Create<SettingsModel>(nameof(MoLi4QQChannelPlugin));
            Console.WriteLine($"茉莉: 来自: {message.ChannelId}-{message.Author.UserName}");

            string text = message.Content;

            #region 去除 @
            try
            {
                // 在吗 <@!13214513123523> 你还好吗  <@!131245451434>
                // 去除 @ 字符
                //string text = message.Content.Replace($"<@!{qChannelApi.GetUserApi().GetCurrentUserAsync().Result.Id}>", "");
                Regex atRegex = new Regex(@"<@\![0-9]*>");
                //var atMatches = atRegex.Matches(message.Content);
                text = atRegex.Replace(message.Content, "");
            }
            catch (Exception ex)
            { }
            #endregion

            // 排除 at
            if (settingsModel.AtEnable)
            {
                return;
            }
            // 排除启用前缀且不满足前缀
            if (!string.IsNullOrEmpty(settingsModel.Prefix) && !text.Trim().StartsWith(settingsModel.Prefix))
            {
                return;
            }

            Console.WriteLine("茉莉准备回复: ");
            MoLiApiResponseModel resModel = new MoLiApiResponseModel();
            try
            {
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
