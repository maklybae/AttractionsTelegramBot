using DataManager.Models;
using DataManager;
using TelegramBot.EventArguments;

namespace TelegramBot.UpdateProcessors;

/// <summary>
/// Represents a processor for handling received messages.
/// </summary>
internal class MessagesProcessor
{
    private readonly StateProcessor _stateProcessor;

    /// <summary>
    /// Initializes a new instance of the MessagesProcessor class with the specified bot manager and state processor.
    /// </summary>
    /// <param name="botManager">The bot manager responsible for handling bot events.</param>
    /// <param name="stateProcessor">The state processor responsible for processing bot states.</param>
    public MessagesProcessor(BotManager botManager, StateProcessor stateProcessor)
    {
        botManager.MessageReceived += ProcessReceivevdMessage;
        _stateProcessor = stateProcessor;
    }

    /// <summary>
    /// Processes the received message.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="args">The event arguments containing the received message.</param>
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
        await _stateProcessor.ProcessRequest(message, null);
    }
}
