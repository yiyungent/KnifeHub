using PluginCore.AspNetCore.Extensions;
using PluginCore.AspNetCore.lmplements;
using PluginCore.Interfaces;
using PluginCore.lmplements;
using KnifeHub.Web.Config;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Serilog;

namespace KnifeHub.Web
{
    public class Program
    {
        #region Fields
        // 导入 WinApi
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hwnd, int cmdShow);

        // 定义可见性常量
        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;
        #endregion

        public static void Main(string[] args)
        {
            // https://github.com/serilog/serilog-aspnetcore
            // https://github.com/serilog/serilog/wiki/Getting-Started
            // Serilog.AspNetCore 已依赖 Serilog.Sinks.Console , Serilog.Sinks.File
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext() // 使用 FromLogContext 方法启用默认的上下文信息
                .WriteTo.Console()
                .WriteTo.File($"logs/{nameof(KnifeHub)}.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                Log.Information("Starting web application");

                {
                    #region 启动参数
                    for (var i = 0; i < args.Length; i++)
                    {
                        System.Console.WriteLine($"args[{i}]: {args[i]}");
                    }
                    if (args.Contains("--no-console"))
                    {
                        System.Console.WriteLine("--no-console: hidden current console window");
                        // 获取控制台窗口句柄
                        var handle = GetConsoleWindow();
                        // 隐藏控制台窗口
                        ShowWindow(handle, SW_HIDE);
                    }
                    // 输出当前平台信息
                    Console.WriteLine($"CurrentPlatform: {Utils.OSUtil.PlatformInfo()}");
                    #endregion

                    var builder = WebApplication.CreateBuilder(args);

                    // UseSerilog
                    builder.Host.UseSerilog();

                    // 配置注入
                    ConfigOptions configOptions = builder.Configuration.GetSection(ConfigOptions.Config).Get<ConfigOptions>();
                    builder.Services.Configure<ConfigOptions>(builder.Configuration.GetSection(ConfigOptions.Config));

                    #region Sentry
                    builder.WebHost.UseSentry(o =>
                    {
                        o.Dsn = "https://ed1fa11da1bb47188350fbcfcb6af159@o4504597240741888.ingest.sentry.io/4504597253980160";
                        // When configuring for the first time, to see what the SDK is doing:
                        //o.Debug = true;
                        // Set TracesSampleRate to 1.0 to capture 100% of transactions for performance monitoring.
                        // We recommend adjusting this value in production.
                        o.TracesSampleRate = 1.0;
                    });

                    //Sentry.SentrySdk.CaptureMessage("Hello Sentry");
                    #endregion

                    // Add services to the container.

                    builder.Services.AddControllers();
                    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                    builder.Services.AddEndpointsApiExplorer();

                    builder.Services.AddSwaggerGen(options =>
                    {
                        // 防止相同的 SchemaId 冲突
                        // https://stackoverflow.com/questions/61881770/invalidoperationexception-cant-use-schemaid-the-same-schemaid-is-already-us
                        options.CustomSchemaIds(type => type.ToString());
                    });

                    #region 跨域配置
                    if (configOptions.AllowAllCors)
                    {
                        Console.WriteLine("跨域: AllowAllCors");
                        builder.Services.AddCors(m => m.AddPolicy("AllowAllCors", a => a.AllowAnyOrigin().AllowAnyHeader()));
                    }
                    else
                    {
                        Console.WriteLine("跨域: AllowedCorsOrigins");
                        #region CorsWhiteList
                        var corsWhiteList = configOptions.CorsWhiteList;
                        // 所有允许跨域的 Origin
                        List<string> allAllowedCorsOrigins = new List<string>();
                        if (corsWhiteList != null && corsWhiteList.Count > 0)
                        {
                            foreach (var item in corsWhiteList)
                            {
                                allAllowedCorsOrigins.Add(item);
                            }

                            // 允许 AspNetCoreClient 跨域请求
                            builder.Services.AddCors(options =>
                            {
                                options.AddPolicy(name: "AllowedCorsOrigins",
                                    builder =>
                                    {
                                        // ConfigOptions 里配置的白名单都允许
                                        builder.WithOrigins(allAllowedCorsOrigins.ToArray())

                                                // 解决发送json,复杂请求问题: https://blog.csdn.net/yangyiboshigou/article/details/78738228
                                                // 解决方法: Access-Control-Allow-Headers: Content-Type
                                                // 参考: https://www.cnblogs.com/jpfss/p/10102132.html
                                                .WithHeaders("Content-Type");
                                    });
                            });
                        }
                        #endregion
                    }
                    #endregion

                    builder.Services.AddPluginCore();

                    var app = builder.Build();

                    // Performance monitoring
                    // Enable automatic tracing integration.
                    // If running with .NET 5 or below, make sure to put this middleware
                    // right after `UseRouting()`.
                    app.UseSentryTracing();

                    // Configure the HTTP request pipeline.
                    if (app.Environment.IsDevelopment())
                    {
                        app.UseSwagger();
                        app.UseSwaggerUI();
                    }

                    #region 跨域配置
                    if (configOptions.AllowAllCors)
                    {
                        app.UseCors("AllowAllCors");
                        Console.WriteLine("AllowAllCors");
                    }
                    else
                    {
                        app.UseCors("AllowedCorsOrigins");
                        Console.WriteLine("AllowedCorsOrigins");
                    }
                    #endregion

                    app.UsePluginCore();

                    app.UseAuthorization();

                    // wwwroot
                    app.UseStaticFiles(new StaticFileOptions()
                    {

                    });

                    app.UseDefaultFiles(new DefaultFilesOptions()
                    {
                        DefaultFileNames = new[] { "index.html" }
                    });

                    app.MapControllers();

                    app.Run();
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }

        }
    }
}
