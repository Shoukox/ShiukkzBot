using System;
using tgbot_final.Bot;

namespace tgbot_final
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            string token = "YOUR_TOKEN";
            while (true)
            {
                BotMain botInstance = new BotMain(token);
                Console.ReadLine();
            }
        }
    }

}
