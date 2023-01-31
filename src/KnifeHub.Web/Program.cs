using PluginCore.AspNetCore.Extensions;
using PluginCore.AspNetCore.lmplements;
using PluginCore.Interfaces;
using PluginCore.lmplements;

var builder = WebApplication.CreateBuilder(args);

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
    // ·ÀÖ¹ÏàÍ¬µÄ SchemaId ³åÍ»
    // https://stackoverflow.com/questions/61881770/invalidoperationexception-cant-use-schemaid-the-same-schemaid-is-already-us
    options.CustomSchemaIds(type => type.ToString());
});

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
