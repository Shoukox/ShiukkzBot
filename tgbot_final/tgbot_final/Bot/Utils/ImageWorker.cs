using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using tgbot_final.Bot.Osu;
using tgbot_final.Bot.Osu.Types;
using tgbot_final.Bot.Osu.Enums;

namespace tgbot_final.Bot.Utils
{
    class ImageWorker
    {
        public static void GetBitmap(string savepath, ImageFormat iform, string name)
        {
            osuApi osuapi = new osuApi(BotMain.osuToken);
            Score[] scores = osuapi.GetTopPlaysByNameAsync(name, 20).Result;
            User user = osuapi.GetUserInfoByNameAsync(name).Result;
            using (WebClient wc = new WebClient())
            {
                wc.DownloadFile($"http://s.ppy.sh/a/{user.user_id}", @"E:\profile.jpg");
            }
            double accuracy = 0, aim = 0, speed = 0;
            foreach (var item in scores)
            {
                double acc = (50 * double.Parse(item.count50) + 100 * double.Parse(item.count100) + 300 * double.Parse(item.count300)) / (300 * (double.Parse(item.countmiss) + double.Parse(item.count50) + double.Parse(item.count100) + double.Parse(item.count300))) * 100;
                Beatmap beatmap = osuapi.GetBeatmapByBeatmapIdAsync(int.Parse(item.beatmap_id), int.Parse(item.enabled_mods)).Result;
                //Console.WriteLine($"{beatmap.title} {beatmap.difficultyrating}");
                aim += double.Parse(beatmap.difficultyrating);
                speed += double.Parse(beatmap.bpm);
            }
            aim = aim / 20;
            speed = speed / 20;
            accuracy = double.Parse(user.accuracy);

            using (Bitmap bitmap = new Bitmap(400, 600))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    using (Image image = Image.FromFile(@"E:\poly.jpg"))
                        graphics.DrawImage(image, 0, 0, 400, 600);
                    using (Image image = Image.FromFile(@"E:\logo.png"))
                        graphics.DrawImage(image, 5, -10, 120, 120);
                    using (Image image = Image.FromFile(@"E:\profile.jpg"))
                        graphics.DrawImage(image, 40, 100, 320, 320);
                    graphics.DrawRectangle(new Pen(new SolidBrush(Color.Black)) { Width = 5 }, new Rectangle() { Location = new Point(40, 100), Size = new Size(320, 320) });
                    graphics.DrawString($"{name.ToUpper()}", new Font("Arial", 25), new SolidBrush(Color.Black), new PointF(105, 30));
                    graphics.DrawString($"Aim: {aim: 0.00}✩", new Font("Arial", 20), new SolidBrush(Color.White), new PointF(50, 450));
                    graphics.DrawString($"Speed: {speed: 0} BPM", new Font("Arial", 20), new SolidBrush(Color.White), new PointF(50, 490));
                    graphics.DrawString($"Accuracy: {accuracy: 0.00}%", new Font("Arial", 20), new SolidBrush(Color.White), new PointF(50, 530));

                }
                bitmap.Save(savepath, iform);
            }
        }
        public static void GetScreenShot(string savepath, ImageFormat iform)
        {
            Bitmap bitmap = new Bitmap(1920, 1080);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(new Point(0, 0), new Point(0, 0), new Size(bitmap.Width, bitmap.Height));
            }
            bitmap.Save(savepath, iform);
        }
    }
}
