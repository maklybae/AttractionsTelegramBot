using DataManager;
using DataManager.Mapping;
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

        Selection selection;
        switch ((ChatStatus)chat.Status)
        {
            case ChatStatus.WAIT_FILE_SELECTION_OPTION:
                selection = new Selection() { Chat =  chat };
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
            case ChatStatus.CHOOSE_SELECTION_FIELDS:
                selection = db.Selections.OrderByDescending(s => s.CreatedAt).First(s => s.Chat.ChatId == chatId);
                var recievedData = args.ReceivedCallback.Data;
                if (recievedData == "RUN")
                {
                    await _botManager.Client.EditMessageTextAsync(chat.ChatId, message.MessageId,
                        $"These fields will be required to select data: {string.Join("; ", db.SelectionParams.Where(s => s.Selection == selection).Select(s => DataField.GetDataField(s.Field)))}",
                        replyMarkup: InlineKeyboardMarkup.Empty());
                }
                else
                {
                    // Выбор поля
                    int receivedField = int.Parse(recievedData!);
                    if (db.SelectionParams.Where(s => s.Selection == selection && s.Field == receivedField).Count() == 0)
                    {
                        var selectionParam = new SelectionParams() { Selection = selection, Field = receivedField };
                        db.SelectionParams.Add(selectionParam);
                        await _botManager.Client.AnswerCallbackQueryAsync(args.ReceivedCallback.Id, $"Field {DataField.GetDataField(receivedField)} marked as selection filter");
                    }
                    else
                    {
                        var param = db.SelectionParams.Where(s => s.Selection == selection && s.Field == receivedField).First();
                        db.SelectionParams.Remove(param);
                        await _botManager.Client.AnswerCallbackQueryAsync(args.ReceivedCallback.Id, $"Field {DataField.GetDataField(receivedField)} unmarked as selection filter");
                    }
                    await db.SaveChangesAsync();
                }
                break;

        }
    }
}
