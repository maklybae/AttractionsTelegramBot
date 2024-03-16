using DataManager;
using DataManager.Models;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.EventArguments;

namespace TelegramBot.UpdateProcessors;

internal class CallbackQueryProcessor
{
    private readonly BotManager _botManager;
    
    public CallbackQueryProcessor(BotManager botManager)
    {
        botManager.CallbackQueryReceived += ProcessCallbackQuery;
        _botManager = botManager;
    }

    public async void ProcessCallbackQuery(object? sender, CallbackQueryReceivedEventArgs args)
    {
        var message = args.ReceivedCallback.Message;
        var chatId = message!.Chat.Id;
        var db = new DatabaseContext();
        var messageText = message.Text;

        Chat chat = db.Chats.Where(s => s.ChatId == chatId).FirstOrDefault()!;
        if (chat == null)
        {
            return;
        }

        switch ((ChatStatus)chat.Status)
        {
            case ChatStatus.WAIT_FILE_SELECTION_OPTION:
                var selection = new Selection() { Chat =  chat };
                switch (args.ReceivedCallback.Data)
                {
                    case "SAMPLE":
                        break;
                    case "LOAD":
                        chat.Status = (int)ChatStatus.WAIT_FILE_SELECTION_LOADING;
                        await _botManager.Client.EditMessageTextAsync(chat.ChatId, message.MessageId, "Load .csv or .json file: ", replyMarkup: InlineKeyboardMarkup.Empty());
                        break;
                    default:
                        var sourceFile = db.Files.Where(s => s.ChatFileId == args.ReceivedCallback.Data).FirstOrDefault()!;
                        selection.SourceFile = sourceFile;
                        await _botManager.Client.EditMessageTextAsync(chat.ChatId, message.MessageId, $"Selected revision: {sourceFile.Description}", replyMarkup: InlineKeyboardMarkup.Empty());
                        break;
                }
                await db.Selections.AddAsync(selection);
                await db.SaveChangesAsync();
                break;
        }
    }
}
