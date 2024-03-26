namespace KnifeHub.Web.Config
{
    public class ConfigOptions
    {
        public const string Config = nameof(KnifeHub);

        public ConfigOptions()
        {
            this.Log = new LogModel();
        }

        public bool AllowAllCors { get; set; } = false;

        public List<string> CorsWhiteList { get; set; }

        public LogModel Log { get; set; }

        public class LogModel
        {
            public int RetainedFileCountLimit { get; set; } = 31;

            public int RetainedFileTimeLimitDays { get; set; } = 31;
        }
    }
}
