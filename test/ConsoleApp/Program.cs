// See https://aka.ms/new-console-template for more information
using Konata.Core.Common;
using Konata.Core.Interfaces;
using Konata.Core.Interfaces.Api;
using static Konata.Core.Events.Model.CaptchaEvent;

Console.WriteLine("Hello, World!");





// Create a bot instance
Console.Write("QQ: ");
string uin = Console.ReadLine().Trim();
Console.Write("Password: ");
string password = Console.ReadLine();
var botKeystore = new BotKeyStore(uin: uin, password: password);
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
        => Console.WriteLine(e.EventMessage);

    // Handle group messages
    bot.OnGroupMessage += (_, e)
        => Console.WriteLine(e.Message);

    // Handle friend messages
    bot.OnFriendMessage += (_, e)
        => Console.WriteLine(e.Message);

    // ... More handlers
}

// Do login
if (!await bot.Login())
{
    Console.WriteLine("Login failed");
    return;
}

Console.WriteLine("We got online!");


