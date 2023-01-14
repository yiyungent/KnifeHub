using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PluginCore.IPlugins;
using Konata.Core;
using Konata.Core.Events.Model;
using Konata.Core.Interfaces.Api;
using QQNotePlugin.Utils;
using System.Text;
using Octokit;
using System.Linq;
using Konata.Core.Message.Model;
using Konata.Core.Message;
using System.Collections.Generic;
using System.IO;

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

                #region 过滤消息
                if (!message.Trim().StartsWith(flagStr))
                {
                    return;
                }
                #endregion

                string fullMessageText = "";
                Dictionary<string, byte[]> imageDic = new Dictionary<string, byte[]>();

                #region 组织新文件内容
                try
                {
                    StringBuilder fullMessageSb = new StringBuilder();
                    foreach (BaseChain chain in obj.e.Message.Chain)
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
                                #region 笔记中的图片
                                try
                                {
                                    ImageChain imageChain = (ImageChain)chain;
                                    byte[] imageBytes = null;
                                    if (imageChain.FileData != null && imageChain.FileData.Length > 0)
                                    {
                                        imageBytes = imageChain.FileData;
                                    }
                                    else if (!string.IsNullOrEmpty(imageChain.ImageUrl))
                                    {
                                        imageBytes = Utils.HttpUtil.HttpDownloadFile(imageChain.ImageUrl);
                                    }
                                    if (imageBytes == null || imageBytes.Length <= 0)
                                    {
                                        continue;
                                    }
                                    //  jpeg和jpg没什么区别，二者是一样的，jpg是jpeg的简称
                                    string imageType = "jpeg";
                                    switch (imageChain.ImageType)
                                    {
                                        case ImageType.Invalid:
                                            break;
                                        case ImageType.Face:
                                            break;
                                        case ImageType.Jpg:
                                            imageType = "jpeg";
                                            break;
                                        case ImageType.Png:
                                            imageType = "png";
                                            break;
                                        case ImageType.Webp:
                                            imageType = "webp";
                                            break;
                                        case ImageType.Pjpeg:
                                            imageType = "jpeg";
                                            break;
                                        case ImageType.Sharpp:
                                            break;
                                        case ImageType.Bmp:
                                            imageType = "bmp";
                                            break;
                                        case ImageType.Gif:
                                            imageType = "gif";
                                            break;
                                        case ImageType.Apng:
                                            imageType = "apng";
                                            break;
                                        default:
                                            imageType = "jpeg";
                                            break;
                                    }

                                    #region 图片 base64 形式
                                    // 由于 GitHub 不支持直接显示 base64 图片, 因此改为上传图片文件
                                    //string imageBase64 = Convert.ToBase64String(imageBytes);
                                    //string imgBase64Html = $"<img src=\"data:image/{imageType};base64,{imageBase64}\" />";
                                    //fullMessageSb.AppendLine(imgBase64Html); 
                                    #endregion

                                    #region 图片文件形式
                                    string dirName = Path.GetFileNameWithoutExtension(settingsModel.GitHub.RepoTargetFilePath);
                                    // 注意: 图片文件名不能有空格, 否则会导致在 GitHub 无法预览
                                    string imageFileName = $"image-{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}-{imageBytes.GetHashCode()}.{imageType}";
                                    //string imageHtml = $"<img src=\"{dirName}/{imageFileName}\" />";
                                    // Markdown 图片标记 容易不显示
                                    string imageMd = $"![{Path.GetFileNameWithoutExtension(imageFileName)}]({dirName}/{imageFileName})";
                                    imageDic.Add($"{dirName}/{imageFileName}", imageBytes);
                                    //fullMessageSb.AppendLine(imageHtml);
                                    fullMessageSb.AppendLine(imageMd);
                                    #endregion
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.ToString());
                                    obj.s.SendFriendMessage(friendUin, $"笔记中的图片获取失败:");
                                    obj.s.SendFriendMessage(friendUin, $"{ex.ToString()}");
                                }
                                #endregion
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

                    fullMessageText = fullMessageText.Trim().Substring(flagStr.Length).Trim();

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

                #region 写入笔记
                try
                {
                    #region 上传笔记中的图片
                    foreach (var item in imageDic)
                    {
                        try
                        {
                            string imageFilePath = Path.Combine(Path.GetDirectoryName(targetFilePath), item.Key);
                            var createImageSet = gitHubClient.Repository.Content.CreateFile(owner, repo, imageFilePath,
                               new CreateFileRequest(message: $"{nameof(QQNotePlugin)} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}",
                               content: Convert.ToBase64String(item.Value), branch: branch, convertContentToBase64: false))
                                .Result;
                        }
                        catch (Exception ex)
                        {
                            obj.s.SendFriendMessage(friendUin, $"{item.Key} 笔记中的图片写入失败:");
                            obj.s.SendFriendMessage(friendUin, ex.ToString());
                        }
                    }
                    #endregion

                    #region 写入笔记文件
                    // try to get the file (and with the file the last commit sha)
                    var existingFile = gitHubClient.Repository.Content.GetAllContentsByRef(owner, repo, targetFilePath, branch).Result;
                    string oldFileContent = existingFile.First().Content;

                    // 新文件内容: 在旧的 后面换行 +
                    fullMessageText = $"  \n{fullMessageText}  \n";
                    string newFileContent = $"{oldFileContent}  \n---  \n{fullMessageText}";

                    // update the file
                    var updateChangeSet = gitHubClient.Repository.Content.UpdateFile(owner, repo, targetFilePath,
                       new UpdateFileRequest(message: $"{nameof(QQNotePlugin)} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}",
                       content: newFileContent, sha: existingFile.First().Sha, branch: branch))
                        .Result;
                    #endregion

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

                #endregion

            }
        }

        public void OnBotOnline((Bot s, BotOnlineEvent e) obj, string botName, uint botUin)
        {

        }

        public void OnBotOffline((Bot s, BotOfflineEvent e) obj, string botName, uint botUin)
        {

        }
        #endregion


    }
}
