using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace tgbot_final.Bot.Utils
{
    class Tools
    {
        public static async void subscribe(Message mess)
        {
            try
            {
                var find = BotMain.groups.IndexOf(BotMain.groups.FirstOrDefault(m => m.id == mess.Chat.Id));
                BotMain.groups[find].notifyOsu = !BotMain.groups[find].notifyOsu;
                await BotMain.bot.SendTextMessageAsync(mess.Chat.Id, $"{BotMain.groups[find].notifyOsu}");
            }
            catch (Exception e)
            {
                Other.OnReceivedError(e, mess);
            }
        }
        public static async void random(Message mess)
        {
            try
            {
                GroupCollection numbers = new Regex(@"(\d*)-(\d*)").Match(mess.Text).Groups;
                int a = int.Parse(numbers[1].Value); int b = int.Parse(numbers[2].Value);
                await BotMain.bot.SendTextMessageAsync(mess.Chat.Id, $"<i>от {a} до {b}:</i>\n<b>{RandomNumberGenerator.GetInt32(a, b + 1)}</b>", ParseMode.Html);
            }
            catch (Exception e)
            {
                Other.OnReceivedError(e, mess);
                await BotMain.bot.SendTextMessageAsync(mess.Chat.Id, $"<i>от {0} до {100}:</i>\n<b>{RandomNumberGenerator.GetInt32(0, 100 + 1)}</b>", ParseMode.Html);
            }
        }
        public static async void flip(Message mess)
        {
            try
            {
                string name = $"{mess.From.FirstName} {mess.From.LastName}";
                await BotMain.bot.SendTextMessageAsync(mess.Chat.Id, $"{name} подбрасывает монету: <b>{new string[] { "орёл", "решка" }[RandomNumberGenerator.GetInt32(0, 2)]}</b>", ParseMode.Html);
            }
            catch (Exception e)
            {
                Other.OnReceivedError(e, mess);
            }
        }
        public static async void balance(Message mess)
        {
            try
            {
                await BotMain.bot.SendTextMessageAsync(mess.Chat.Id, $"Ваш баланс: {BotMain.users.FirstOrDefault(m => m.id == mess.From.Id).balance:N0} тийин.\n<b>!бонус</b> для получения дополнительных монет.", ParseMode.Html);
            }
            catch (Exception e)
            {
                Other.OnReceivedError(e, mess);
            }
        }
        public static async void top(Message mess)
        {
            try
            {
                BotMain.users = BotMain.users.OrderBy(m => m.balance).ToList();
                string text = "Топ богатых игроков в этой группе:\n"; int i = 1;
                foreach (var item in BotMain.users)
                {
                    Chat chat = await BotMain.bot.GetChatAsync(item.id);
                    string name = $"{chat.FirstName} {chat.LastName}";
                    text += $"<b>{i}</b>. <i>{name}</i> -=<code>{item.balance}</code>=-\n";
                    i += 1;
                }
                await BotMain.bot.SendTextMessageAsync(mess.Chat.Id, text, ParseMode.Html);
            }
            catch (Exception e)
            {
                Other.OnReceivedError(e, mess);
            }
        }
        public static async void report(Message mess)
        {
            try
            {
                await BotMain.bot.SendTextMessageAsync(mess.Chat.Id, "Жалоба была успешно отправлена создателю.");
                await BotMain.bot.ForwardMessageAsync(BotMain.creatorId, mess.Chat.Id, mess.MessageId);
            }
            catch (Exception e)
            {
                Other.OnReceivedError(e, mess);
            }
        }
        public static async void bonusBalance(Message mess)
        {
            try
            {
                if (BotMain.users.FirstOrDefault(m => m.id == mess.From.Id).balance < 500)
                {
                    BotMain.users.FirstOrDefault(m => m.id == mess.From.Id).balance += 500;
                    await BotMain.bot.SendTextMessageAsync(mess.Chat.Id, "Вы получили 500 тийин");
                }
                else
                {
                    await BotMain.bot.SendTextMessageAsync(mess.Chat.Id, "Для получения бонусных монет, нужно иметь меньше, чем 500 тийин");
                }
            }
            catch (Exception e)
            {
                Other.OnReceivedError(e, mess);
            }
        }
        public static async void start(Message mess)
        {
            try
            {
                string text = "Многофункциональный бот, в котором имеются игры, разные приколюхи, а также некоторый функционал для osu!\nВведите <b>/help</b> для получения <i>всех</i> функций бота.";
                await BotMain.bot.SendTextMessageAsync(mess.Chat.Id, text, ParseMode.Html);
            }
            catch (Exception e)
            {
                Other.OnReceivedError(e, mess);
            }
        }
        public static async void help(Message mess)
        {
            try
            {
                string text = "<b>!osu</b> [name] - <i>вывести инфу о вашем профиле</i>\n" +
                    "<b>!rs</b> [count] - <i>вывести count последних скоров</i>\n" +
                    "";
                await BotMain.bot.SendTextMessageAsync(mess.Chat.Id, text, ParseMode.Html);
            }
            catch (Exception e)
            {
                Other.OnReceivedError(e, mess);
            }
        }
        public static async void getListOsuTG(Message mess)
        {
            try
            {
                string text = "";
                for (int i = 0; i <= BotMain.osuUserTGs.Count - 1; i++)
                {
                    text += $"{BotMain.osuUserTGs[i].name}\n{BotMain.osuUserTGs[i].lastCheckedScore}\n\n";
                }
                await BotMain.bot.SendTextMessageAsync(mess.Chat.Id, text);
            }
            catch (Exception e)
            {
                Other.OnReceivedError(e, mess);
            }
        }
        public static async void osuRemove(Message mess)
        {
            try
            {
                string name = string.Join(' ', mess.Text.Split(' ').Skip(1));
                BotMain.osuUserTGs.Remove(BotMain.osuUserTGs.First(m => m.name == name));
                await BotMain.bot.SendTextMessageAsync(mess.Chat.Id, $"{name} удален");
            }
            catch (Exception e)
            {
                Other.OnReceivedError(e, mess);
            }
        }
        public static async void osuAdd(Message mess)
        {
            try
            {
                string name = string.Join(' ', mess.Text.Split(' ').Skip(1));
                if (BotMain.osuUserTGs.FirstOrDefault(m => m.name == name) == default)
                {
                    BotMain.osuUserTGs.Add(new tgbot_final.Bot.Types.OsuUserTG { id = mess.From.Id, name = name });
                }
                await BotMain.bot.SendTextMessageAsync(mess.Chat.Id, $"{name} добавлен");
            }
            catch (Exception e)
            {
                Other.OnReceivedError(e, mess);
            }
        }
        public static async void messageto(Message mess)
        {
            try
            {
                await BotMain.bot.SendTextMessageAsync(long.Parse(mess.Text.Split(' ')[1]), string.Join(' ', mess.Text.Split(' ').Skip(2)));
            }
            catch (Exception e)
            {
                Other.OnReceivedError(e, mess);
            }
        }
            
    }
}
