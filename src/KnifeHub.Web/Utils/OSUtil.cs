using System.Runtime.InteropServices;

namespace KnifeHub.Web.Utils
{
    public static class OSUtil
    {
        public static string PlatformInfo()
        {
            string rtn = $"{GetPlatform()}-{GetOSBitCount()}-OS-{RuntimeInformation.OSArchitecture}-Process-{RuntimeInformation.ProcessArchitecture}";
            return rtn;
        }

        public static bool IsWindows()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }

        public static bool IsLinux()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        }

        public static bool IsMacOS()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        }

        public static string GetPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "Windows";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "Linux";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "MacOS";
            }

            return "Unknown platform";
        }

        public static string GetArchitecture()
        {
            return RuntimeInformation.ProcessArchitecture.ToString();
        }

        public static int GetOSBitCount()
        {
            return (IntPtr.Size == 8) ? 64 : 32;
        }
    }
}
