// See https://aka.ms/new-console-template for more information
using Konata.Core.Common;
using Konata.Core.Interfaces;
using Konata.Core.Interfaces.Api;
using System.Text.Json;
using static Konata.Core.Events.Model.CaptchaEvent;

Console.WriteLine("KonataPlugin KonataApp-v0.2.0 - QQ登录软件");


// Create a bot instance
Console.Write("输入你的 QQ: ");
string uin = Console.ReadLine().Trim();
Console.Write("输入你的 QQ密码: ");
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
        bool isSuccess = false;
        System.Console.WriteLine();
        if (e.Type == CaptchaType.Slider)
        {
            Console.WriteLine("☆★☆★ 滑动验证");
            Console.WriteLine("请复制下方链接在浏览器打开, 然后如 README.md 中所写获取 ticket, 然后在下方输入");
            System.Console.WriteLine();
            Console.WriteLine(e.SliderUrl);
            System.Console.WriteLine();
            Console.WriteLine("☆★☆★ 粘贴 ticket:");
            isSuccess = bot.SubmitSliderTicket(Console.ReadLine()?.Trim() ?? "");
        }
        else if (e.Type == CaptchaType.Sms)
        {
            Console.WriteLine("☆★☆★ 短信验证");
            System.Console.WriteLine();
            Console.WriteLine(e.Phone);
            System.Console.WriteLine();
            Console.WriteLine("☆★☆★ 输入接收到的验证码:");
            isSuccess = bot.SubmitSmsCode(Console.ReadLine()?.Trim() ?? "");
        }
        System.Console.WriteLine();
        Console.WriteLine($"验证 {(isSuccess ? "通过" : "失败, 请重新验证")}");
        System.Console.WriteLine();
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

    //bot.OnLoginFailed

    // ... More handlers
}

while (!await bot.Login())
{
    Console.WriteLine("登录失败, 自动尝试重新登录并验证, 若为 QQ或密码错误, 请重新运行软件 以便 重新输入QQ及密码");
}

Console.WriteLine("登录成功");

Console.WriteLine("下方是你的 BotKeyStore.json, 请将它复制好, 用于登录使用");
Console.WriteLine("不要复制 start 与 end 两行");
Console.WriteLine("-------------------------------start-------------------------------------");
string jsonStr = JsonSerializer.Serialize(bot.KeyStore,
               new JsonSerializerOptions { WriteIndented = true });
File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "BotKeyStore.json"), jsonStr, System.Text.Encoding.UTF8);
Console.WriteLine(jsonStr);
Console.WriteLine("-------------------------------end-------------------------------------");


