using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PluginCore.IPlugins;
using Konata.Core;
using Konata.Core.Events.Model;
using Konata.Core.Interfaces.Api;
using QQBotHub.Sdk.IPlugins;
using QQNotePlugin.Utils;
using System.Text;
using Octokit;
using System.Linq;

namespace QQNotePlugin
{
    public class QQNotePlugin : BasePlugin, IQQBotPlugin
    {
        public override (bool IsSuccess, string Message) AfterEnable()
        {
            Console.WriteLine($"{nameof(QQNotePlugin)}: {nameof(AfterEnable)}");
            return base.AfterEnable();
        }

        public override (bool IsSuccess, string Message) BeforeDisable()
        {
            Console.WriteLine($"{nameof(QQNotePlugin)}: {nameof(BeforeDisable)}");
            return base.BeforeDisable();
        }

        #region QQBot
        public void OnGroupMessage((Bot s, GroupMessageEvent e) obj, string message, string groupName, uint groupUin, uint memberUin)
        {
            //SettingsModel settingsModel = PluginCore.PluginSettingsModelFactory.Create<SettingsModel>(nameof(QQNotePlugin));

        }

        public void OnFriendMessage((Bot s, FriendMessageEvent e) obj, string message, uint friendUin)
        {
            SettingsModel settingsModel = PluginCore.PluginSettingsModelFactory.Create<SettingsModel>(nameof(QQNotePlugin));

            Console.WriteLine($"{nameof(QQNotePlugin)}: 来自: {friendUin}");
            if (settingsModel.AllowFriends != null && settingsModel.AllowFriends.Count >= 1 && settingsModel.AllowFriends.Contains(friendUin.ToString()))
            {
                Console.WriteLine($"{nameof(QQNotePlugin)}: {message}");
                string flagStr = "note";
                if (!message.Trim().StartsWith(flagStr))
                {
                    return;
                }

                string fullMessageText = "";

                #region 组织新文件内容
                try
                {
                    StringBuilder fullMessageSb = new StringBuilder();
                    foreach (var chain in obj.e.Message.Chain)
                    {
                        switch (chain.Type)
                        {
                            case Konata.Core.Message.BaseChain.ChainType.At:
                                break;
                            case Konata.Core.Message.BaseChain.ChainType.Reply:
                                break;
                            case Konata.Core.Message.BaseChain.ChainType.Text:
                                fullMessageSb.AppendLine(chain.ToString());
                                break;
                            case Konata.Core.Message.BaseChain.ChainType.Image:
                                break;
                            case Konata.Core.Message.BaseChain.ChainType.Flash:
                                break;
                            case Konata.Core.Message.BaseChain.ChainType.Record:
                                break;
                            case Konata.Core.Message.BaseChain.ChainType.Video:
                                break;
                            case Konata.Core.Message.BaseChain.ChainType.QFace:
                                break;
                            case Konata.Core.Message.BaseChain.ChainType.BFace:
                                break;
                            case Konata.Core.Message.BaseChain.ChainType.Xml:
                                break;
                            case Konata.Core.Message.BaseChain.ChainType.MultiMsg:
                                break;
                            case Konata.Core.Message.BaseChain.ChainType.Json:
                                break;
                            default:
                                break;
                        }
                    }
                    fullMessageText = fullMessageSb.ToString();

                    if (string.IsNullOrEmpty(fullMessageText))
                    {
                        obj.s.SendFriendMessage(friendUin, $"{nameof(fullMessageText)} 为空");
                        return;
                    }

                    fullMessageText = fullMessageText.Substring(flagStr.Length).Trim();

                    Console.WriteLine($"{nameof(QQNotePlugin)}.fullMessageText: {fullMessageText}");
                }
                catch (Exception ex)
                {
                    obj.s.SendFriendMessage(friendUin, "笔记内容组织失败");
                    obj.s.SendFriendMessage(friendUin, ex.ToString());
                    return;
                }
                #endregion

                #region GitHub
                GitHubClient gitHubClient = new GitHubClient(new ProductHeaderValue(nameof(QQNotePlugin)));
                gitHubClient.Credentials = new Credentials(settingsModel.GitHub.AccessToken);

                // github variables
                string owner = settingsModel.GitHub.RepoOwner;
                string repo = settingsModel.GitHub.RepoName;
                string branch = settingsModel.GitHub.RepoBranch;
                string targetFilePath = settingsModel.GitHub.RepoTargetFilePath;

                try
                {
                    // try to get the file (and with the file the last commit sha)
                    var existingFile = gitHubClient.Repository.Content.GetAllContentsByRef(owner, repo, targetFilePath, branch).Result;
                    string oldFileContent = existingFile.First().Content;

                    // 新文件内容: 在旧的 后面换行 +
                    string newFileContent = $"{oldFileContent}\n{fullMessageText}";

                    // update the file
                    var updateChangeSet = gitHubClient.Repository.Content.UpdateFile(owner, repo, targetFilePath,
                       new UpdateFileRequest(message: $"{nameof(QQNotePlugin)}-{DateTime.Now.ToString("yyyy-MM-dd HH-mm:ss")}",
                       content: newFileContent, sha: existingFile.First().Sha, branch: branch))
                        .Result;

                    obj.s.SendFriendMessage(friendUin, "笔记写入成功");
                }
                //catch (Octokit.NotFoundException)
                catch (Exception ex)
                {
                    // if file is not found, create it
                    //var createChangeSet = gitHubClient.Repository.Content.CreateFile(owner, repo, targetFilePath, new CreateFileRequest("API File creation", "Hello Universe! " + DateTime.UtcNow, branch)).Result;

                    obj.s.SendFriendMessage(friendUin, "笔记写入失败");
                }

                #endregion

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
