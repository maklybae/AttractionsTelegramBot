using Telegram.Bot;
using DataManager.Models;
using DataManager;
using Telegram.Bot.Types;

namespace TelegramBot;

internal class MessagesProcessor
{
    private readonly BotManager _botManager;

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
        var db = new DatabaseContext();

        Chat chat = db.Chats.Where(s => s.ChatId == chatId).FirstOrDefault() ?? new Chat() 
        { 
            ChatId = chatId, Status = (int)ChatStatus.WAIT_COMMAND 
        };        

        var tskSavetoDb = db.SaveChangesAsync();

        Console.WriteLine($"Message \"{messageText}\" in processing");

        if (string.IsNullOrEmpty(messageText))
            return;

        Task<Message>? tskSendMessage = null;
        if (messageText.StartsWith('/'))
        {
            // Command
            switch (messageText)
            {
                case "/start":
                case "/help":
                    tskSendMessage = _botManager.Client.SendTextMessageAsync(
                        chatId: chatId,
                        text: @"Welcome to telegram bot!");
                    break;
            }
        }

        await tskSavetoDb;
        if (tskSendMessage != null)
        {
            await tskSendMessage;
        }
    }
}
