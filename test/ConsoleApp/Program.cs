// See https://aka.ms/new-console-template for more information
using Konata.Core.Common;
using Konata.Core.Interfaces;
using Konata.Core.Interfaces.Api;
using System.Text.Json;
using static Konata.Core.Events.Model.CaptchaEvent;

Console.WriteLine("Hello, World!");


// Create a bot instance
Console.Write("QQ: ");
string uin = Console.ReadLine().Trim();
Console.Write("Password: ");
string password = Console.ReadLine();
var botKeystore = new BotKeyStore(uin: uin, password: password);

string logsDir = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
if (!Directory.Exists(logsDir))
{
    Directory.CreateDirectory(logsDir);
}
var bot = BotFather.Create(BotConfig.Default(), BotDevice.Default(), botKeystore);
{
    // Handle the captcha
    bot.OnCaptcha += (bot, e) =>
    {
        if (e.Type == CaptchaType.Slider)
        {
            Console.WriteLine(e.SliderUrl);
            bot.SubmitSliderTicket(Console.ReadLine());
        }
        else if (e.Type == CaptchaType.Sms)
        {
            Console.WriteLine(e.Phone);
            bot.SubmitSmsCode(Console.ReadLine());
        }
    };

    // Print the log
    bot.OnLog += (_, e)
        =>
    {
        string logFilePath = Path.Combine(logsDir, DateTime.Now.ToString("yyyy-MM-dd HH"));
        File.AppendAllLines(logFilePath, new string[] {
            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"),
            e.EventMessage
        }, System.Text.Encoding.UTF8);
    };

    // Handle group messages
    bot.OnGroupMessage += (_, e)
        => Console.WriteLine($"群消息: {e.Message.Chain?.FirstOrDefault()?.ToString() ?? ""}");

    // Handle friend messages
    bot.OnFriendMessage += (_, e)
        => Console.WriteLine($"好友消息: {e.Message.Chain?.FirstOrDefault()?.ToString() ?? ""}");

    // ... More handlers
}

// Do login
if (!await bot.Login())
{
    Console.WriteLine("Login failed");
    return;
}

Console.WriteLine("We got online!");

Console.WriteLine("下方是你的 BotKeyStore.json, 请将它复制好, 用于登录使用");
Console.WriteLine("-------------------------------start-------------------------------------");
string jsonStr = JsonSerializer.Serialize(bot.KeyStore,
               new JsonSerializerOptions { WriteIndented = true });
File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "BotKeyStore.json"), jsonStr, System.Text.Encoding.UTF8);
Console.WriteLine(jsonStr);
Console.WriteLine("-------------------------------end-------------------------------------");


