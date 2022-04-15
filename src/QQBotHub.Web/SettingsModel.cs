
using Konata.Core.Common;

namespace QQBotHub.Web
{
    public class SettingsModel
    {
        // 使用 = "" , 确保不被 json 化为 null
        public string Uin { get; set; } = "";
        public string Password { get; set; } = "";
        public BotKeyStore BotKeyStore { get; set; }
    }
}
