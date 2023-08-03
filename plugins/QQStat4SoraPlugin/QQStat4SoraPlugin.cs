using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PluginCore.IPlugins;
using QQStat4SoraPlugin.Utils;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Sora.EventArgs.SoraEvent;
using Sora.Entities.Segment;
using Sora.Entities;

namespace QQStat4SoraPlugin
{
    public class QQStat4SoraPlugin : BasePlugin, ISoraPlugin
    {

        public override (bool IsSuccess, string Message) AfterEnable()
        {
            Console.WriteLine($"{nameof(QQStat4SoraPlugin)}: {nameof(AfterEnable)}");
            return base.AfterEnable();
        }

        public override (bool IsSuccess, string Message) BeforeDisable()
        {
            Console.WriteLine($"{nameof(QQStat4SoraPlugin)}: {nameof(BeforeDisable)}");
            return base.BeforeDisable();
        }

        public void OnGroupMessage(string msgType, GroupMessageEventArgs eventArgs)
        {
            var groupInfo = eventArgs.SourceGroup.GetGroupInfo().Result.groupInfo;
            string groupName = groupInfo.GroupName;
            long groupUin = groupInfo.GroupId;
            long memberUin = eventArgs.SenderInfo.UserId;

            SettingsModel settingsModel = PluginCore.PluginSettingsModelFactory.Create<SettingsModel>(nameof(QQStat4SoraPlugin));

            #region 收集群消息
            try
            {
                // 保存数据库
                int successRow = DbContext.InsertIntoMessage(new Models.Message()
                {
                    Content = ConvertToString(eventArgs.Message),
                    CreateTime = DateTime.Now.ToTimeStamp13(),
                    GroupName = groupName,
                    GroupUin = groupUin.ToString(),
                    QQName = eventArgs.SenderInfo.Nick,
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

        }

        public void OnPrivateMessage(string msgType, PrivateMessageEventArgs eventArgs)
        {
            SettingsModel settingsModel = PluginCore.PluginSettingsModelFactory.Create<SettingsModel>(nameof(QQStat4SoraPlugin));
            if (settingsModel.AdminQQ != null && settingsModel.AdminQQ == eventArgs.SenderInfo.UserId.ToString())
            {
                string message = eventArgs.Message.GetText();
                // 来自 超级管理员 的消息
                if (message.Trim() == "#计数")
                {
                    try
                    {
                        long count = DbContext.Count().Result;
                        eventArgs.Reply(new MessageBody(new List<SoraSegment>(){
                            SoraSegment.Text($"共收集 Message {count} 条")
                        }));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("查询 Message 条数 出错:");
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
        }

        private string ConvertToString(Sora.Entities.MessageContext message)
        {
            string rtnStr = message.ToString();

            return rtnStr;
        }

    }
}
