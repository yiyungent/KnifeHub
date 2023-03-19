using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PluginCore;
using PluginCore.Interfaces;
using TgClientPlugin.Utils;
using TgClientPlugin.ResponseModels;
using TgClientPlugin.RequestModels;
using PluginCore.IPlugins;
using Starksoft.Net.Proxy;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TL;

namespace TgClientPlugin.Controllers
{
    /// <summary>
    /// 其实也可以不写这个, 直接访问 Plugins/ZhiDaoPlugin/index.html
    /// 
    /// 下面的方法, 是去掉 index.html
    /// 
    /// 若 wwwroot 下有其它需要访问的文件, 如何 css, js, 而你又不想每次新增 action 指定返回, 则 Route 必须 Plugins/{PluginId},
    /// 这样访问 Plugins/TgClientPlugin/css/main.css 就会访问到你插件下的 wwwroot/css/main.css
    /// </summary>
    [Route($"api/Plugins/{nameof(TgClientPlugin)}")]
    [Authorize("PluginCore.Admin")]
    public class HomeController : Controller
    {
        #region Fields

        private readonly IPluginFinder _pluginFinder;
        private readonly bool _debug;

        #endregion

        #region Propertities

        public static WTelegram.Client Client;
        public static User My;
        public static readonly Dictionary<long, User> Users = new();
        public static readonly Dictionary<long, ChatBase> Chats = new();
        public static bool TgClientConnected { get; set; }

        #endregion

        #region Ctor
        public HomeController(IPluginFinder pluginFinder)
        {
            _pluginFinder = pluginFinder;
            string debugStr = EnvUtil.GetEnv("DEBUG");
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


        #region Actions

        [Route($"/Plugins/{nameof(TgClientPlugin)}")]
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            string indexFilePath = System.IO.Path.Combine(PluginPathProvider.PluginWwwRootDir(nameof(TgClientPlugin)), "index.html");

            return PhysicalFile(indexFilePath, "text/html");
        }

        [HttpGet]
        [Route(nameof(Info))]
        public async Task<BaseResponseModel> Info()
        {
            BaseResponseModel responseModel = new BaseResponseModel();
            try
            {
                // TODO: Info
                responseModel.Code = 1;
                responseModel.Message = "成功";
            }
            catch (Exception ex)
            {
                responseModel.Code = -1;
                responseModel.Message = "失败";
                responseModel.Data = ex.ToString();
            }

            return await Task.FromResult(responseModel);
        }

        [HttpPost]
        [Route(nameof(Login))]
        public async Task<BaseResponseModel> Login([FromBody] LoginRequestModel requestModel)
        {
            BaseResponseModel responseModel = new BaseResponseModel();
            LoginResponseDataModel dataModel = new LoginResponseDataModel();
            try
            {
                var settings = Utils.SettingsUtil.Get(nameof(TgClientPlugin));
                if (Client == null)
                {
                    Client = new WTelegram.Client(apiID: settings.ApiId, apiHash: settings.ApiHash);
                    if (settings.Proxy != null && settings.Proxy.ProxyEnabled)
                    {
                        // [WTelegramClient/EXAMPLES.md at master · wiz0u/WTelegramClient](https://github.com/wiz0u/WTelegramClient/blob/master/EXAMPLES.md#use-a-proxy-or-mtproxy-to-connect-to-telegram)
                        // SOCKS/HTTPS proxies 
                        Client.TcpHandler = async (address, port) =>
                        {
                            // ProxyHost, ProxyPort, ProxyUsername, ProxyPassword
                            var proxy = new Socks5ProxyClient(proxyHost: settings.Proxy.ProxyHost, proxyPort: settings.Proxy.ProxyPort);
                            //var proxy = xNet.Socks5ProxyClient.Parse("host:port:username:password");
                            return proxy.CreateConnection(address, port);
                        };
                    }
                    Client.OnUpdate += Client_OnUpdate;
                }
                if (Client.User == null)
                {
                    // 先尝试一下不使用 requestModel.LoginInfo, 因为可能保存了登录 Session, 而无需
                    string loginNeed = "请在输入框中输入 手机号(中国大陆 +86), 再次点击登录 (1)";
                    try
                    {
                        loginNeed = await Client.Login(requestModel.LoginInfo);
                    }
                    catch (System.Exception ex)
                    {
                        System.Console.WriteLine(ex.ToString());
                    }
                    if (loginNeed == null)
                    {
                        // 登录成功: loginNeed 一定为 null
                        try
                        {
                            System.Console.WriteLine("登录成功");
                            // We collect all infos about the users/chats so that updates can be printed with their names
                            var dialogs = await Client.Messages_GetAllDialogs(); // dialogs = groups/channels/users
                            dialogs.CollectUsersChats(Users, Chats);
                            dataModel.LoginNeed = null;
                        }
                        catch (System.Exception ex)
                        {
                            System.Console.WriteLine(ex.ToString());
                            dataModel.LoginNeed = ex.ToString();
                        }
                    }
                    else
                    {
                        switch (loginNeed)
                        {
                            case "verification_code":
                                dataModel.LoginNeed = "请在输入框中输入 验证码, 再次点击登录";
                                break;
                            case "phone_number":
                                dataModel.LoginNeed = "请在输入框中输入 手机号(中国大陆 +86), 再次点击登录";
                                break;
                            default:
                                dataModel.LoginNeed = loginNeed;
                                break;
                        }
                    }
                }

                responseModel.Code = 1;
                responseModel.Message = dataModel.LoginNeed;
                if (dataModel.LoginNeed == null)
                {
                    responseModel.Message = $"登录成功";
                }
                responseModel.Data = dataModel;
            }
            catch (Exception ex)
            {
                responseModel.Code = -1;
                responseModel.Message = "提交登录失败";
                responseModel.Data = ex.ToString();
            }

            return await Task.FromResult(responseModel);
        }

