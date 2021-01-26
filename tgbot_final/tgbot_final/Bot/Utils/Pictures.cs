using BooruSharp.Booru;
using System;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using tgbot_final.Bot.Logs;

namespace tgbot_final.Bot.Utils
{
    class Pictures
    {
        public async static void SendPicture(Message mess, string search)
        {
            try
            {
                DanbooruDonmai booru = new DanbooruDonmai();
                BooruSharp.Search.Post.SearchResult result = await booru.GetRandomPostAsync(search);

                await BotMain.bot.SendPhotoAsync(mess.Chat.Id, new InputOnlineFile(result.FileUrl), $"Source: {result.FileUrl}");
            }
            catch (Exception e)
            {
                LogsLevels.LogError(e.ToString());
                SendPicture(mess, search);
            }
        }
    }
}
