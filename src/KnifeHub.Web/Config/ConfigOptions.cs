using Serilog.Events;

namespace KnifeHub.Web.Config
{
    public class ConfigOptions
    {
        public const string Config = nameof(KnifeHub);

        public ConfigOptions()
        {
            this.Log = new LogModel();
        }

        public SentryModel Sentry { get; set; }

        public class SentryModel
        {
            public bool Enabled { get; set; }
        }

        public bool AllowAllCors { get; set; } = false;

        public List<string> CorsWhiteList { get; set; }

        //public List<string> CorsAllowHeaders { get; set; }

        public LogModel Log { get; set; }

        public class LogModel
        {
            public string MinimumLevel { get; set; }

            public LogEventLevel MinimumLevelEnum
            {
                get
                {
                    LogEventLevel rtn = LogEventLevel.Warning;
                    switch (MinimumLevel?.ToLower() ?? "")
                    {
                        case "debug":
                            rtn = LogEventLevel.Debug;
                            break;
                        case "information":
                            rtn = LogEventLevel.Information;
                            break;
                        case "warning":
                            rtn = LogEventLevel.Warning;
                            break;
                        case "error":
                            rtn = LogEventLevel.Error;
                            break;
                        default:
                            rtn = LogEventLevel.Warning;
                            break;
                    }

                    return rtn;
                }
            }

            public int RetainedFileCountLimit { get; set; } = 31;

            public int RetainedFileTimeLimitDays { get; set; } = 31;
        }
    }
}
