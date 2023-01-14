using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

namespace QQChannelPlugin.Utils
{
    public class LogUtil
    {
        public const string Sign = nameof(QQChannelPlugin);

        public static void Info(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"{Sign}: ");
            Console.ResetColor();
            DateTime now = DateTime.Now;
            Console.WriteLine(now.ToString("yyyy-MM-dd HH:mm:ss"));
            Console.WriteLine(message);
        }

        public static void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"{Sign}: ");
            Console.ResetColor();
            DateTime now = DateTime.Now;
            Console.WriteLine(now.ToString("yyyy-MM-dd HH:mm:ss"));
            Console.WriteLine(message);
        }

        public static void Exception(Exception ex)
        {
            Error(ex.ToString());
            Exception exception = ex;
            while (exception.InnerException != null)
            {
                exception = ex.InnerException;
                Error(exception.Message);
            }
        }

    }
}
