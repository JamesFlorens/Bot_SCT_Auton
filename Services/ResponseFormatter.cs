using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Test.Data;
using Test.Infrastructure;

namespace Test.Services
{
    public class ResponseFormatter
    {
        private readonly ITelegramBotClient _bot;
        private readonly Logger _logger;
        private readonly SqliteDataManager _db;
        private FileSystemWatcher _watcher;
        public ResponseFormatter(ITelegramBotClient bot, Logger logger)
        {
            _bot = bot;
            _logger = logger;
            _db = new SqliteDataManager(logger);
        }

        public void WatchFile(string filePath)
        {
            if (!File.Exists(filePath)) return;
            if (_watcher != null) { _watcher.Dispose(); }
            string directory = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileName(filePath);
            _watcher = new FileSystemWatcher(directory, fileName);
            _watcher.NotifyFilter = NotifyFilters.LastWrite;
            _watcher.Changed += (s, e) => {
                _logger.Log("Файл Excel изменен! Авто-обновление базы...");
                Thread.Sleep(1500);
                _db.ImportFromExcel(filePath);
            };
            _watcher.EnableRaisingEvents = true;
            _db.ImportFromExcel(filePath);
        }

        public async Task SendSchedule(long chatId, string group)
        {
            var data = _db.GetLessons(group);
            if (data == null || data.Count == 0) return;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"🎓 <b>ГРУППА: {group}</b>");
            sb.AppendLine($"🕒 <code>Обновлено: {DateTime.Now:HH:mm}</code>");
            sb.AppendLine("⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯");
            string lastDay = "";
            foreach (var row in data)
            {
                string rawDay = row[0].Trim(); 
                if (lastDay != rawDay)
                {
                    if (!string.IsNullOrEmpty(lastDay)) sb.AppendLine();
                    lastDay = rawDay;
                    int spaceIndex = rawDay.IndexOf(' ');
                    if (spaceIndex != -1)
                    {
                        string dayName = rawDay.Substring(0, spaceIndex).Trim();
                        string datePart = rawDay.Substring(spaceIndex).Trim();
                        sb.AppendLine($"📅 <b>{dayName.ToUpper()}</b> {datePart}");
                    }
                    else
                    {
                        sb.AppendLine($"📅 <b>{rawDay.ToUpper()}</b>");
                    }
                    sb.AppendLine();
                }
                string pairEmoji = row[1] switch
                {
                    "1" => "1️⃣",
                    "2" => "2️⃣",
                    "3" => "3️⃣",
                    "4" => "4️⃣",
                    "5" => "5️⃣",
                    _ => "🔹"
                };
                sb.AppendLine($"{pairEmoji} <b>{row[2]}</b>");
                sb.AppendLine($"      ┗ 📍 Кабинет: <code>{row[3]}</code>");
                sb.AppendLine();
            }
            sb.AppendLine("⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯");
            string[] quotes = {
                "Дорогу осилит идущий. 🐾",
                "Знание — сила, а выспаться — бесценно. ✨",
                "Твой мозг — это мышца. 💪",
                "Ученье — свет, а неученье — на работу к восьми. ☀️",
                "Гранит науки твердый, но ты сильнее! 🦷"
            };
            Random rnd = new Random();
            sb.AppendLine($"<i>{quotes[rnd.Next(quotes.Length)]}</i>");
            var kb = new InlineKeyboardMarkup(new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData("« Назад к выбору", "back_to_groups") }
            });
            await _bot.SendTextMessageAsync( 
                chatId: chatId,
                text: sb.ToString(),
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: kb
            );
        }

    }
}