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
using PluginCore.Interfaces;

namespace AutoLoginPlugin
{
    public class AutoLoginPlugin : BasePlugin, ITimeJobPlugin, IQQBotPlugin
    {

        #region Fields

        /// <summary>
        /// 1min
        /// </summary>
        public long SecondsPeriod => 60;

        private readonly IPluginFinder _pluginFinder;

        private readonly bool _debug;

        #endregion

        #region Ctor
        public AutoLoginPlugin(IPluginFinder pluginFinder)
        {
            _pluginFinder = pluginFinder;
            string debugStr = KonataPlugin.Utils.EnvUtil.GetEnv("DEBUG");
            if (!string.IsNullOrEmpty(debugStr) && bool.TryParse(debugStr, out bool debug))
            {
                _debug = debug;
            }
            else
            {
                _debug = false;
            }
        }
        #endregion

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
                //string filePath = Path.Combine(PluginPathProvider.PluginsRootPath(), nameof(AutoLoginPlugin), "BotKeyStore.json");
                if (KonataBotStore.Bot != null && KonataBotStore.Bot.IsOnline() && KonataBotStore.Bot.KeyStore != null)
                {
                    // 将正常上线状态下的 KeyStore 保存下来

                    //string jsonStr = KonataPlugin.Utils.JsonUtil.Obj2JsonStr(KonataBotStore.Bot.KeyStore);
                    //File.WriteAllText(filePath, contents: jsonStr, Encoding.UTF8);
                    var konataPluginOldSettings = KonataPlugin.Utils.SettingsUtil.Get(nameof(KonataPlugin));
                    konataPluginOldSettings.BotKeyStore = KonataBotStore.Bot.KeyStore;
                    KonataPlugin.Utils.SettingsUtil.Set(nameof(KonataPlugin), konataPluginOldSettings);
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
                    #region 重新 new
                    //string jsonStr = File.ReadAllText(filePath, Encoding.UTF8);
                    //BotKeyStore botKeyStore = KonataPlugin.Utils.JsonUtil.JsonStr2Obj<BotKeyStore>(jsonStr);
                    var konataPluginOldSettings = KonataPlugin.Utils.SettingsUtil.Get(nameof(KonataPlugin));
                    // 再利用之前保存的 KeyStore 重新创建机器人并登录上线
                    if (konataPluginOldSettings.BotKeyStore != null)
                    {
                        // 由于匿机后, 对象内存数据全丢失, 只能重新创建
                        //KonataBotStore.Bot = BotFather.Create(BotConfig.Default(), BotDevice.Default(), konataPluginOldSettings.BotKeyStore);

                        #region 重新登录
                        await Login(settingsModel, konataPluginOldSettings);
                        #endregion
                    }
                    #endregion

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"执行定时任务失败: {ex.ToString()}");
            }

            await Task.CompletedTask;
        }

        private async Task Login(SettingsModel settingsModel, KonataPlugin.SettingsModel konataPluginOldSettings)
        {
            bool isLoginSuccess = false;
            try
            {
                //isLoginSuccess = await KonataBotStore.Bot.Login();
                var konataPluginHomeController = new KonataPlugin.Controllers.HomeController(_pluginFinder);
                // 注意: 这里无法获取是否已经登录成功完成上线
                // 直接用 此插件的登录方法不容易出错
                var responseModel = await konataPluginHomeController.Login(new KonataPlugin.RequestModels.LoginRequestModel
                {
                    LoginType = "config",
                    BotKeyStore = KonataPlugin.Utils.JsonUtil.Obj2JsonStr(konataPluginOldSettings.BotKeyStore)
                });
                //if (responseModel.Code=)
                //{

                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine("重新登录出错:");
                Console.WriteLine(ex.ToString());
            }
            //if (isLoginSuccess)
            //{
            //    if (!string.IsNullOrEmpty(settingsModel.AdminQQ) && uint.TryParse(settingsModel.AdminQQ, out uint adminUin))
            //    {
            //        await KonataBotStore.Bot.SendFriendMessage(friendUin: adminUin, "自动重新登录成功");
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("重新登录失败");
            //}
        }

        public void OnGroupMessage((Bot s, GroupMessageEvent e) obj, string message, string groupName, uint groupUin, uint memberUin)
        {

        }

        public void OnFriendMessage((Bot s, FriendMessageEvent e) obj, string message, uint friendUin)
        {

        }

        public async void OnBotOnline((Bot s, BotOnlineEvent e) obj, string botName, uint botUin)
        {
            SettingsModel settingsModel = PluginCore.PluginSettingsModelFactory.Create<SettingsModel>(nameof(AutoLoginPlugin));
            if (!string.IsNullOrEmpty(settingsModel.AdminQQ) && uint.TryParse(settingsModel.AdminQQ, out uint adminUin))
            {
                await KonataBotStore.Bot.SendFriendMessage(friendUin: adminUin, "自动重新登录成功");
            }
        }

        public void OnBotOffline((Bot s, BotOfflineEvent e) obj, string botName, uint botUin)
        {

        }
        #endregion





    }
}
