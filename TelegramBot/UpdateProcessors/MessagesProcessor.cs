using Telegram.Bot;
using DataManager.Models;
using DataManager;
using TelegramBot.EventArguments;
using DataManager.Mapping;

namespace TelegramBot.UpdateProcessors;

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

        // Check for chat existing and creating if it is the first call.
        Chat chat = db.Chats.Where(s => s.ChatId == chatId).FirstOrDefault()!;
        if (chat == null)
        {
            chat = new Chat()
            {
                ChatId = chatId,
                Status = (int)ChatStatus.WAIT_COMMAND
            };
            await db.Chats.AddAsync(chat);
        }
        await Console.Out.WriteLineAsync($"Message \"{messageText}\" in processing");

        Task<Telegram.Bot.Types.Message>? tskSendMessage = null;

        // Check for file message
        if (message.Document != null && chat.Status == (int)ChatStatus.WAIT_FILE_SELECTION_LOADING)
        {
            var selection = db.Selections.OrderByDescending(s => s.CreatedAt).First();
            var file = new ChatFile() { ChatFileId = message.Document.FileId, Chat = chat, IsSource = true };
            db.Files.Add(file);
            selection.SourceFile = file;
            chat.Status = (int)ChatStatus.CHOOSE_SELECTION_FIELDS;
            tskSendMessage = _botManager.Client.SendTextMessageAsync(chatId, $"Select fields for selecting data", replyMarkup: _keyboardsManager.GenerateFieldsKeyboard());
            await db.SaveChangesAsync();
            return;
        }

        // Processing text messages
        if (string.IsNullOrEmpty(messageText))
        {
            return;
        }

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
                    chat.Status = (int)ChatStatus.WAIT_COMMAND;
                    break;
                case "/selection":

                    tskSendMessage = _botManager.Client.SendTextMessageAsync(
                        chatId: chatId,
                        text: @"Choose file from the list (recently processed and sample file are available):",
                        replyMarkup: _keyboardsManager.GenerateInlineKeyboardFiles(chat)
                        );
                    chat.Status = (int)ChatStatus.WAIT_FILE_SELECTION_OPTION;


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
