using OppaiSharp;
using System;
using System.IO;
using System.Linq;
using System.Net;
using Telegram.Bot.Types;
using tgbot_final.Bot.Types;

namespace tgbot_final.Bot.Utils
{
    class Other
    {
        public static long getbet(Message mess)
        {
            bool hasEnoughMoney(Message mess, long bet)
            {
                if (BotMain.users.FirstOrDefault(m => m.id == mess.From.Id).balance >= bet) return true; else return false;
            }
            long bet = 0;
            try
            {
                bet = long.Parse(mess.Text.Split(" ")[1]);
                if (bet < 0) bet = 0;
            }
            catch (Exception e)
            {
                Other.OnReceivedError(e, mess, "");
                bet = 500;
            }
            if (hasEnoughMoney(mess, bet))
            {
                return bet;
            }
            return -1;
        }
        public static void changeBalance(Telegram.Bot.Types.User user, long changeValue)
        {
            BotMain.users.FirstOrDefault(m => m.id == user.Id).balance += changeValue;
        }
        public static void CheckUser(Message message)
        {
            if (BotMain.users.FirstOrDefault(m => m.id == message.From.Id) == default) BotMain.users.Add(new Types.User { id = message.From.Id, balance = 1000 });
            if (BotMain.groups.FirstOrDefault(m => m.id == message.Chat.Id) == default && message.From.Id != message.Chat.Id) BotMain.groups.Add(new Types.Group { id = message.Chat.Id });
        }
        public static void ConsoleOutput(Message message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"[{message.Date}] ");
            Console.ResetColor();
            Console.Write($"{message.From.FirstName} {message.From.Username} {message.Chat.Title}");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(message.Text);
            Console.ResetColor();
        }
        public static void CheckMessage(Message message)
        {
            if (message.Text.StartsWith("/"))
            {
                if (message.Text.StartsWith("/start")) Tools.start(message);
                else if (message.Text.StartsWith("/help")) Tools.help(message);
                else if (message.Text.StartsWith("/sub")) Tools.subscribe(message);
                else if (message.Text.StartsWith("/list")) Tools.getListOsuTG(message);
                else if (message.Text.StartsWith("/rep")) Tools.report(message);
                else if (message.From.Id == BotMain.creatorId)
                {
                    if (message.Text.StartsWith("/del")) AdminOnly.deleteMessage(message);
                    else if (message.Text.StartsWith("/kick")) AdminOnly.kickUser(message);
                    else if (message.Text.StartsWith("/groups")) AdminOnly.AllGroups(message);
                    else if (message.Text.StartsWith("/get")) AdminOnly.getInfo(message);
                    else if (message.Text.StartsWith("/msgto")) Tools.messageto(message);
                }
            }
            else if (message.Text.StartsWith("!"))
            {
                if (message.Text.StartsWith("!osu")) OsuFuncs.osu(message);
                else if (message.Text.StartsWith("!card")) OsuFuncs.getCard(message);
                else if (message.Text.StartsWith("!add")) Tools.osuAdd(message);
                else if (message.Text.StartsWith("!remove")) Tools.osuRemove(message);
                else if (message.Text.StartsWith("!rs")) OsuFuncs.rs(message);
                else if (message.Text.StartsWith("!score")) OsuFuncs.scores(message);

                else if (message.Text.StartsWith("!roll")) Tools.random(message);
                else if (message.Text.StartsWith("!flip")) Tools.flip(message);
                else if (message.Text.StartsWith("!б")) Tools.balance(message);
                else if (message.Text.StartsWith("!топ")) Tools.top(message);

                else if (message.Text.StartsWith("!банд")) Games.bandit(message);
                else if (message.Text.StartsWith("!бонус")) Tools.bonusBalance(message);

            }
        }
        public static async void OnReceivedError(Exception e, Message mess, string sendtext = "Чет не получилось :(")
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.ToString());
                Console.ResetColor();
                await BotMain.bot.SendTextMessageAsync(BotMain.creatorId, e.ToString());
                if (sendtext != "")
                {
                    await BotMain.bot.SendTextMessageAsync(mess.Chat.Id, sendtext);
                    await BotMain.bot.ForwardMessageAsync(BotMain.creatorId, mess.Chat.Id, mess.MessageId);
                }
            }
            catch (Exception) { }
        }
        public static double ppCalc(Osu.Types.Beatmap beatmap, double accuracy, Mods mods, int misses, int combo)
        {
            byte[] data = new WebClient().DownloadData($"https://osu.ppy.sh/osu/{beatmap.beatmap_id}");
            var stream = new MemoryStream(data, false);
            var reader = new StreamReader(stream);
            var beatmapp = Beatmap.Read(reader);
            var pp = new PPv2(new PPv2Parameters(beatmapp, accuracy: accuracy / 100, misses, combo, mods: mods));
            return pp.Total;
        }
        public static void saveInfo()
        {
            using (StreamWriter sw = new StreamWriter(@"E:\bot\osutg.txt", false))
            {
                foreach (var item in BotMain.osuUserTGs)
                {
                    sw.WriteLine($"\"{item.id}\"===\"{item.name}\"===\"{item.lastCheckedScore}\"");
                }
            }
            using (StreamWriter sw = new StreamWriter(@"E:\bot\groups.txt", false))
            {
                foreach (var item in BotMain.groups)
                {
                    sw.WriteLine($"\"{item.id}\"===\"{item.notifyOsu}\"");
                }
            }
        }
        public static void getInfo()
        {
            using (StreamReader sr = new StreamReader(@"E:\bot\osutg.txt"))
            {
                while (!sr.EndOfStream)
                {
                    string[] ab = sr.ReadLine().Split("===").Select(m => m.Substring(1, m.Length - 2)).ToArray();
                    long id = long.Parse(ab[0]);
                    string name = ab[1];
                    DateTimeOffset dateTimeOffset = default;
                    bool lastCheckedScore = DateTimeOffset.TryParse(ab[2], out dateTimeOffset);
                    BotMain.osuUserTGs.Add(new OsuUserTG { id = id, name = name, lastCheckedScore = dateTimeOffset });
                }
            }
            using (StreamReader sr = new StreamReader(@"E:\bot\groups.txt"))
            {
                while (!sr.EndOfStream)
                {
                    string[] ab = sr.ReadLine().Split("===").Select(m => m.Substring(1, m.Length - 2)).ToArray();
                    long id = long.Parse(ab[0]);
                    bool notifyOsu = bool.Parse(ab[1]);
                    BotMain.groups.Add(new Group { id = id, notifyOsu = notifyOsu });
                }
            }
        }
    }
}
