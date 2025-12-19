using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Polling;
using Test.Infrastructure;

namespace Test.Services
{
    public class TelegramHandlerService
    {
        private readonly Logger _logger;
        private readonly UserActionHandler? _updateHandler;

        public TelegramHandlerService(Logger logger, UserActionHandler? updateHandler)
        {
            _logger = logger;
            _updateHandler = updateHandler;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            if (update.CallbackQuery is { } callbackQuery)
            {
                string data = callbackQuery.Data ?? "";
                if (data.ToLower() != "back" && data.ToLower() != "назад")
                {
                    string user = callbackQuery.From.FirstName ?? "User";
                    long userId = callbackQuery.From.Id;
                    _logger.Log($"🔘 Кнопка: [{data}] от {user} (ID: {userId})");
                }
            }

            if (update.Message is { } message && message.Text is { } messageText)
            {
                if (messageText.ToLower() != "назад" && messageText != "/start")
                {
                    string user = message.From?.FirstName ?? "User";
                    long userId = message.From?.Id ?? 0;
                    _logger.Log($"📩 Запрос: [{messageText}] от {user} (ID: {userId})");
                }
            }

            if (_updateHandler != null)
                await _updateHandler.HandleUpdate(botClient, update);
        }

        public Task HandleErrorAsync(ITelegramBotClient b, Exception e, HandleErrorSource s, CancellationToken ct)
        {
            _logger.Log("⚠️ Ошибка Telegram API: " + e.Message);
            return Task.CompletedTask;
        }
    }
}