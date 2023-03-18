using Starksoft.Net.Proxy;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TL;

namespace TgClientPlugin.Utils
{
    public class TgClientUtil
    {
        static WTelegram.Client Client;
        static User My;
        static readonly Dictionary<long, User> Users = new();
        static readonly Dictionary<long, ChatBase> Chats = new();

        public static async Task Init()
        {
            Client = new WTelegram.Client();

            var oldSettings = Utils.SettingsUtil.Get(nameof(TgClientPlugin));
            if (oldSettings.Proxy != null && oldSettings.Proxy.ProxyEnabled)
            {
                // [WTelegramClient/EXAMPLES.md at master · wiz0u/WTelegramClient](https://github.com/wiz0u/WTelegramClient/blob/master/EXAMPLES.md#use-a-proxy-or-mtproxy-to-connect-to-telegram)
                // SOCKS/HTTPS proxies 
                Client.TcpHandler = async (address, port) =>
                {
                    // ProxyHost, ProxyPort, ProxyUsername, ProxyPassword
                    var proxy = new Socks5ProxyClient(proxyHost: "127.0.0.1", proxyPort: 10808);
                    //var proxy = xNet.Socks5ProxyClient.Parse("host:port:username:password");
                    return proxy.CreateConnection(address, port);
                };
            }

            using (Client)
            {
                Client.OnUpdate += Client_OnUpdate;
                // My = await Client.LoginUserIfNeeded();
                await DoLogin(Environment.GetEnvironmentVariable("phone_number"));

                // We collect all infos about the users/chats so that updates can be printed with their names
                var dialogs = await Client.Messages_GetAllDialogs(); // dialogs = groups/channels/users
                dialogs.CollectUsersChats(Users, Chats);
                Console.ReadKey();
            }
        }

        private static async Task DoLogin(string loginInfo) // (add this method to your code)
        {
            while (Client.User == null)
            {
                switch (await Client.Login(loginInfo)) // returns which config is needed to continue login
                {
                    case "verification_code":
                        loginInfo = null;
                        while(loginInfo == null) {
                            Console.WriteLine("需要 verification_code ! 尝试从 code.txt 获取");
                            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "code.txt");
                            string code = await File.ReadAllTextAsync(path: filePath, encoding: System.Text.Encoding.UTF8);
                            loginInfo = null;
                            if (!string.IsNullOrEmpty(code))
                            {
                                loginInfo = code.Trim();
                            }
                            Thread.Sleep(3000);
                        }
                        break;
                    default: loginInfo = ""; break;
                }
            }
            My = Client.User;
            Users[My.id] = My;

            // Note: on login, Telegram may sends a bunch of updates/messages that happened in the past and were not acknowledged
            Console.WriteLine($"We are logged-in as {My.username ?? My.first_name + " " + My.last_name} (id {My.id})");
        }


        // if not using async/await, we could just return Task.CompletedTask
        private static async Task Client_OnUpdate(IObject arg)
        {
            if (arg is not UpdatesBase updates) return;
            updates.CollectUsersChats(Users, Chats);
            foreach (var update in updates.UpdateList)
                switch (update)
                {
                    case UpdateNewMessage unm: await DisplayMessage(unm.message); break;
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

    }
}
