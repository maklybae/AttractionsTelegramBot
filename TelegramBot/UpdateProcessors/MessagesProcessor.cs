using DataManager.Models;
using DataManager;
using TelegramBot.EventArguments;

namespace TelegramBot.UpdateProcessors;

internal class MessagesProcessor
{
    private readonly StateProcessor _stateProcessor;

    public MessagesProcessor(BotManager botManager)
    {
        botManager.MessageReceived += ProcessReceivevdMessage;
        _stateProcessor = new StateProcessor(botManager);
    }

    public async void ProcessReceivevdMessage(object? sender, MessageReceivedEventArgs args)
    {
        var message = args.ReceivedMessage;
        var messageText = message.Text;
        var chatId = message.Chat.Id;

        // Check for chat existing and creating if it is the first call.
        // Sign up or log in
        using (var db = new DatabaseContext())
        {
            Chat chat = db.Chats.Where(s => s.ChatId == chatId).FirstOrDefault()!;
            if (chat == null)
            {
                chat = new Chat() { ChatId = chatId };
                await db.Chats.AddAsync(chat);
                await db.SaveChangesAsync();
            }
        }
        await Console.Out.WriteLineAsync($"Message \"{messageText}\" in processing");
        
        await _stateProcessor.ProcessRequest(message, null);
    }
}
