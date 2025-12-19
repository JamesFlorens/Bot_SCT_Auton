using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
namespace Test.Services
{
    public class BotService
    {
        public ITelegramBotClient? Client { get; private set; }
        private CancellationTokenSource? _cts;
        public void Start(string apiKey, IUpdateHandler handler)
        {
            Client = new TelegramBotClient(apiKey);
            _cts = new CancellationTokenSource();
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[] { UpdateType.Message, UpdateType.CallbackQuery }
            };
            Client.StartReceiving(
                updateHandler: handler,
                receiverOptions: receiverOptions,
                cancellationToken: _cts.Token
            );
        }
        public void Stop()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
        }
    }
}