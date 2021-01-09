using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace tgbot_final.Bot.Utils
{
    class Games
    {
        public static async void bandit(Message mess)
        {
            try
            {
                long bet = Other.getbet(mess);
                if (bet == -1)
                {
                    await BotMain.bot.SendTextMessageAsync(mess.Chat.Id, "Кажется, что у вас не хватает монет, либо ставка слишком велика / неправильный формат ставки");
                    return;
                }
                string name = $"{mess.From.FirstName} {mess.From.LastName}";
                string text = $"{name}:Ставка {bet}\n[🎁][🎁][🎁]";
                string[] emoji = new string[18] { "👻", "🎱", "💥", "🍄", "🦆", "🐴", "🌑", "🤡", "🌕", "🌒", "🌖", "🌓", "🐞", "🐭", "🎢", "⚡️", "💊", "💣" };
                double coff = 1;
                Message bandit = await BotMain.bot.SendTextMessageAsync(mess.Chat.Id, text);
                for (int i = 0; i <= 2; i++)
                {
                    int rnd = RandomNumberGenerator.GetInt32(0, 18);
                    if (emoji[rnd] == "🌑") coff = (coff * 0.5);
                    else if (emoji[rnd] == "🍄") coff = (coff * 0.7);
                    else if (emoji[rnd] == "🌕") coff = (coff * 2);
                    else if (emoji[rnd] == "🌖") coff = (coff * 1.25);
                    else if (emoji[rnd] == "🌒") coff = (coff * 0.75);
                    else if (emoji[rnd] == "🌓") coff = (coff * 1);
                    else if (emoji[rnd] == "💥") coff = (coff * 1.75);
                    else if (emoji[rnd] == "🐞") coff = coff * 1.3;
                    else if (emoji[rnd] == "🤡") coff = coff / 1.5;
                    else if (emoji[rnd] == "🎢") coff = (coff * 0.7);
                    else if (emoji[rnd] == "⚡️")
                    {
                        int lightning = RandomNumberGenerator.GetInt32(1, 31);
                        coff = (coff * ((double)((double)lightning / (double)10)));
                    }
                    else if (emoji[rnd] == "🎱") coff = coff * 2.5;
                    else if (emoji[rnd] == "💣") coff = coff * 0.2;
                    else if (emoji[rnd] == "🐴") coff = (coff * 0.8);
                    else if (emoji[rnd] == "🦆") coff = (coff * 1.15);
                    else if (emoji[rnd] == "🐭") coff = coff * 0.85;
                    else if (emoji[rnd] == "👻") coff = Math.Pow(coff, 1.5);
                    else if (emoji[rnd] == "💊")
                    {
                        int capsule = RandomNumberGenerator.GetInt32(1, 3);
                        if (capsule == 1) coff = coff * 0;
                        else if (capsule == 2) coff = coff * 2;
                    }
                    var index = text.IndexOf("🎁");
                    text = text.Remove(index, 2).Insert(index, emoji[rnd]);
                    if (i == 2)
                    {
                        text += $"\nВы выиграли: {(long)(bet * coff) - bet}";
                        Other.changeBalance(mess.From, (long)(bet * coff) - bet);
                    }
                    await Task.Delay(2000);
                    await BotMain.bot.EditMessageTextAsync(bandit.Chat.Id, bandit.MessageId, text);
                }
            }
            catch (Exception e)
            {
                Other.OnReceivedError(e, mess, $"Произошла ошибка. Напишите \"/report текст проблемы\" в чат, чтобы дать разработчику узнать о проблеме.");
            }
        }
    }
}
