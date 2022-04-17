using PluginCore.AspNetCore.Extensions;
using PluginCore.AspNetCore.lmplements;
using PluginCore.Interfaces;
using PluginCore.lmplements;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddPluginCore();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UsePluginCore();

app.UseAuthorization();

// wwwroot
app.UseStaticFiles();

#region 确保有 settings.json 文件
QQBotHub.Web.Utils.SettingsUtil.EnsureExist();
#endregion

app.MapControllers();

app.Run();
