using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Telegram.Bot;
using Telegram.Bot.Types;
using tgbot_final.Bot.Logs;
using tgbot_final.Bot.Types;
using tgbot_final.Bot.Utils;
using Group = tgbot_final.Bot.Types.Group;
using User = tgbot_final.Bot.Types.User;


namespace tgbot_final.Bot
{
    class BotMain
    {
        public static TelegramBotClient bot { get; set; }
        public static long creatorId = 728384906;
        public static string botUsername { get; set; }
        public static List<OsuUserTG> osuUserTGs = new List<OsuUserTG>();
        public static List<User> users = new List<User>();
        public static List<Group> groups = new List<Group>();
        public static Dictionary<string, string> commands = new Dictionary<string, string>() { { "/anime", "rating:s" }, { "/loli", "loli" }, { "/ecchi", "rating:q" },
                                                            { "/yuri", "yuri" }, { "/hentai","rating:e" }, {"/uncensored", "uncensored"}, {"/neko", "cat_ears"},{ "/wallpaper", "wallpaper"} };
        public static string osuToken = "67368ae869a6b45f012b6a7a8536ee65226ad257";
        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);
        public delegate bool HandlerRoutine(CtrlTypes CtrlType);

        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }

        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            Other.saveInfo();
            return true;
        }
        public BotMain(string token)
        {
            SetConsoleCtrlHandler(new HandlerRoutine(ConsoleCtrlCheck), true);
            Other.getInfo();
            bot = new TelegramBotClient(token);
            botUsername = "@"+bot.GetMeAsync().Result.Username.ToLower();
            bot.GetUpdatesAsync(-1);
            bot.StartReceiving();
            OnEvents();
            Console.ReadLine();
            bot.StopReceiving();
        }
        public void OnEvents()
        {
            bot.OnMessage += Bot_OnMessage;
            OsuFuncs.notifyFunc();
        }
        private void Bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            Message message = e.Message;
            if (message.Text != null)
            {
                message.Text = message.Text.ToLower();
                LogsLevels.LogMessage(message.Chat.Title, message.From.Username, message.Text);
                Other.CheckUser(message);
                Other.CheckMessage(message);
            }
        }

    }
}
