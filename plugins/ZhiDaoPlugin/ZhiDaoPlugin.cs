using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PluginCore.IPlugins;
using Konata.Core;
using Konata.Core.Events.Model;
using Konata.Core.Interfaces.Api;
using ZhiDaoPlugin.Utils;
using Konata.Core.Message.Model;
using Konata.Core.Common;
using System.Text;
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

                if (message.StartsWith("#问"))
                {
                    if (!IsAdmin(obj.s, adminQQ: settingsModel.AdminQQ, groupUin: groupUin, memberUin: memberUin))
                    {
                        return;
                    }

                    #region 自定义问答
                    int answerIndex = text.IndexOf("#答");
                    string questionStr = text.Substring(0, answerIndex).Trim().Replace("#问", "").Trim();
                    string answerStr = text.Substring(answerIndex + 1 + 1).Trim();

                    try
                    {
                        var dbModel = DbContext.QueryAllQABox().FirstOrDefault(m => m.QQGroup == groupUin.ToString()
                                                                                    && m.Question.Trim().ToLower() == questionStr.ToLower());
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
                                Answer = answerStr,
                                QQGroup = groupUin.ToString()
                            };
                            DbContext.InsertIntoQABox(dbModel);
                        }

                        obj.s.SendGroupMessage(groupUin: groupUin, "学习成功");
                    }
                    catch (Exception ex)
                    {
                        obj.s.SendGroupMessage(groupUin: groupUin, ex.ToString());
                    }
                    #endregion
                }
                else if (message.StartsWith("#删除问答"))
                {
                    if (!IsAdmin(obj.s, adminQQ: settingsModel.AdminQQ, groupUin: groupUin, memberUin: memberUin))
                    {
                        return;
                    }

                    #region 删除问答
                    text = text.Replace("#删除问答", "").Trim().ToLower();
                    var dbModel = DbContext.QueryAllQABox().FirstOrDefault(m => m.QQGroup == groupUin.ToString()
                                                                                && m.Question.Trim().ToLower() == text);
                    if (dbModel != null)
                    {
                        DbContext.DeleteQABox(dbModel);
                        obj.s.SendGroupMessage(groupUin: groupUin, "删除问答成功");
                    }
                    else
                    {
                        obj.s.SendGroupMessage(groupUin: groupUin, "不存在此问答");
                    }
                    #endregion
                }
                else if (message.StartsWith("#已学习"))
                {
                    if (!IsAdmin(obj.s, adminQQ: settingsModel.AdminQQ, groupUin: groupUin, memberUin: memberUin))
                    {
                        return;
                    }

                    #region 已学习
                    var dbModelList = DbContext.QueryAllQABox().Where(m => m.QQGroup == groupUin.ToString()).ToList();
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("已学习:");
                    foreach (var dbModel in dbModelList)
                    {
                        sb.AppendLine(dbModel.Question);
                    }
                    obj.s.SendGroupMessage(groupUin: groupUin, sb.ToString());
                    #endregion
                }
                else
                {
                    #region 关键词回复
                    try
                    {
                        var boxList = DbContext.QueryAllQABox().Where(m => m.QQGroup == groupUin.ToString()).ToList();
                        foreach (var item in boxList)
                        {
                            if (text.Contains(item.Question.ToLower().Trim()))
                            {
                                obj.s.SendGroupMessage(groupUin: groupUin,
                                    ReplyChain.Create(obj.e.Message),
                                    AtChain.Create(obj.e.MemberUin),
                                    TextChain.Create(item.Answer));
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

        }

        public void OnFriendMessage((Bot s, FriendMessageEvent e) obj, string message, uint friendUin)
        {

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


        private bool IsAdmin(Bot bot, string adminQQ, uint groupUin, uint memberUin)
        {
            var memeberInfo = bot.GetGroupMemberInfo(groupUin: groupUin, memberUin: memberUin, forceUpdate: true).Result;
            bool isAdmin = adminQQ == memberUin.ToString() || memeberInfo.Role == RoleType.Owner || memeberInfo.Role == RoleType.Admin;

            return isAdmin;
        }

    }
}
