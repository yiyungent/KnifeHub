using System;
using System.Threading.Tasks;
using PluginCore.IPlugins;
using PluginCore.Interfaces;
using QQChannelFramework.Models.MessageModels;
using QQChannelFramework.Api;
using PluginCore;
using QQChannelPlugin;
using QQChannelPlugin.IPlugins;

namespace AutoLogin4QQChannelPlugin
{
    public class AutoLogin4QQChannelPlugin : BasePlugin, ITimeJobPlugin, IQQChannelPlugin
    {

        #region Fields

        private readonly IPluginFinder _pluginFinder;

        private static bool _isFirstStart = true;

        #endregion

        #region Props
        public long SecondsPeriod
        {
            get
            {
                var settings = PluginSettingsModelFactory.Create<SettingsModel>(nameof(AutoLogin4QQChannelPlugin));

                return settings.SecondsPeriod;
            }
        }
        #endregion

        #region Ctor
        public AutoLogin4QQChannelPlugin(IPluginFinder pluginFinder)
        {
            _pluginFinder = pluginFinder;
        }
        #endregion

        public override (bool IsSuccess, string Message) AfterEnable()
        {
            Console.WriteLine($"{nameof(AutoLogin4QQChannelPlugin)}: {nameof(AfterEnable)}");
            return base.AfterEnable();
        }

        public override (bool IsSuccess, string Message) BeforeDisable()
        {
            Console.WriteLine($"{nameof(AutoLogin4QQChannelPlugin)}: {nameof(BeforeDisable)}");
            return base.BeforeDisable();
        }

        #region 定时任务
        public async Task ExecuteAsync()
        {
            try
            {
                #region 由于内部自动断线重连, 因此断线只有可能是由于程序重启导致需要重新登录上线
                if (QQChannelBotStore.Bots == null || QQChannelBotStore.Bots.Count <= 0)
                {
                    try
                    {
                        var qqChannelPluginHomeController = new QQChannelPlugin.Controllers.HomeController(_pluginFinder);
                        await qqChannelPluginHomeController.Login();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("尝试 自动重新登录 QQ频道机器人 失败:");
                        Console.WriteLine(ex.ToString());
                    }
                }
                #endregion

                #region 初始化
                if (_isFirstStart)
                {
                    if (QQChannelBotStore.Bots != null && QQChannelBotStore.Bots.Count >= 1)
                    {
                        foreach (var item in QQChannelBotStore.Bots)
                        {
                            // MyBot内部实现了断线重连机制，不需要用户通过监听此事件进行主动断线重连
                            //item.ChannelBot.OnClose += () => { };

                            // TODO: 其它需要的未注册事件 可以在这里做
                        }
                    }
                    _isFirstStart = false;
                }
                #endregion


            }
            catch (Exception ex)
            {
                Console.WriteLine($"执行定时任务失败: {ex.ToString()}");
            }

            await Task.CompletedTask;
        }


        #endregion


        #region IQQChannelPlugin
        public void OnConnected(string botAppId)
        {
            Console.WriteLine($"{botAppId} QQ频道机器人 自动重新登录成功");
        }

        public void AuthenticationSuccess(string botAppId)
        {

        }

        public void OnError(string botAppId, Exception ex)
        {

        }

        public void ReceivedAtMessage(string botAppId, Message message, QQChannelApi qChannelApi)
        {

        }

        public void ReceivedDirectMessage(string botAppId, Message message, QQChannelApi qChannelApi)
        {

        }

        public void ReceivedUserMessage(string botAppId, Message message, QQChannelApi qChannelApi)
        {

        }
        #endregion




    }
}
