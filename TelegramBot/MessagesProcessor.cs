using Telegram.Bot;
using DataManager.Models;
using DataManager;

namespace TelegramBot;

internal class MessagesProcessor
{
    private readonly BotManager _botManager;
    private readonly KeyboardsManager _keyboardsManager = new KeyboardsManager();

    public MessagesProcessor(BotManager botManager)
    {
        botManager.MessageReceived += ProcessReceivevdMessage;
        _botManager = botManager;
    }

    public async void ProcessReceivevdMessage(object? sender, MessageReceivedEventArgs args)
    {
        var message = args.ReceivedMessage;
        var messageText = message.Text;
        var chatId = message.Chat.Id;
        using var db = new DatabaseContext();

        Chat chat = db.Chats.Where(s => s.ChatId == chatId).FirstOrDefault()!;
        if (chat == null)
        {
            chat = new Chat()
            {
                 ChatId = chatId,
                 Status = (int)ChatStatus.WAIT_COMMAND
            };
            await db.Chats.AddAsync(chat);
            Console.WriteLine("hello");
        }
        await Console.Out.WriteLineAsync($"Message \"{messageText}\" in processing");

        if (string.IsNullOrEmpty(messageText))
            return;

        Task<Telegram.Bot.Types.Message>? tskSendMessage = null;
        if (messageText.StartsWith('/'))
        {
            // Command
            switch (messageText)
            {
                case "/start":
                case "/help":
                    tskSendMessage = _botManager.Client.SendTextMessageAsync(
                        chatId: chatId,
                        text: @"Welcome to telegram bot!"
                        );
                    break;
                case "/selection":

                    tskSendMessage = _botManager.Client.SendTextMessageAsync(
                        chatId: chatId,
                        text: @"Choose file from the list (recently processed and sample file are available):",
                        replyMarkup: _keyboardsManager.GenerateInlineKeyboardFiles(chat)
                        );

                    break;
                default:
                    await Console.Out.WriteLineAsync($"Unknown message \"{messageText}\"");
                    break;
            }
        }

        await db.SaveChangesAsync();
        if (tskSendMessage != null)
        {
            await tskSendMessage;
        }
    }
}
