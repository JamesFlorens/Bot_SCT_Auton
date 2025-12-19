using Telegram.Bot.Types.ReplyMarkups;

namespace Test.Infrastructure
{
    public static class KeyboardFactory
    {
        public static InlineKeyboardMarkup Groups(IEnumerable<string> groups)
        {
            var rows = new List<List<InlineKeyboardButton>>();
            var groupList = groups.ToList();
            for (int i = 0; i < groupList.Count; i += 2)
            {
                var row = groupList.Skip(i).Take(2)
                    .Select(g => InlineKeyboardButton.WithCallbackData($"👥 {g}", g)) 
                    .ToList();
                rows.Add(row);
            }
            rows.Add(new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithUrl("📢 Новости", "https://t.me/news_bot_ckt"),
                InlineKeyboardButton.WithUrl("👨‍💻 Support", "https://t.me/C_Konstant")
            });
            return new InlineKeyboardMarkup(rows);
        }
        public static InlineKeyboardMarkup BackButton()
        {
            return new InlineKeyboardMarkup(new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData("« Назад к выбору групп", "back_to_groups") }
            });
        }
    }
}