using OppaiSharp;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using tgbot_final.Bot.Osu;
using tgbot_final.Bot.Osu.Types;
using Beatmap = tgbot_final.Bot.Osu.Types.Beatmap;
using User = tgbot_final.Bot.Osu.Types.User;

namespace tgbot_final.Bot.Utils
{
    class OsuFuncs
    {
        public static void notifyFunc()
        {
            while (true)
            {
                try
                {
                    //Console.WriteLine($"isTimerWorking: {BotMain.isTimerWorking}");
                    if (BotMain.isTimerWorking) continue;
                    BotMain.isTimerWorking = true;
                    //Console.WriteLine("timed");
                    for (int i = 0; i <= BotMain.osuUserTGs.Count - 1; i++)
                    {
                        //Console.WriteLine($"[Timer] {BotMain.osuUserTGs[i].name}");
                        osuApi osuApi = new osuApi(BotMain.osuToken);
                        Score[] recentScores = osuApi.GetRecentScoresByNameAsync(BotMain.osuUserTGs[i].name, 100).Result;
                        //Console.WriteLine($"[Timer] osuApi + recentscores ({osuUserTGs[i].name})");
                        if (recentScores == null)
                        {
                            continue;
                        }
                        int start = recentScores.Length - 1;
                        bool startgot = false;
                        for (int q = recentScores.Length - 1; q >= 0; q--)
                        {
                            if (DateTimeOffset.Parse(recentScores[q].date) > BotMain.osuUserTGs[i].lastCheckedScore)
                            {
                                start = q;
                                startgot = true;
                                break;
                            }
                        }
                        //Console.WriteLine($"[Timer] get start var ({osuUserTGs[i].name})");
                        if (!startgot) continue;
                        for (int j = start; j >= 0; j--)
                        {
                            double accuracy = (50 * double.Parse(recentScores[j].count50) + 100 * double.Parse(recentScores[j].count100) + 300 * double.Parse(recentScores[j].count300)) / (300 * (double.Parse(recentScores[j].countmiss) + double.Parse(recentScores[j].count50) + double.Parse(recentScores[j].count100) + double.Parse(recentScores[j].count300))) * 100;
                            Beatmap beatmap = osuApi.GetBeatmapByBeatmapIdAsync(int.Parse(recentScores[j].beatmap_id)).Result;
                            if (beatmap == null) continue;
                            //Console.WriteLine($"[Timer] getting pp ({osuUserTGs[i].name})");
                            double curpp = Other.ppCalc(beatmap, accuracy, (Mods)osuApi.CalculateModsMods(int.Parse(recentScores[j].enabled_mods)), int.Parse(recentScores[j].countmiss), int.Parse(recentScores[j].maxcombo));
                            double ifFCpp = Other.ppCalc(beatmap, accuracy, (Mods)osuApi.CalculateModsMods(int.Parse(recentScores[j].enabled_mods)), 0, int.Parse(beatmap.max_combo));
                            //Console.WriteLine($"[Timer] got pp ({osuUserTGs[i].name})");
                            //Console.WriteLine($"{osuUserTGs[i].name} {curpp} {ifFCpp}");
                            Mods mods = (Mods)osuApi.CalculateModsMods(int.Parse(recentScores[j].enabled_mods));
                            if (curpp > 200 && recentScores[j].rank != "F")
                            {

                                foreach (var item in BotMain.groups)
                                {
                                    if (item.notifyOsu)
                                    {
                                        BotMain.bot.SendTextMessageAsync(item.id, $"<b><u>{BotMain.osuUserTGs[i].name}</u></b> недавно прошел данную карту, набрав <i>{curpp:N0}pp</i>!\n" +
                                            $"<b>({recentScores[j].rank})</b> <a href=\"https://osu.ppy.sh/beatmaps/{beatmap.beatmap_id}\">{beatmap.title} [{beatmap.version}]</a> <b>{osuApi.GetBeatmapByBeatmapIdAsync(int.Parse(beatmap.beatmap_id)).Result.GetApproved()}</b>\n" +
                                            $"{recentScores[j].count300}-{recentScores[j].count100}-{recentScores[j].count50}-{recentScores[j].countmiss}x❌ - <b><i>{accuracy:N2}%</i></b>\n" +
                                            $"<b>{mods}</b> <i>{recentScores[j].maxcombo}/{beatmap.max_combo}</i> <b><u>{curpp:N0}pp</u></b> (<b><u>{ifFCpp:N0}pp</u></b> if FC)\n" +
                                            $"{DateTimeOffset.Parse(recentScores[j].date).AddHours(5):dd.MM.yyyy hh:mm}", ParseMode.Html, disableNotification: true).Wait();
                                        BotMain.osuUserTGs[i].lastCheckedScore = DateTimeOffset.Parse(recentScores[j].date);
                                    }
                                }
                            }

                        }
                    }

                }
                catch (Exception ex)
                {
                    Other.OnReceivedError(ex, null, "");
                }
                Console.WriteLine("[Timer] end of timer");
                BotMain.isTimerWorking = false;
            }

        }
        public static async void scores(Message mess)
        {
            try
            {
                osuApi osuapi = new osuApi(BotMain.osuToken);
                string beatmap_id = "";
                for (int i = mess.Text.Length - 1; i >= 0; i--)
                {
                    if (mess.Text[i] != '/') beatmap_id = beatmap_id.Insert(0, mess.Text[i].ToString()); else break;
                }
                string name = string.Join(" ", mess.Text.Split(" ").Skip(1).SkipLast(1));
                Score[] scores = await osuapi.GetScoresOnMapByName(name, long.Parse(beatmap_id));
                string text = "";
                foreach (var item in scores)
                {
                    Mods mods = (Mods)osuapi.CalculateModsMods(int.Parse(item.enabled_mods));
                    double accuracy = (50 * double.Parse(item.count50) + 100 * double.Parse(item.count100) + 300 * double.Parse(item.count300)) / (300 * (double.Parse(item.countmiss) + double.Parse(item.count50) + double.Parse(item.count100) + double.Parse(item.count300))) * 100;
                    Beatmap beatmap = await osuapi.GetBeatmapByBeatmapIdAsync(int.Parse(beatmap_id));
                    double curpp = Other.ppCalc(beatmap, accuracy, mods, int.Parse(item.countmiss), int.Parse(item.maxcombo));
                    double IfFCpp = Other.ppCalc(beatmap, accuracy, mods, 0, int.Parse(beatmap.max_combo));
                    text += $"<b>({item.rank})</b> <a href=\"https://osu.ppy.sh/beatmaps/{item.beatmap_id}\">{beatmap.title} [{beatmap.version}]</a> <b>({beatmap.GetApproved()})</b>\n" +
                        $"{item.count300}-{item.count100}-{item.count50}-{item.countmiss}❌ - <b><i>{accuracy:N2}</i></b>%\n" +
                        $"<b>{mods}</b> <i>{item.maxcombo}/{beatmap.max_combo}</i> <b><u>{curpp:N0}pp</u></b> (<b><u>~{IfFCpp:N0}pp</u></b> if FC)\n({DateTimeOffset.Parse(item.date).AddHours(5):dd.MM.yyyy HH:mm})\n\n";
                }
                await BotMain.bot.SendTextMessageAsync(mess.Chat.Id, text, ParseMode.Html);

            }
            catch (Exception e)
            {
                Other.OnReceivedError(e, mess, "Нет скоров");
            }
        }

