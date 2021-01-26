using System;
using tgbot_final.Bot;

namespace tgbot_final
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            //1328219094:AAFjFsz80--0N_hT1K1-FRXVpkQvyQdGAWA fake
            //1077875912:AAGoe-3mixyIv22Rf7IWHmstIc-Qh8bunb4 real
            string token = "1077875912:AAGoe-3mixyIv22Rf7IWHmstIc-Qh8bunb4";
            while (true)
            {
                BotMain botInstance = new BotMain(token);
                Console.ReadLine();
            }
        }
    }

}
