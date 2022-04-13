using Konata.Core;
using Konata.Core.Common;
using Konata.Core.Interfaces;
using Konata.Core.Interfaces.Api;
using PluginCore.IPlugins;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Konata.Core.Events.Model.CaptchaEvent;

namespace QQBotPlugin
{
    public class QQBotPlugin : BasePlugin
    {
        public override (bool IsSuccess, string Message) AfterEnable()
        {
            Utils.LogUtil.Info($"{nameof(QQBotPlugin)}: {nameof(AfterEnable)}");

            #region Bot

            // Create a bot instance
            var bot = BotFather.Create(BotPluginStore.BotConfig, BotPluginStore.BotDevice, BotPluginStore.BotKeyStore);
            {
                // Print the log
                bot.OnLog += (s, e) =>
                {
                    //Utils.LogUtil.Info(e.EventMessage);
                };

                // Handle the captcha
                bot.OnCaptcha += (s, e) =>
                {
                    Utils.LogUtil.Info("QQ 登录验证:");
                    if (e.Type == CaptchaType.Slider)
                    {
                        Utils.LogUtil.Info(e.SliderUrl);
                        Utils.LogUtil.Info($"使用 http://your-domain/api/plugins/QQBotPlugin/Login?captcha=your-captcha&code=your-code 提交验证");
                        //((Bot)s).SubmitSliderTicket(Console.ReadLine());

                        Controllers.LoginController.CaptchaType = CaptchaType.Slider;
                        Controllers.LoginController.CaptchaMessage = $"{e.SliderUrl}";
                    }
                    else if (e.Type == CaptchaType.Sms)
                    {
                        Utils.LogUtil.Info(e.Phone);
                        Utils.LogUtil.Info($"使用 http://your-domain/api/plugins/QQBotPlugin/Login?captcha=your-captcha&code=your-code 提交验证");
                        //((Bot)s).SubmitSmsCode(Console.ReadLine());

                        Controllers.LoginController.CaptchaType = CaptchaType.Sms;
                        Controllers.LoginController.CaptchaMessage = $"{e.Phone}";
                    }
                };

                // Handle messages from group
                bot.OnGroupMessage += (s, e) =>
                {
                    Utils.LogUtil.Info($"群消息: {e.Message.Chain?.FirstOrDefault()?.ToString() ?? ""}");
                };

                // Handle messages from friend
                bot.OnFriendMessage += (s, e) =>
                {
                    Utils.LogUtil.Info($"好友消息: {e.Message.Chain?.FirstOrDefault()?.ToString() ?? ""}");
                };

                bot.OnBotOnline += (s, e) =>
                {
                    Utils.LogUtil.Info($"{s.Name} 上线");
                };

                bot.OnBotOffline += (s, e) =>
                {
                    Utils.LogUtil.Info($"{s.Name} 离线");
                };
                // ... More handlers
            }

            // Do login
            //Task<bool> loginTask = bot.Login();

            // 下方操作会阻塞, 并且是阻塞到过登录验证
            //if (!bot.Login().Result)
            //{
            //    Utils.LogUtil.Info($"{nameof(QQBotPlugin)} 启用后 QQ 自动登录失败");
            //    return base.AfterEnable();
            //}
            //else
            //{
            //    Utils.LogUtil.Info($"{nameof(QQBotPlugin)} 启用后 QQ 自动登录成功");
            //}

            #endregion

            // 登录成功, 保存起来
            BotPluginStore.Bot = bot;

            Utils.LogUtil.Info("QQBot 启用成功");


            return base.AfterEnable();
        }

        public override (bool IsSuccess, string Message) BeforeDisable()
        {
            Utils.LogUtil.Info($"{nameof(QQBotPlugin)}: {nameof(BeforeDisable)}");
            return base.BeforeDisable();
        }

    }

    public class BotPluginStore
    {
        public static Bot Bot { get; set; }

        public static class StoreUtil
        {
            public static T Get<T>(string fileName, T defaultObj)
            {
                T rtnObj = defaultObj;
                string configDir = Path.Combine(PluginCore.PluginPathProvider.PluginsRootPath(), nameof(QQBotPlugin), "QQConfig");
                if (!Directory.Exists(configDir))
                {
                    Directory.CreateDirectory(configDir);
                }
                string filePath = Path.Combine(configDir, fileName);
                if (!File.Exists(filePath))
                {
                    string jsonStr = Utils.JsonUtil.Obj2JsonStr(rtnObj);
                    File.WriteAllText(path: filePath, contents: jsonStr, encoding: System.Text.Encoding.UTF8);
                }
                else
                {
                    string jsonStr = File.ReadAllText(path: filePath, encoding: System.Text.Encoding.UTF8);
                    rtnObj = Utils.JsonUtil.JsonStr2Obj<T>(jsonStr);
                }

                return rtnObj;
            }

            public static void Set<T>(T obj, string fileName)
            {
                string configDir = Path.Combine(PluginCore.PluginPathProvider.PluginsRootPath(), nameof(QQBotPlugin), "QQConfig");
                if (!Directory.Exists(configDir))
                {
                    Directory.CreateDirectory(configDir);
                }
                string filePath = Path.Combine(configDir, fileName);
                string jsonStr = Utils.JsonUtil.Obj2JsonStr(obj);
                File.WriteAllText(path: filePath, contents: jsonStr, encoding: System.Text.Encoding.UTF8);
            }

        }

        public static BotConfig BotConfig
        {
            get
            {
                BotConfig botConfig = BotConfig.Default();
                botConfig = StoreUtil.Get<BotConfig>("config.json", botConfig);

                return botConfig;
            }
            set
            {
                BotConfig botConfig = value;
                StoreUtil.Set<BotConfig>(botConfig, "config.json");
            }
        }

        public static BotDevice BotDevice
        {
            get
            {
                BotDevice botDevice = BotDevice.Default();
                botDevice = StoreUtil.Get<BotDevice>("device.json", botDevice);

                return botDevice;
            }
            set
            {
                BotDevice botDevice = value;
                StoreUtil.Set<BotDevice>(botDevice, "device.json");
            }
        }

        public static BotKeyStore BotKeyStore
        {
            get
            {
                SettingsModel settingsModel = PluginCore.PluginSettingsModelFactory.Create<SettingsModel>(nameof(QQBotPlugin));
                string account = settingsModel.QQ;
                string password = settingsModel.Password;
                BotKeyStore botKeyStore = new BotKeyStore(uin: account, password: password);
                botKeyStore = StoreUtil.Get<BotKeyStore>("keystore.json", botKeyStore);

                return botKeyStore;
            }
            set
            {
                BotKeyStore botKeyStore = value;
                StoreUtil.Set<BotKeyStore>(botKeyStore, "keystore.json");
            }
        }
    }

}
