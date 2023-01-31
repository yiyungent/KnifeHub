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

        public async void ReceivedAtMessage(string botAppId, Message message, QQChannelApi qChannelApi)
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
            if (!string.IsNullOrEmpty(settingsModel.Prefix) && text.Trim().StartsWith(settingsModel.Prefix))
            {
                // 移除前缀, 不考虑空格前缀
                text = text.Trim().Substring(settingsModel.Prefix.Length);
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
                Console.WriteLine("茉莉API回复出错: ");
                Console.WriteLine(ex.ToString());
            }

            if (resModel != null && resModel.code == "00000")
            {
                if (resModel.data != null && resModel.data.Count >= 1)
                {
                    foreach (var item in resModel.data)
                    {
                        try
                        {
                            if (item.typed == "1") {
                                await qChannelApi.GetMessageApi().SendTextMessageAsync(channelId: message.ChannelId, content: item.content, passiveReference: message.Id);
                            } else if (item.typed == "2") {
                                string imageUrl = "https://files.molicloud.com/" + item.content;
                                await qChannelApi.GetMessageApi().SendImageMessageAsync(channelId: message.ChannelId, imageUrl: imageUrl, passiveReference: message.Id);
                            } else {
                                await qChannelApi.GetMessageApi().SendTextMessageAsync(channelId: message.ChannelId, content: item.content, passiveReference: message.Id);
                            }
                        }
                        catch (System.Exception ex)
                        {
                            System.Console.WriteLine($"{botAppId} 回复出错:");
                            System.Console.WriteLine(ex.ToString());
                        }
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
        public async void ReceivedUserMessage(string botAppId, Message message, QQChannelApi qChannelApi)
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
            if (!string.IsNullOrEmpty(settingsModel.Prefix) && text.Trim().StartsWith(settingsModel.Prefix))
            {
                // 移除前缀, 不考虑空格前缀
                text = text.Trim().Substring(settingsModel.Prefix.Length);
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
                Console.WriteLine("茉莉API回复出错: ");
                Console.WriteLine(ex.ToString());
            }

            if (resModel != null && resModel.code == "00000")
            {
                if (resModel.data != null && resModel.data.Count >= 1)
                {
                    foreach (var item in resModel.data)
                    {
                        try
                        {
                            if (item.typed == "1") {
                                await qChannelApi.GetMessageApi().SendTextMessageAsync(channelId: message.ChannelId, content: item.content, passiveReference: message.Id);
                            } else if (item.typed == "2") {
                                string imageUrl = "https://files.molicloud.com/" + item.content;
                                await qChannelApi.GetMessageApi().SendImageMessageAsync(channelId: message.ChannelId, imageUrl: imageUrl, passiveReference: message.Id);
                            } else {
                                await qChannelApi.GetMessageApi().SendTextMessageAsync(channelId: message.ChannelId, content: item.content, passiveReference: message.Id);
                            }
                        }
                        catch (System.Exception ex)
                        {
                            System.Console.WriteLine($"{botAppId} 回复出错:");
                            System.Console.WriteLine(ex.ToString());
                        }
                    }
                }
            }
        }
        #endregion


    }
}