        public static async void getCard(Message mess)
        {
            try
            {
                string name = string.Join(' ', mess.Text.Split(' ').Skip(1));
                ImageWorker.GetBitmap($@"E:\{name}.png", ImageFormat.Png, name);
                InputOnlineFile card = new InputOnlineFile(new MemoryStream(System.IO.File.ReadAllBytes($@"E:\{name}.png")));
                await BotMain.bot.SendPhotoAsync(mess.Chat.Id, card);
            }
            catch (Exception e)
            {
                Other.OnReceivedError(e, mess);
            }
        }
        public static async void osu(Message mess)
        {
            try
            {
                osuApi osuApi = new osuApi(BotMain.osuToken);
                string name = string.Join(' ', mess.Text.Split(' ').Skip(1));
                if (name == "")
                {
                    await BotMain.bot.SendTextMessageAsync(mess.Chat.Id, "а ты кто?\n!osu [name]");
                    return;
                }
                User user = await osuApi.GetUserInfoByNameAsync(name);
                string text = $"<b>{name}'s profile!\n\nMain Information:</b>\n<code>join_date: {user.join_date:G}\ncountry: {user.country}" +
    $"\npp: {Convert.ToDouble(user.pp_raw):N0}pp\naccuracy: {double.Parse(user.accuracy):N2}%\nglobal_rank: #{user.pp_rank}" +
    $"\ncountry_rank: #{user.pp_country_rank}\nplaycount: {user.playcount}\nlevel: {double.Parse(user.level):N0}</code>";
                int count = 0;
                text += $"\n\n<b>Top Plays:</b>\n<code>";
                Score[] topscores = await osuApi.GetTopPlaysByNameAsync(name);

                foreach (var item in topscores)
                {
                    count++;
                    Mods mods = (Mods)osuApi.CalculateModsMods(int.Parse(item.enabled_mods));
                    double accuracy = (50 * double.Parse(item.count50) + 100 * double.Parse(item.count100) + 300 * double.Parse(item.count300)) / (300 * (double.Parse(item.countmiss) + double.Parse(item.count50) + double.Parse(item.count100) + double.Parse(item.count300))) * 100;
                    Beatmap curBeatmap = await osuApi.GetBeatmapByBeatmapIdAsync(int.Parse(item.beatmap_id));
                    text += $"{count}. </code><b>({item.rank})</b><a href=\"https://osu.ppy.sh/beatmaps/{curBeatmap.beatmap_id}\">{curBeatmap.title} [{curBeatmap.version}]</a><code>\n {item.count300}-{item.count100}-{item.count50}-{item.countmiss}❌ - {accuracy:N2}%" +
                        $"\n <b>{mods}</b> <i>{item.maxcombo}/{curBeatmap.max_combo}</i> <b><u>{item.pp:N0}pp</u></b>\n{DateTimeOffset.Parse(item.date).AddHours(5):dd.MM.yyyy HH:mm}\n\n";
                    if (count == 5) break;
                }
                text += "</code>";

                await BotMain.bot.SendTextMessageAsync(mess.Chat.Id, text, ParseMode.Html);
            }
            catch (Exception e)
            {
                Other.OnReceivedError(e, mess);
            }
        }
        public static async void rs(Message mess)
        {
            try
            {
                string text = "";
                osuApi osuApi = new osuApi(BotMain.osuToken);
                string name = string.Join(' ', mess.Text.Split(' ').Skip(1));
                if (name == "")
                {
                    await BotMain.bot.SendTextMessageAsync(mess.Chat.Id, "а ты кто?\n!rs [name]");
                    return;
                }
                Score recentScore = (await osuApi.GetRecentScoresByNameAsync(name))[0];
                if (recentScore == default) throw new FormatException();
                Beatmap beatmap = await osuApi.GetBeatmapByBeatmapIdAsync(long.Parse(recentScore.beatmap_id));
                Mods mods = (Mods)osuApi.CalculateModsMods(int.Parse(recentScore.enabled_mods));
                double accuracy = (50 * double.Parse(recentScore.count50) + 100 * double.Parse(recentScore.count100) + 300 * double.Parse(recentScore.count300)) / (300 * (double.Parse(recentScore.countmiss) + double.Parse(recentScore.count50) + double.Parse(recentScore.count100) + double.Parse(recentScore.count300))) * 100;
                double curpp = Other.ppCalc(beatmap, accuracy, mods, int.Parse(recentScore.countmiss), int.Parse(recentScore.maxcombo));
                double ppIFfc = Other.ppCalc(beatmap, accuracy, mods, 0, int.Parse(beatmap.max_combo));
                text += $"<b>({recentScore.rank})</b> <a href=\"https://osu.ppy.sh/beatmaps/{beatmap.beatmap_id}\">{beatmap.title} [{beatmap.version}]</a> <b>({beatmap.GetApproved()})</b>\n" +
                    $"{recentScore.count300}-{recentScore.count100}-{recentScore.count50}-{recentScore.countmiss}❌ - <b><i>{accuracy:N2}</i></b>%\n" +
                    $"<b>{mods}</b> <i>{recentScore.maxcombo}/{beatmap.max_combo}</i> <b><u>{curpp:N0}pp</u></b> (<b><u>~{ppIFfc:N0}pp</u></b> if FC)\n({DateTimeOffset.Parse(recentScore.date).AddHours(5):dd.MM.yyyy HH:mm})\n\n";
                await BotMain.bot.SendTextMessageAsync(mess.Chat.Id, text, ParseMode.Html);
            }
            catch (FormatException e)
            {
                Other.OnReceivedError(e, mess, "Он ничего не играл последние 24 часа.");
            }
            catch (Exception e)
            {
                Other.OnReceivedError(e, mess, "Чет не получилось :(");
            }
        }
    }
}
