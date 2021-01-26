using System;

namespace tgbot_final.Bot.Logs
{
    class LogsLevels
    {
        private static void ChangeConsoleColor(ConsoleColor consoleColor)
        {
            Console.ForegroundColor = consoleColor;
        }

        private static void LogDate()
        {
            ChangeConsoleColor(ConsoleColor.Cyan);
            Console.Write($"{DateTime.Now} ");
        }
        public static void LogText(string text)
        {
            ChangeConsoleColor(ConsoleColor.White);
            Console.WriteLine(text);
        }
        public static void LogError(string text)
        {
            LogDate();
            ChangeConsoleColor(ConsoleColor.Red);
            Console.WriteLine(text);
        }
        public static void LogMessage(string group, string username, string message)
        {
            LogDate();
            ChangeConsoleColor(ConsoleColor.White);
            Console.Write($"({group}) {username} ");
            ChangeConsoleColor(ConsoleColor.Magenta);
            Console.WriteLine($"{message}");
        }
    }
}
