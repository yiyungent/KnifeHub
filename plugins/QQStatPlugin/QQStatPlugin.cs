using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PluginCore.IPlugins;
using Konata.Core;
using Konata.Core.Events.Model;
using Konata.Core.Interfaces.Api;
using QQStatPlugin.Utils;
using Konata.Core.Message.Model;
using Konata.Core.Common;
using System.Text;
using Konata.Core.Message;
using System.Collections.Generic;
using KonataPlugin;
using System.Linq;

namespace QQStatPlugin
{
    public class QQStatPlugin : BasePlugin, IQQBotPlugin, ITimeJobPlugin
    {
        /// <summary>
        /// 43200 = 12 小时
        /// </summary>
        public long SecondsPeriod => 43200;

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
            SettingsModel settingsModel = PluginCore.PluginSettingsModelFactory.Create<SettingsModel>(nameof(QQStatPlugin));

            #region 收集群消息
            try
            {
                // 保存数据库
                int successRow = DbContext.InsertIntoMessage(new Models.Message()
                {
                    Content = ConvertToString(obj.e.Chain),
                    CreateTime = DateTime.Now.ToTimeStamp13(),
                    GroupName = groupName,
                    GroupUin = groupUin.ToString(),
                    QQName = obj.e.Message.Sender.Name,
                    QQUin = memberUin.ToString()
                });
                Console.WriteLine($"成功插入 {successRow} 行");
            }
            catch (Exception ex)
            {
                Console.WriteLine("插入表 Message 出错:");
                Console.WriteLine(ex.ToString());
            }
            #endregion

            #region 图表
            Console.WriteLine($"{groupName} ({groupUin})");
            bool isChartGroup = settingsModel.ChartGroups != null && settingsModel.ChartGroups.Count >= 1 && settingsModel.ChartGroups.Contains(groupUin.ToString());
            bool isAdminGroup = settingsModel.AdminGroups != null && settingsModel.AdminGroups.Count >= 1 && settingsModel.AdminGroups.Contains(groupUin.ToString());
            Console.WriteLine($"{nameof(isChartGroup)}:{isChartGroup}");
            Console.WriteLine($"{nameof(isAdminGroup)}:{isAdminGroup}");
            if (isAdminGroup || isChartGroup)
            {
                Console.WriteLine($"进入 {DateTime.Now.ToString()}");
                BotMember member = null;
                try
                {
                    member = obj.s.GetGroupMemberInfo(groupUin: groupUin, memberUin: memberUin, forceUpdate: true).Result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("获取群成员信息失败");
                }
                if (member != null)
                {
                    bool isAdmin = member.Uin.ToString() == settingsModel.AdminQQ || member.Role == Konata.Core.Common.RoleType.Owner || member.Role == Konata.Core.Common.RoleType.Admin;
                    if (isAdmin || isChartGroup)
                    {
                        if (message.Contains("#帮助"))
                        {
                            StringBuilder stringBuilder = new StringBuilder();
                            stringBuilder.AppendLine("#日历");
                            stringBuilder.AppendLine("#日历 指定某人QQ");
                            stringBuilder.AppendLine("#折线");
                            stringBuilder.AppendLine("#折线 指定某人QQ");
                            stringBuilder.AppendLine("#排行榜");
                            stringBuilder.AppendLine("补充:");
                            stringBuilder.AppendLine("日历为 计算消息字数");
                            stringBuilder.AppendLine("折线为 计算消息字数");
                            obj.s.SendGroupMessage(groupUin, stringBuilder.ToString());
                        }
                        else if (message.Contains("#日历"))
                        {
                            #region 日历
                            SendCalendar(obj, message, groupUin, memberUin, settingsModel);
                            #endregion
                        }
                        else if (message.Contains("#折线"))
                        {
                            #region 折线
                            try
                            {
                                SendStackedArea(obj: obj, message: message, groupUin: groupUin, settingsModel: settingsModel, memberUin: memberUin);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("SendStackedArea() 失败:");
                                Console.WriteLine(ex.ToString());
                            }
                            #endregion
                        }
                        else if (message.Contains("#排行榜"))
                        {
                            #region 排行榜
                            try
                            {
                                var memeberList = obj.s.GetGroupMemberList(groupUin: groupUin, forceUpdate: true).Result.ToList()
                                    .Select(m => (m.NickName, m.Uin)).ToList();
                                var memeberUinList = memeberList.Select(m => m.Uin).ToList();

                                var topByGroupList = DbContext.TopByGroup(groupUin: groupUin.ToString()).Result.ToList();
                                List<BaseChain> baseChains = new List<BaseChain>();
                                baseChains.Add(TextChain.Create("本群发言排行榜 (总字数)"));
                                baseChains.Add(TextChain.Create("\r\n"));
                                for (int i = 0; i < topByGroupList.Count; i++)
                                {
                                    baseChains.Add(TextChain.Create($"{(i + 1)}: "));
                                    baseChains.Add(TextChain.Create($"总字数: {topByGroupList[i].TotalContentLen}  "));
                                    if (memeberUinList.Contains(uint.Parse(topByGroupList[i].QQUin)))
                                    {
                                        //var memberTemp = memeberList.FirstOrDefault(m => m.Uin.ToString() == topByGroupList[i].QQUin);
                                        // 没有 memberTemp.Name

                                        var memberTemp = obj.s.GetGroupMemberInfo(groupUin: groupUin, memberUin: uint.Parse(topByGroupList[i].QQUin), forceUpdate: true).Result;

                                        //baseChains.Add(AtChain.Create(uint.Parse(topByGroupList[i].QQUin)));
                                        baseChains.Add(TextChain.Create($"{memberTemp.NickName}({topByGroupList[i].QQUin})"));
                                    }
                                    else
                                    {
                                        baseChains.Add(TextChain.Create($"已退群({topByGroupList[i].QQUin})"));
                                    }
                                    baseChains.Add(TextChain.Create("\r\n"));
                                }
                                obj.s.SendGroupMessage(groupUin, baseChains.ToArray());
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("发送 排行榜 失败:");
                                Console.WriteLine(ex.ToString());
                            }
                            #endregion
                        }
                    }
                }
            }
            #endregion

        }




