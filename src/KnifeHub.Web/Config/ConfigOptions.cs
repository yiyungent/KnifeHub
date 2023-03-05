namespace KnifeHub.Web.Config
{
    public class ConfigOptions
    {
        public const string Config = "KnifeHub";

        public bool AllowAllCors { get; set; }

        public List<string> CorsWhiteList { get; set; }
    }
}