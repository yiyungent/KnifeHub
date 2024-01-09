using System.Reflection;

namespace KnifeHub.Web.Utils
{
    public class AppBuildStampUtil
    {
        public static string Branch
        => Stamp[0];

        public static string CommitHash
            => Stamp[1][..16];

        public static string BuildTime
            => Stamp[2];

        public static string Version
            => InformationalVersion;

        private static readonly string[] Stamp
            = typeof(Program).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
                .FirstOrDefault(x => x.Key == "BuildStamp").Value.Split(";");

        private static readonly string InformationalVersion
            = typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
    }
}
