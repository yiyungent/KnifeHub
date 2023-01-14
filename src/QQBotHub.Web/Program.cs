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
    // ·ÀÖ¹ÏàÍ¬µÄ SchemaId ³åÍ»
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
app.UseStaticFiles(new StaticFileOptions()
{

});

app.UseDefaultFiles(new DefaultFilesOptions()
{
    DefaultFileNames = new[] { "index.html" }
});

app.MapControllers();

app.Run();
