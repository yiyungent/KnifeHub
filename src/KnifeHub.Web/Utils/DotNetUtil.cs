// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace KnifeHub.Web.Utils
{
    public class DotNetUtil
    {
        public static string DotNetVersion
        {
            get
            {
                string dotNetVersion =
                    System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription;

                return dotNetVersion;
            }
        }

        public static string? AspNetCoreFileVersion
        {
            get
            {
                var assembly = typeof(Microsoft.AspNetCore.Http.HttpContext).Assembly;
                var fileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
                string? aspNetCoreVersion = fileVersionInfo.FileVersion;

                return aspNetCoreVersion;
            }
        }

        public static string? AspNetCoreProductVersion
        {
            get
            {
                var assembly = typeof(Microsoft.AspNetCore.Http.HttpContext).Assembly;
                var fileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
                string? aspNetCoreVersion = fileVersionInfo.ProductVersion;

                return aspNetCoreVersion;
            }
        }

        public static string ToString()
        {
            string str = $".NET Version: {DotNetVersion} ASP.NET Core FileVersion: {AspNetCoreFileVersion ?? "null"} ASP.NET Core ProductVersion: {AspNetCoreProductVersion ?? "null"}";

            return str;
        }
    }
}
