namespace KonataPlugin.RequestModels
{
    public class LoginRequestModel
    {
        public string Uin { get; set; }

        public string Password { get; set; }

        public string BotKeyStore { get; set; }

        public string LoginType { get;set; }
    }
}
