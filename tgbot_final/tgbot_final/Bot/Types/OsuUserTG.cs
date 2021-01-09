using System;

namespace tgbot_final.Bot.Types
{
    class OsuUserTG
    {
        public long id { get; set; }
        public string name { get; set; }
        public DateTimeOffset lastCheckedScore { get; set; }
    }
}
