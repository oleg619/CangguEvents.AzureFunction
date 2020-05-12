using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;

namespace CangguEvents.AzureFunction
{
    public class TelegramMessageHandler
    {
        private readonly TelegramBotClient _telegramClient;
        private const string HeartEmoji = "\U0001F496";

        public TelegramMessageHandler(
            TelegramBotClient telegramClient)
        {
            _telegramClient = telegramClient;
        }

        public Task Handle(Update update) => update.Type switch
        {
            UpdateType.Message => HandleMessage(update.Message),
            UpdateType.InlineQuery => HandleInline(update.InlineQuery),
            _ => Task.CompletedTask
        };

        private async Task HandleMessage(Message message)
        {
            var text = message.Type switch
            {
                MessageType.Text when ShouldSend(message) => GetGuid().ToString(),
                MessageType.ChatMembersAdded => $"Hello {GetUserLink(message.From)}. {Environment.NewLine}" +
                                                $"Guid for you: {GetGuid()}",
                _ => null
            };

            if (text != null)
            {
                await _telegramClient.SendTextMessageAsync(message.Chat.Id, text + Environment.NewLine +
                                                                             HeartEmoji, ParseMode.Markdown);
            }
        }

        private async Task HandleInline(InlineQuery inlineQuery)
        {
            var article = new InlineQueryResultArticle("id1", "Дай гуїд", new InputTextMessageContent(
                $"{GetGuid()}{Environment.NewLine}{HeartEmoji}"));

            await _telegramClient.AnswerInlineQueryAsync(inlineQuery.Id,
                new[]
                {
                    article
                }, 0);
        }

        // Unification for futures, maybe we will want to generate specify guid 
        private static Guid GetGuid() => Guid.NewGuid();

        private static string GetUserLink(User user) => $"[{user.FirstName} {user.LastName}](tg://user?id={user.Id})";

        private static bool ShouldSend(Message message) =>
            GetCommand(message.Text) == "/guid" || message.Text.Equals("Дай гуїд", StringComparison.OrdinalIgnoreCase);

        private static string GetCommand(string message) => message.Split("@")[0];
    }
}