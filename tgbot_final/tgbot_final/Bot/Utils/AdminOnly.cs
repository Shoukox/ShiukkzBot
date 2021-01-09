using Newtonsoft.Json;
using System;
using System.Linq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace tgbot_final.Bot.Utils
{
    class AdminOnly
    {
        public static async void deleteMessage(Message mess)
        {
            try
            {
                await BotMain.bot.DeleteMessageAsync(mess.ReplyToMessage.Chat.Id, mess.ReplyToMessage.MessageId);
            }
            catch (Exception e)
            {
                Other.OnReceivedError(e, mess);
            }
        }

        public static async void kickUser(Message mess)
        {
            try
            {
                await BotMain.bot.KickChatMemberAsync(mess.ReplyToMessage.Chat.Id, mess.ReplyToMessage.From.Id);
            }
            catch (Exception e)
            {
                Other.OnReceivedError(e, mess);
            }
        }
        public static async void AllGroups(Message mess)
        {
            try
            {
                string[] ab = mess.Text.Split(" ");
                string text = "";
                if (ab[1] == "all")
                {
                    int count = 0;
                    for (int i = 0; i <= BotMain.groups.Count - 1; i++)
                    {
                        try
                        {
                            count += 1;
                            var t = await BotMain.bot.GetChatAsync(BotMain.groups[i].id);
                            text += $"{count}. {t.Title} [{i}]\n";
                        }
                        catch (Exception)
                        {
                            BotMain.groups.RemoveAt(i);
                            i--;
                        }
                    }
                    await BotMain.bot.SendTextMessageAsync(mess.Chat.Id, text);
                }
                else if (ab[1] == "toall")
                {
                    for (int i = 0; i <= BotMain.groups.Count - 1; i++)
                    {
                        if (BotMain.groups[i].id.ToString().StartsWith("-100"))
                        {
                            try
                            {
                                await BotMain.bot.SendTextMessageAsync(BotMain.groups[i].id, string.Join(" ", ab.Skip(2)));
                            }
                            catch (Exception)
                            {
                                BotMain.groups.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
                else if (ab[1] == "to")
                {
                    try
                    {
                        await BotMain.bot.SendTextMessageAsync(BotMain.groups[int.Parse(ab[2])].id, string.Join(" ", ab.Skip(3)));
                    }
                    catch (Exception)
                    {

                    }
                }
                else if (ab[1] == "replyto")
                {
                    try
                    {
                        Message messreply = mess.ReplyToMessage;
                        string caption = "";
                        if (mess.ReplyToMessage.Photo != null)
                        {
                            if (mess.ReplyToMessage.Caption != null) caption = mess.ReplyToMessage.Caption;
                            await BotMain.bot.SendPhotoAsync(BotMain.groups[int.Parse(ab[2])].id, new InputOnlineFile(mess.ReplyToMessage.Photo[0].FileId), caption: caption);
                        }
                        else if (mess.ReplyToMessage.Sticker != null)
                        {
                            await BotMain.bot.SendStickerAsync(BotMain.groups[int.Parse(ab[2])].id, new InputOnlineFile(mess.ReplyToMessage.Sticker.FileId));
                        }
                        else if (mess.ReplyToMessage.Video != null)
                        {
                            if (mess.ReplyToMessage.Caption != null) caption = mess.ReplyToMessage.Caption;
                            await BotMain.bot.SendVideoAsync(BotMain.groups[int.Parse(ab[2])].id, new InputOnlineFile(mess.ReplyToMessage.Video.FileId), caption: caption);
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
                else if (ab[1] == "fromall")
                {
                    for (int i = 0; i <= BotMain.groups.Count - 1; i++)
                    {
                        if (BotMain.groups[i].id.ToString().StartsWith("-100"))
                        {
                            try
                            {
                                await BotMain.bot.SendTextMessageAsync(BotMain.groups[i].id, string.Join(" ", ab.Skip(2)));
                                await BotMain.bot.LeaveChatAsync(BotMain.groups[i].id);
                                BotMain.groups.RemoveAt(i);
                            }
                            catch (Exception)
                            {
                                BotMain.groups.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }
                else if (ab[1] == "from")
                {
                    try
                    {
                        int ind = int.Parse(ab[2]);
                        if (ab.Length >= 4)
                            await BotMain.bot.SendTextMessageAsync(BotMain.groups[ind].id, string.Join(" ", ab.Skip(3)));
                        await BotMain.bot.LeaveChatAsync(BotMain.groups[ind].id);
                        BotMain.groups.RemoveAt(ind);
                    }
                    catch (Exception)
                    {
                    }
                }
                else if (ab[1] == "remove")
                {
                    BotMain.groups.RemoveAt(int.Parse(ab[2]));
                }
            }
            catch (Exception e)
            {
                Other.OnReceivedError(e, mess, "");
                await BotMain.bot.SendTextMessageAsync(mess.Chat.Id, "/groups\nremove [index] - del from bd\nfrom [ind] [mes] - leave + send mes\nfromall [mes] - leave from all + send mes\nto [ind] [mes] - send mes to group\ntoall [mes] - send mes to all groups\nall - get groups index");
            }

        }
        public static async void getInfo(Message mess)
        {
            try
            {
                await BotMain.bot.SendTextMessageAsync(mess.Chat.Id, JsonConvert.SerializeObject(mess.ReplyToMessage, Formatting.Indented));

            }
            catch (Exception e)
            {
                Other.OnReceivedError(e, mess);
            }
        }
    }
}
