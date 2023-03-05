using PluginCore.AspNetCore.Extensions;
using PluginCore.AspNetCore.lmplements;
using PluginCore.Interfaces;
using PluginCore.lmplements;
using KnifeHub.Web.Config;

var builder = WebApplication.CreateBuilder(args);

// 配置注入
ConfigOptions configOptions = builder.Configuration.GetSection(ConfigOptions.Config).Get<ConfigOptions>();
builder.Services.Configure<ConfigOptions>(builder.Configuration.GetSection(ConfigOptions.Config));

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