        public void OnFriendMessage((Bot s, FriendMessageEvent e) obj, string message, uint friendUin)
        {
            // 保存数据库
            #region 只收集群聊
            //try
            //{
            //    int successRow = DbContext.InsertIntoMessage(new Models.Message()
            //    {
            //        Content = message,
            //        CreateTime = DateTime.Now.ToTimeStamp13(),
            //        GroupName = null,
            //        GroupUin = null,
            //        QQName = obj.e.Message.Sender.Name,
            //        QQUin = friendUin.ToString()
            //    });
            //    Console.WriteLine($"成功插入 {successRow} 行");
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("插入表 Message 出错:");
            //    Console.WriteLine(ex.ToString());
            //} 
            #endregion

            SettingsModel settingsModel = PluginCore.PluginSettingsModelFactory.Create<SettingsModel>(nameof(QQStatPlugin));
            if (settingsModel.AdminQQ != null && settingsModel.AdminQQ == friendUin.ToString())
            {
                // 来自 超级管理员 的消息
                if (message.Trim() == "#计数")
                {
                    try
                    {
                        long count = DbContext.Count().Result;
                        obj.s.SendFriendMessage(friendUin, $"共收集 Message {count} 条");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("查询 Message 条数 出错");
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


        #region 定时任务
        public async Task ExecuteAsync()
        {
            try
            {
                #region TODO
                //SettingsModel settingsModel = PluginCore.PluginSettingsModelFactory.Create<SettingsModel>(nameof(QQStatPlugin));
                //if (KonataBotStore.Bot != null && KonataBotStore.Bot.IsOnline())
                //{
                //    var groupList = await KonataBotStore.Bot.GetGroupList(forceUpdate: true);
                //    foreach (var group in groupList)
                //    {
                //        if (settingsModel.ChartGroups.Contains(group.Uin.ToString()))
                //        {
                //            SendStackedArea((KonataBotStore.Bot, null), message: "#折线", groupUin: group.Uin, settingsModel: settingsModel);

                //            List<BaseChain> baseChains = new List<BaseChain>()
                //            {
                //                TextChain.Create("发送 #帮助 获取更多信息")
                //            };

                //            await KonataBotStore.Bot.SendGroupMessage(groupUin: group.Uin, baseChains.ToArray());
                //        }
                //    }

                //} 
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine($"执行定时任务失败: {ex.ToString()}");
            }

            await Task.CompletedTask;
        }
        #endregion


        public void SendCalendar((Bot s, GroupMessageEvent e) obj, string message, uint groupUin, uint memberUin, SettingsModel settingsModel)
        {
            string token = Guid.NewGuid().ToString();
            Controllers.CalendarController.TempData.CreateTime = DateTime.Now;
            Controllers.CalendarController.TempData.GroupUin = groupUin.ToString();
            Controllers.CalendarController.TempData.MemeberUin = null;

            // 下方获取当前群聊
            //string urlParam = $"{settingsModel.BaseUrl}/Plugins/QQStatPlugin/Calendar?groupUin={groupUin.ToString()}";
            string urlParam = $"{settingsModel.BaseUrl}/Plugins/QQStatPlugin/Calendar";
            string targetMemberUinStr = message.Replace("#日历", "")?.Trim();
            if (uint.TryParse(targetMemberUinStr, out uint targetMemberUin))
            {
                // 仅此人 日历
                //urlParam += $"&memeberUin={targetMemberUin}";
                Controllers.StackedAreaController.TempData.MemeberUin = targetMemberUin.ToString();
            }

            Console.WriteLine($"准备发送统计: {urlParam}");
            try
            {
                obj.s.SendGroupMessage(groupUin, $"{urlParam} \r\n 1小时后/下次失效,需重新获取,数据较多,请耐心等待");
            }
            catch (Exception ex)
            {
                Console.WriteLine("发送统计失败:");
                Console.WriteLine(ex.ToString());
            }

            // 注意: url 编码, 这样才能正确传参
            urlParam = System.Web.HttpUtility.UrlEncode(urlParam, System.Text.Encoding.UTF8);

            // 加个time 防止缓存
            // ScreenshotUrl: xxx.com?url=
            string imageUrl = $"{settingsModel.ScreenshotUrl}{urlParam}&time={DateTime.Now.ToTimeStamp13()}";

            Console.WriteLine(imageUrl);
            try
            {
                var image = ImageChain.CreateFromUrl(imageUrl);
                obj.s.SendGroupMessage(groupUin, image);
                obj.s.SendGroupPoke(groupUin: groupUin, memberUin: memberUin);
                obj.s.SendGroupMessage(groupUin, AtChain.Create(memberUin));
            }
            catch (Exception ex)
            {
                obj.s.SendGroupMessage(groupUin, "发送 日历 图片失败");
                //obj.s.SendGroupMessage(groupUin, imageUrl);

                Console.WriteLine("发送 日历 图片失败");
                Console.WriteLine(ex.ToString());
            }
        }

        public void SendStackedArea((Bot s, GroupMessageEvent e) obj, string message, uint groupUin, SettingsModel settingsModel, uint memberUin = 0)
        {
            Console.WriteLine("进入 SendStackedArea");
            string token = Guid.NewGuid().ToString();
            Controllers.StackedAreaController.TempData.CreateTime = DateTime.Now;
            Controllers.StackedAreaController.TempData.GroupUin = groupUin.ToString();
            Controllers.StackedAreaController.TempData.MemeberUin = null;

            // 下方获取当前群聊
            //string urlParam = $"{settingsModel.BaseUrl}/Plugins/QQStatPlugin/StackedArea?groupUin={groupUin.ToString()}";
            string urlParam = $"{settingsModel.BaseUrl}/Plugins/QQStatPlugin/StackedArea";
            string targetMemberUinStr = message.Replace("#折线", "")?.Trim();
            if (uint.TryParse(targetMemberUinStr, out uint targetMemberUin))
            {
                // 仅此人 日历
                //urlParam += $"&memeberUin={targetMemberUin}";
                Controllers.StackedAreaController.TempData.MemeberUin = targetMemberUin.ToString();
            }

            Console.WriteLine($"准备发送统计: {urlParam}");
            try
            {
                obj.s.SendGroupMessage(groupUin, $"{urlParam} \r\n 1小时后/下次失效,需重新获取,数据较多,请耐心等待");
            }
            catch (Exception ex)
            {
                Console.WriteLine("发送统计失败:");
                Console.WriteLine(ex.ToString());
            }

            // 注意: url 编码, 这样才能正确传参
            urlParam = System.Web.HttpUtility.UrlEncode(urlParam, System.Text.Encoding.UTF8);

            // 加个time 防止缓存
            // ScreenshotUrl: xxx.com?url=
            string imageUrl = $"{settingsModel.ScreenshotUrl}{urlParam}&time={DateTime.Now.ToTimeStamp13()}";

            Console.WriteLine(imageUrl);
            try
            {
                var image = ImageChain.CreateFromUrl(imageUrl);
                obj.s.SendGroupMessage(groupUin, image);
                if (memberUin > 0)
                {
                    obj.s.SendGroupPoke(groupUin: groupUin, memberUin: memberUin);
                    obj.s.SendGroupMessage(groupUin, AtChain.Create(memberUin));
                }
            }
            catch (Exception ex)
            {
                obj.s.SendGroupMessage(groupUin, "发送 折线 图片失败");
                //obj.s.SendGroupMessage(groupUin, imageUrl);

                Console.WriteLine("发送 折线 图片失败");
                Console.WriteLine(ex.ToString());
            }
        }

        private string ConvertToString(MessageChain chains)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in chains)
            {
                switch (item.Type)
                {
                    case BaseChain.ChainType.At:
                        sb.AppendLine(item.ToString());
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
