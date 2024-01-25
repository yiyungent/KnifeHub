using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PluginCore.IPlugins;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using PluginCore;
using Microsoft.Extensions.Logging;
using Hangfire;
using Hangfire.Storage.SQLite;

namespace HangfirePlugin
{
    /// <summary>
    /// https://docs.hangfire.io/en/latest/getting-started/aspnet-core-applications.html
    /// </summary>
    public class HangfirePlugin : BasePlugin, IStartupPlugin
    {
        #region Fields
        private readonly ILogger<HangfirePlugin> _logger;
        // private readonly IBackgroundJobClient _backgroundJobClient;
        #endregion

        #region Ctor
        public HangfirePlugin(ILogger<HangfirePlugin> logger)
        {
            _logger = logger;
            // _backgroundJobClient = backgroundJobClient;
        }
        #endregion


        public override (bool IsSuccess, string Message) AfterEnable()
        {
            Console.WriteLine($"{nameof(HangfirePlugin)}: {nameof(AfterEnable)}");
            var res = base.AfterEnable();
            if (res.IsSuccess)
            {
                res.Message = $"注意: 插件({nameof(HangfirePlugin)})需主程序重启后才能正式完成启用";
            }

            return res;
        }

        public override (bool IsSuccess, string Message) BeforeDisable()
        {
            Console.WriteLine($"{nameof(HangfirePlugin)}: {nameof(BeforeDisable)}");
            return base.BeforeDisable();
        }

        public void Configure(IApplicationBuilder app)
        {
            // app.UseHangfireDashboard();
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfirePluginAuthorizationFilter() }
            });
            // _backgroundJobClient.Enqueue(() => Console.WriteLine("Hello world from Hangfire!"));
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add Hangfire services.
            string dbFilePath = Path.Combine(PluginPathProvider.PluginsRootPath(), nameof(HangfirePlugin), "hangfire.db");
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                // https://github.com/raisedapp/Hangfire.Storage.SQLite
                .UseSQLiteStorage(nameOrConnectionString: dbFilePath, options: new SQLiteStorageOptions()
                {

                })
                // https://github.com/HangfireIO/Hangfire/issues/470
                .UseTypeResolver((typeName) =>
                {
                    try
                    {
                        return Hangfire.Common.TypeHelper.DefaultTypeResolver(typeName); //try using default resolver
                    }
                    catch (Exception) //if couldn't find
                    {
                        //get it from AppDomain loaded assemblies.
                        var reqType = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes().Where(x => typeName.StartsWith(x.FullName))).FirstOrDefault();

                        if (reqType != null)
                            return reqType;

                        throw;
                    }
                })
            );

            // Add the processing server as IHostedService
            services.AddHangfireServer();

            // https://github.com/HangfireIO/Hangfire/issues/470
            //Hangfire.Common.TypeHelper.CurrentTypeResolver += typeName =>
            //{
            //    if (typeName.Contains("MyCompany.Services.Messaging"))
            //    {
            //        // Load assembly from the specific place and use it to find the type
            //        return Assembly.GetType(typeName);
            //    }

            //    return TypeHelper.DefaultTypeResolver(typeName);
            //};
        }

        public int ConfigureServicesOrder => -999;

        public int ConfigureOrder => -999;

    }
}
