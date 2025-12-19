using Telegram.Bot;
using System;
using System.Threading.Tasks;
using Test.Infrastructure;

namespace Test.Services
{
    public class StatusReporter
    {
        private System.Timers.Timer _timer;
        private readonly TelegramBotEngine _bot;
        private readonly Logger _logger;
        private readonly long _adminId = 733157554;
        public StatusReporter(TelegramBotEngine bot, Logger logger)
        {
            _bot = bot;
            _logger = logger;
            _timer = new System.Timers.Timer(1800000);
            _timer.Elapsed += async (s, e) => await SendReport();
            _timer.AutoReset = true;
        }
        public void Start()
        {
            _timer.Start();
            _logger.Log("📊 Мониторинг запущен: отчеты в ТГ каждые 30 минут.");
        }
        public void Stop() => _timer.Stop();
        private async Task SendReport()
        {
            try
            {
                if (_bot?.Client != null)
                {
                    await _bot.Client.SendTextMessageAsync(_adminId, "🤖 Система стабильна. Бот в сети.");
                    _logger.Log("📨 Отправлен отчет о стабильности в Telegram администратору.");
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"⚠️ Не удалось отправить отчет в ТГ: {ex.Message}");
            }
        }

        public async Task SendInstantNotification(string message)
        {
            try
            {
                if (_bot?.Client != null)
                {
                    await _bot.Client.SendTextMessageAsync(_adminId, $"🔔 [СОБЫТИЕ]: {message}");
                    _logger.Log($"📨 Уведомление отправлено в ТГ: {message}");
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"⚠️ Ошибка ТГ при отправке уведомления: {ex.Message}");
            }
        }
    }
}