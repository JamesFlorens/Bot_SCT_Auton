using Telegram.Bot;

namespace Test.Services
{
    public class StatusReporter
    {
        private System.Timers.Timer _timer;
        private readonly BotService _bot;
        private readonly long _adminId = 733157554;
        public StatusReporter(BotService bot)
        {
            _bot = bot;
            _timer = new System.Timers.Timer(1800000); // 30 мин
            _timer.Elapsed += async (s, e) => await SendReport();
            _timer.AutoReset = true;
        }
        public void Start() => _timer.Start();
        public void Stop() => _timer.Stop();
        private async Task SendReport()
        {
            try
            {
                if (_bot?.Client != null)
                    await _bot.Client.SendMessage(_adminId, "🤖 Система стабильна. Бот в сети.");
            }
            catch { }
        }
    }
}