using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PluginCore.IPlugins;
using Konata.Core;
using Konata.Core.Events.Model;
using Konata.Core.Interfaces.Api;
using QQBotHub.Sdk.IPlugins;
using ZhiDaoPlugin.Utils;
using Konata.Core.Message.Model;
using Konata.Core.Common;
using System.Text;
using QQBotHub.Sdk;
using Konata.Core.Message;
using System.Collections.Generic;
using System.Linq;

namespace ZhiDaoPlugin
{
    public class ZhiDaoPlugin : BasePlugin, IQQBotPlugin
    {
        public override (bool IsSuccess, string Message) AfterEnable()
        {
            Console.WriteLine($"{nameof(ZhiDaoPlugin)}: {nameof(AfterEnable)}");
            return base.AfterEnable();
        }

        public override (bool IsSuccess, string Message) BeforeDisable()
        {
            Console.WriteLine($"{nameof(ZhiDaoPlugin)}: {nameof(BeforeDisable)}");
            return base.BeforeDisable();
        }

        #region QQBot
        public void OnGroupMessage((Bot s, GroupMessageEvent e) obj, string message, string groupName, uint groupUin, uint memberUin)
        {
            SettingsModel settingsModel = PluginCore.PluginSettingsModelFactory.Create<SettingsModel>(nameof(ZhiDaoPlugin));

            if (settingsModel.Groups != null && settingsModel.Groups.Length > 0 && settingsModel.Groups.Contains(groupUin.ToString()))
            {
                string text = ConvertToString(obj.e.Message.Chain).ToLower();
                #region 关键词
                try
                {
                    var boxList = DbContext.QueryAllQABox();
                    foreach (var item in boxList)
                    {
                        if (text.Contains(item.Question.ToLower().Trim()))
                        {
                            obj.s.SendGroupMessage(groupUin: groupUin, AtChain.Create(obj.e.MemberUin), TextChain.Create(item.Answer));
                        }
                    }
                }
                catch (Exception ex)
                {
                    obj.s.SendGroupMessage(groupUin: groupUin, ex.ToString());
                }
                #endregion
            }


        }

        public void OnFriendMessage((Bot s, FriendMessageEvent e) obj, string message, uint friendUin)
        {
            SettingsModel settingsModel = PluginCore.PluginSettingsModelFactory.Create<SettingsModel>(nameof(ZhiDaoPlugin));
            if (settingsModel.AdminQQ != null && settingsModel.AdminQQ == friendUin.ToString())
            {
                #region 学习
                if (message.StartsWith("#问"))
                {
                    string text = ConvertToString(obj.e.Message.Chain);
                    int answerIndex = text.IndexOf("#答");
                    string questionStr = text.Substring(0, answerIndex).Trim().Replace("#问", "").Trim();
                    string answerStr = text.Substring(answerIndex + 1 + 1).Trim();

                    try
                    {
                        var dbModel = DbContext.QueryAllQABox().FirstOrDefault(m => m.Question.Trim().ToLower() == questionStr.ToLower());
                        if (dbModel != null)
                        {
                            dbModel.Answer = answerStr;
                            dbModel.UpdateTime = DateTime.Now.ToTimeStamp13();
                            DbContext.UpdateQABox(dbModel);
                        }
                        else
                        {
                            dbModel = new Models.QABox()
                            {
                                CreateTime = DateTime.Now.ToTimeStamp13(),
                                UpdateTime = DateTime.Now.ToTimeStamp13(),
                                Question = questionStr,
                                Answer = answerStr
                            };
                            DbContext.InsertIntoQABox(dbModel);
                        }

                        obj.s.SendFriendMessage(friendUin: friendUin, "学习成功");
                    }
                    catch (Exception ex)
                    {
                        obj.s.SendFriendMessage(friendUin: friendUin, ex.ToString());
                    }
                }
                #endregion


                #region 删除问答
                if (message.StartsWith("#删除问答"))
                {
                    string text = ConvertToString(obj.e.Message.Chain).Replace("#删除问答", "").Trim().ToLower();
                    var dbModel = DbContext.QueryAllQABox().FirstOrDefault(m => m.Question.Trim().ToLower() == text);
                    if (dbModel != null)
                    {
                        DbContext.DeleteQABox(dbModel);
                        obj.s.SendFriendMessage(friendUin: friendUin, "删除问答成功");
                    }
                    else
                    {
                        obj.s.SendFriendMessage(friendUin: friendUin, "不存在此问答");
                    }

                }
                #endregion

                #region 已学习
                if (message.StartsWith("#已学习"))
                {
                    var dbModelList = DbContext.QueryAllQABox();
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("已学习:");
                    foreach (var dbModel in dbModelList)
                    {
                        sb.AppendLine(dbModel.Question);
                    }
                    obj.s.SendFriendMessage(friendUin: friendUin, sb.ToString());
                }
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

        private string ConvertToString(MessageChain chains)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in chains)
            {
                switch (item.Type)
                {
                    case BaseChain.ChainType.At:
                        break;
                    case BaseChain.ChainType.Reply:
                        break;
                    case BaseChain.ChainType.Text:
                        sb.AppendLine(item.ToString());
                        break;
                    case BaseChain.ChainType.Image:
                        break;
                    case BaseChain.ChainType.Flash:
                        break;
                    case BaseChain.ChainType.Record:
                        break;
                    case BaseChain.ChainType.Video:
                        break;
                    case BaseChain.ChainType.QFace:
                        break;
                    case BaseChain.ChainType.BFace:
                        break;
                    case BaseChain.ChainType.Xml:
                        break;
                    case BaseChain.ChainType.MultiMsg:
                        break;
                    case BaseChain.ChainType.Json:
                        break;
                    default:
                        break;
                }
            }

            return sb.ToString();
        }

    }
}
