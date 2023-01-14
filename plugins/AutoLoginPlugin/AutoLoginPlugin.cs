using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PluginCore.IPlugins;
using Konata.Core;
using Konata.Core.Events.Model;
using Konata.Core.Interfaces.Api;
using AutoLoginPlugin.Utils;
using Konata.Core.Message.Model;
using Konata.Core.Common;
using System.Text;
using Konata.Core.Message;
using System.Collections.Generic;
using Konata.Core.Interfaces;
using System.IO;
using PluginCore;
using KonataPlugin;

namespace AutoLoginPlugin
{
    public class AutoLoginPlugin : BasePlugin, ITimeJobPlugin
    {
        /// <summary>
        /// 1min
        /// </summary>
        public long SecondsPeriod => 60;

        public override (bool IsSuccess, string Message) AfterEnable()
        {
            Console.WriteLine($"{nameof(AutoLoginPlugin)}: {nameof(AfterEnable)}");
            return base.AfterEnable();
        }

        public override (bool IsSuccess, string Message) BeforeDisable()
        {
            Console.WriteLine($"{nameof(AutoLoginPlugin)}: {nameof(BeforeDisable)}");
            return base.BeforeDisable();
        }

        #region 定时任务
        public async Task ExecuteAsync()
        {
            try
            {
                SettingsModel settingsModel = PluginCore.PluginSettingsModelFactory.Create<SettingsModel>(nameof(AutoLoginPlugin));
                string filePath = Path.Combine(PluginPathProvider.PluginsRootPath(), nameof(AutoLoginPlugin), "BotKeyStore.json");
                if (KonataBotStore.Bot != null && KonataBotStore.Bot.IsOnline() && KonataBotStore.Bot.KeyStore != null)
                {
                    string jsonStr = KonataPlugin.Utils.JsonUtil.Obj2JsonStr(KonataBotStore.Bot.KeyStore);
                    File.WriteAllText(filePath, contents: jsonStr, Encoding.UTF8);
                }
                // 取消: 发现这么调用, 会触发验证, 而直接 用 BotKeyStore new 一个, 就不会(好多次这样都没有验证)
                //else if (QQBotStore.Bot != null && !QQBotStore.Bot.IsOnline())
                //{
                //    #region 重新登录
                //    await Login(settingsModel);
                //    #endregion
                //}
                else
                {
                    if (File.Exists(filePath))
                    {
                        #region 重新 new
                        string jsonStr = File.ReadAllText(filePath, Encoding.UTF8);
                        BotKeyStore botKeyStore = KonataPlugin.Utils.JsonUtil.JsonStr2Obj<BotKeyStore>(jsonStr);
                        KonataBotStore.Bot = BotFather.Create(BotConfig.Default(), BotDevice.Default(), botKeyStore);
                        #endregion

                        #region 重新登录
                        await Login(settingsModel);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"执行定时任务失败: {ex.ToString()}");
            }

            await Task.CompletedTask;
        }

        private static async Task Login(SettingsModel settingsModel)
        {
            bool isLoginSuccess = false;
            try
            {
                isLoginSuccess = await KonataBotStore.Bot.Login();
            }
            catch (Exception ex)
            {
                Console.WriteLine("重新登录出错:");
                Console.WriteLine(ex.ToString());
            }
            if (isLoginSuccess)
            {
                if (!string.IsNullOrEmpty(settingsModel.AdminQQ) && uint.TryParse(settingsModel.AdminQQ, out uint adminUin))
                {
                    await KonataBotStore.Bot.SendFriendMessage(friendUin: adminUin, "自动重新登录成功");
                }
            }
            else
            {
                Console.WriteLine("重新登录失败");
            }
        }
        #endregion





    }
}
