// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;

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
                string? aspNetCoreVersion = null;
                var assembly = typeof(Microsoft.AspNetCore.Http.HttpContext).Assembly;
                if (string.IsNullOrEmpty(assembly.Location))
                {
                    // 当程序集是动态加载的或者是从内存中加载的（比如在ASP.NET Core应用中，某些程序集可能被嵌入到主程序集中，或者通过AssemblyLoadContext动态加载），此时Location属性可能为空。
                    // 使用 AssemblyFileVersionAttribute 获取文件版本
                    try
                    {
                        var fileVersionAttr = assembly.GetCustomAttribute<System.Reflection.AssemblyFileVersionAttribute>();
                        aspNetCoreVersion = fileVersionAttr?.Version;
                    }
                    catch (Exception ex)
                    { }
                }
                else
                {
                    try
                    {
                        var fileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
                        aspNetCoreVersion = fileVersionInfo.FileVersion;
                    }
                    catch (Exception ex)
                    { }
                }

                return aspNetCoreVersion;
            }
        }

        public static string? AspNetCoreProductVersion
        {
            get
            {
                string? aspNetCoreVersion = null;
                var assembly = typeof(Microsoft.AspNetCore.Http.HttpContext).Assembly;
                if (string.IsNullOrEmpty(assembly.Location))
                {
                    try
                    {
                        var versionAttr1 = assembly.GetCustomAttribute<System.Reflection.AssemblyVersionAttribute>();
                        var versionAttr2 = assembly.GetCustomAttribute<System.Reflection.AssemblyInformationalVersionAttribute>();
                        aspNetCoreVersion = versionAttr1?.Version ?? "null" + "+" + versionAttr2?.InformationalVersion ?? "null";
                    }
                    catch (Exception ex)
                    { }
                }
                else
                {
                    try
                    {
                        var fileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
                        aspNetCoreVersion = fileVersionInfo.ProductVersion;
                    }
                    catch (Exception ex)
                    { }
                }

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