        [HttpPost]
        [Route(nameof(Logout))]
        public async Task<BaseResponseModel> Logout()
        {
            BaseResponseModel responseModel = new BaseResponseModel();
            try
            {
                if (Client != null)
                {
                    try
                    {
                        Client.DisableUpdates();
                        await Client.Auth_LogOut();
                        Client.Dispose();
                        Client = null;
                        GC.Collect();
                    }
                    catch (System.Exception ex)
                    {
                        System.Console.WriteLine(nameof(Logout));
                        System.Console.WriteLine(ex.ToString());
                    }
                }

                responseModel.Code = 1;
                responseModel.Message = "提交退出成功";
            }
            catch (Exception ex)
            {
                responseModel.Code = -1;
                responseModel.Message = "提交退出失败";
                responseModel.Data = ex.ToString();
            }

            return await Task.FromResult(responseModel);
        }

        #endregion

        [Route(nameof(Download))]
        public async Task<ActionResult> Download()
        {
            string dbFilePath = DbContext.DbFilePath;
            var fileStream = System.IO.File.OpenRead(dbFilePath);
            //System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();

            return File(fileStream: fileStream, contentType: "application/x-sqlite3", fileDownloadName: $"{nameof(TgClientPlugin)}.sqlite", enableRangeProcessing: true);
        }

        #region TgClient

        private static async Task Client_OnUpdate(IObject arg)
        {
            if (arg is not UpdatesBase updates) return;
            updates.CollectUsersChats(Users, Chats);
            foreach (var update in updates.UpdateList)
                switch (update)
                {
                    case UpdateNewMessage unm:
                        await DisplayMessage(unm.message);

                        #region UpdateNewMessage
                        int count = DbContext.InsertIntoMessage(new Models.Message
                        {
                            UId = unm.message.From.ID.ToString(),
                            UName = unm.message.From.ToString() ?? "",
                            GroupId = unm.message.Peer.ID.ToString(),
                            GroupName = unm.message.Peer.ToString(),
                            Content = unm.message.ToString(),
                            CreateTime = unm.message.Date.ToTimeStamp13()
                        });
                        if (count < 1)
                        {
                            System.Console.WriteLine($"{nameof(Client_OnUpdate)}: 插入数据库失败");
                        }
                        #endregion

                        break;
                    case UpdateEditMessage uem: await DisplayMessage(uem.message, true); break;
                    // Note: UpdateNewChannelMessage and UpdateEditChannelMessage are also handled by above cases
                    case UpdateDeleteChannelMessages udcm: Console.WriteLine($"{udcm.messages.Length} message(s) deleted in {Chat(udcm.channel_id)}"); break;
                    case UpdateDeleteMessages udm: Console.WriteLine($"{udm.messages.Length} message(s) deleted"); break;
                    case UpdateUserTyping uut: Console.WriteLine($"{User(uut.user_id)} is {uut.action}"); break;
                    case UpdateChatUserTyping ucut: Console.WriteLine($"{Peer(ucut.from_id)} is {ucut.action} in {Chat(ucut.chat_id)}"); break;
                    case UpdateChannelUserTyping ucut2: Console.WriteLine($"{Peer(ucut2.from_id)} is {ucut2.action} in {Chat(ucut2.channel_id)}"); break;
                    case UpdateChatParticipants { participants: ChatParticipants cp }: Console.WriteLine($"{cp.participants.Length} participants in {Chat(cp.chat_id)}"); break;
                    case UpdateUserStatus uus: Console.WriteLine($"{User(uus.user_id)} is now {uus.status.GetType().Name[10..]}"); break;
                    case UpdateUserName uun: Console.WriteLine($"{User(uun.user_id)} has changed profile name: {uun.first_name} {uun.last_name}"); break;
                    case UpdateUser uu: Console.WriteLine($"{User(uu.user_id)} has changed infos/photo"); break;
                    default: Console.WriteLine(update.GetType().Name); break; // there are much more update types than the above example cases
                }
        }

        // in this example method, we're not using async/await, so we just return Task.CompletedTask
        private static Task DisplayMessage(MessageBase messageBase, bool edit = false)
        {
            if (edit) Console.Write("(Edit): ");
            switch (messageBase)
            {
                case Message m: Console.WriteLine($"{Peer(m.from_id) ?? m.post_author} in {Peer(m.peer_id)}> {m.message}"); break;
                case MessageService ms: Console.WriteLine($"{Peer(ms.from_id)} in {Peer(ms.peer_id)} [{ms.action.GetType().Name[13..]}]"); break;
            }
            return Task.CompletedTask;
        }

        private static string User(long id) => Users.TryGetValue(id, out var user) ? user.ToString() : $"User {id}";
        private static string Chat(long id) => Chats.TryGetValue(id, out var chat) ? chat.ToString() : $"Chat {id}";
        private static string Peer(Peer peer) => peer is null ? null : peer is PeerUser user ? User(user.user_id)
            : peer is PeerChat or PeerChannel ? Chat(peer.ID) : $"Peer {peer.ID}";

        #endregion

    }
}
