using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Test.Models;

namespace Test.Services
{
    public class UpdateHandlerService
    {
        private readonly ScheduleService _scheduleService;
        private readonly Logger _logger;
        public UpdateHandlerService(ScheduleService scheduleService, Logger logger)
        {
            _scheduleService = scheduleService;
            _logger = logger;
        }
        public async Task HandleUpdate(ITelegramBotClient botClient, Update update)
        {
            if (update.Type == UpdateType.Message && update.Message?.Text != null)
            {
                await HandleTextMessage(botClient, update.Message);
            }
            else if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery != null)
            {
                await HandleCallbackQuery(botClient, update.CallbackQuery);
            }
        }
        private async Task HandleTextMessage(ITelegramBotClient botClient, Telegram.Bot.Types.Message message)
        {
            long chatId = message.Chat.Id;
            string text = message.Text ?? "";
            string userInfo = message.From?.FirstName ?? "User";
            if (text == "/start")
            {
                _logger.Log($"👋 {userInfo} запустил бота");
                await botClient.SendMessage(chatId, "Выберите группу или введите номер:",
                    replyMarkup: KeyboardFactory.Groups(ScheduleConfig.AvailableGroups));
                return;
            }
            var foundGroup = GroupHelper.FindGroup(text);
            if (foundGroup != null)
            {
                _logger.Log($"📥 Запрос группы: {foundGroup} от {userInfo}");
                await _scheduleService.SendSchedule(chatId, foundGroup);
            }
        }

        private async Task HandleCallbackQuery(ITelegramBotClient botClient, CallbackQuery query)
        {
            if (query.Message == null) return;
            long chatId = query.Message.Chat.Id;
            string data = query.Data ?? "";
            await botClient.AnswerCallbackQuery(query.Id);
            if (data == "back_to_groups")
            {
                await botClient.SendMessage(chatId, "Выберите группу:",
                    replyMarkup: KeyboardFactory.Groups(ScheduleConfig.AvailableGroups));
            }
            else
            {
                var group = GroupHelper.FindGroup(data);
                if (group != null) await _scheduleService.SendSchedule(chatId, group);
            }
        }
    }
}