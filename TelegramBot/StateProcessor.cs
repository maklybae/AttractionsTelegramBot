using DataManager;
using DataManager.Mapping;
using DataManager.Models;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot;
using Models.DataFormatProcessors;
using Models;

namespace TelegramBot;

internal class StateProcessor
{
    private readonly BotManager _botManager;
    private readonly FileManager _fileManager;
    private readonly KeyboardsManager _keyboardsManager = new();

    public StateProcessor(BotManager botManager)
    {
        _botManager = botManager;
        _fileManager = new FileManager(botManager);
    }

    public async Task ProcessRequest(Telegram.Bot.Types.Message message, Telegram.Bot.Types.CallbackQuery? callbackQuery = null)
    {
        // Get currentState from chat info
        ChatStatus currentState;
        Chat chat;
        using (var db = new DatabaseContext())
        {
            chat = db.Chats.Where(s => s.ChatId == message.Chat.Id).First();
            currentState = (ChatStatus)chat.Status;
        }

        if (await TryCommand(message, callbackQuery))
            return;
        try
        {
            switch (currentState)
            {
                case ChatStatus.WAIT_COMMAND:
                    break;
                case ChatStatus.WAIT_FILE_SELECTION_OPTION:
                    await WaitFileSelectionOption(message, callbackQuery); break;
                case ChatStatus.WAIT_FILE_SELECTION_LOADING:
                    await WaitFileSelectionLoading(message, callbackQuery); break;
                case ChatStatus.CHOOSE_SELECTION_FIELDS:
                    await ChooseSelectionFields(message, callbackQuery); break;
                case ChatStatus.CHOOSE_SELECTION_PARAMS:
                    await ChooseSelectionParams(message, callbackQuery); break;
                case ChatStatus.WAIT_SELECTION_SAVING_TYPE:
                    await WaitSelectionFileType(message, callbackQuery); break;
                case ChatStatus.WAIT_FILE_SORTING_OPTION:
                    await WaitFileSortingOption(message, callbackQuery); break;
                case ChatStatus.WAIT_FILE_SORTING_LOADING:
                    await WaitFileSortingLoading(message, callbackQuery); break;
                case ChatStatus.CHOOSE_SORTING_FIELDS:
                    await ChooseSortingFields(message, callbackQuery); break;
                case ChatStatus.CHOOSE_SORTING_PARAMS:
                    await ChooseSortingParams(message, callbackQuery); break;
                case ChatStatus.WAIT_SORTING_SAVING_TYPE:
                    await WaitSortingSavingType(message, callbackQuery); break;
            }
        }
        catch (Exception e)
        {
            await Console.Out.WriteLineAsync($"Error during state processing occured: {e.Message}");
        }
    }

    private async Task<bool> TryCommand(Telegram.Bot.Types.Message message, Telegram.Bot.Types.CallbackQuery? callbackQuery)
    {
        try
        {
            await WaitCommand(message, callbackQuery);
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    private async Task WaitCommand(Telegram.Bot.Types.Message message, Telegram.Bot.Types.CallbackQuery? callbackQuery)
    {
        using var db = new DatabaseContext();
        var chat = db.Chats.Where(s => s.ChatId == message.Chat.Id).First();

        if (message.Text == null || !message.Text.StartsWith("/"))
        {
            throw new ArgumentException("Unknown command", message.Text);
        }

        var messageText = message.Text;
        switch (messageText)
        {
            case "/start":
            case "/help":
                await _botManager.Client.SendTextMessageAsync(
                    chatId: chat.ChatId,
                    text: @"Welcome to telegram bot!"
                    );
                chat.Status = (int)ChatStatus.WAIT_COMMAND;
                break;
            case "/selection":
                await _botManager.Client.SendTextMessageAsync(
                    chatId: chat.ChatId,
                    text: @"Choose file from the list (recently processed and sample file are available):",
                    replyMarkup: _keyboardsManager.GenerateInlineKeyboardFiles(chat)
                    );
                chat.Status = (int)ChatStatus.WAIT_FILE_SELECTION_OPTION;
                break;
            case "/sorting":
                await _botManager.Client.SendTextMessageAsync(
                    chatId: chat.ChatId,
                    text: @"Choose file from the list (recently processed and sample file are available):",
                    replyMarkup: _keyboardsManager.GenerateInlineKeyboardFiles(chat)
                    );
                chat.Status = (int)ChatStatus.WAIT_FILE_SORTING_OPTION;
                break;
            default:
                await Console.Out.WriteLineAsync($"Unknown message \"{messageText}\"");
                break;
        }
        await db.SaveChangesAsync();
    }

    private async Task WaitFileSelectionOption(Telegram.Bot.Types.Message message, Telegram.Bot.Types.CallbackQuery? callbackQuery)
    {
        if (callbackQuery == null)
        {
            throw new ArgumentException("Not a correct answer", nameof(callbackQuery));
        }

        using var db = new DatabaseContext();
        var chat = db.Chats.Where(s => s.ChatId == message.Chat.Id).First();
        var selection = new Selection() { Chat = chat };
        switch (callbackQuery.Data)
        {
            case "SAMPLE":
                break;
            case "LOAD":
                chat.Status = (int)ChatStatus.WAIT_FILE_SELECTION_LOADING;
                await _botManager.Client.EditMessageTextAsync(chat.ChatId, message.MessageId, "Load .csv or .json file: ", replyMarkup: InlineKeyboardMarkup.Empty());
                break;
            default:
                var sourceFile = db.Files.Where(s => s.FileId == int.Parse(callbackQuery.Data!)).FirstOrDefault()!;
                selection.IdentNumberFile = sourceFile.FileId;
                await _botManager.Client.EditMessageTextAsync(chat.ChatId, message.MessageId, $"Selected revision: {sourceFile.Description}", replyMarkup: InlineKeyboardMarkup.Empty());
                chat.Status = (int)ChatStatus.CHOOSE_SELECTION_FIELDS;
                await _botManager.Client.SendTextMessageAsync(chat.ChatId, $"Select fields for selecting data", replyMarkup: _keyboardsManager.GenerateFieldsKeyboard());
                break;
        }
        await db.Selections.AddAsync(selection);
        await db.SaveChangesAsync();
    }

    private async Task WaitFileSelectionLoading(Telegram.Bot.Types.Message message, Telegram.Bot.Types.CallbackQuery? callbackQuery)
    {
        if (message.Document == null)
        {
            throw new ArgumentException("Not a correct answer", nameof(message));
        }
        var fileId = await _fileManager.DownloadAndValidateSourceFileToDatabase(message);

        using var db = new DatabaseContext();
        var selection = db.Selections.OrderByDescending(s => s.CreatedAt).First();
        selection.IdentNumberFile = fileId;
        db.SaveChanges();

        var chat = db.Chats.Where(s => s.ChatId == message.Chat.Id).First();
        chat.Status = (int)ChatStatus.CHOOSE_SELECTION_FIELDS;
        await _botManager.Client.SendTextMessageAsync(chat.ChatId, $"Select fields for selecting data", replyMarkup: _keyboardsManager.GenerateFieldsKeyboard());
        await db.SaveChangesAsync();
        return;
    }

    private async Task ChooseSelectionFields(Telegram.Bot.Types.Message message, Telegram.Bot.Types.CallbackQuery? callbackQuery)
    {
        if (callbackQuery == null)
        {
            throw new ArgumentException("Not a correct answer", nameof(callbackQuery));
        }

        using var db = new DatabaseContext();
        var chat = db.Chats.Where(s => s.ChatId == message.Chat.Id).First();
        var selection = db.Selections.OrderByDescending(s => s.CreatedAt).First(s => s.Chat.ChatId == chat.ChatId);
        var recievedData = callbackQuery.Data;
        if (recievedData == "RUN")
        {
            // Запуск выбора значений.
            if (!db.SelectionParams.Where(s => s.Selection == selection).Any())
            {
                await _botManager.Client.AnswerCallbackQueryAsync(callbackQuery.Id, $"Nothing was selected for selection. Choose any field");
                return;
            }
            await _botManager.Client.EditMessageTextAsync(chat.ChatId, message.MessageId,
                $"These fields will be required to select data: {string.Join("; ", db.SelectionParams.Where(s => s.Selection == selection).Select(s => DataField.GetDataField(s.Field)))}",
                replyMarkup: InlineKeyboardMarkup.Empty());
            var requestedField = DataField.GetDataField(db.SelectionParams.Where(s => s.Selection == selection && s.Value == null).First().Field);
            await _botManager.Client.SendTextMessageAsync(chat.ChatId, $"Eneter value for the field \"{requestedField}\"");
            chat.Status = (int)ChatStatus.CHOOSE_SELECTION_PARAMS;
        }
        else
        {
            // Выбор поля
            int receivedField = int.Parse(recievedData!);
            if (!db.SelectionParams.Where(s => s.Selection == selection && s.Field == receivedField).Any())
            {
                var selectionParam = new SelectionParams() { Selection = selection, Field = receivedField };
                db.SelectionParams.Add(selectionParam);
                await _botManager.Client.AnswerCallbackQueryAsync(callbackQuery.Id, $"Field {DataField.GetDataField(receivedField)} marked as selection filter");
            }
            else
            {
                var param = db.SelectionParams.Where(s => s.Selection == selection && s.Field == receivedField).First();
                db.SelectionParams.Remove(param);
                await _botManager.Client.AnswerCallbackQueryAsync(callbackQuery.Id, $"Field {DataField.GetDataField(receivedField)} unmarked as selection filter");
            }
        }
        await db.SaveChangesAsync();
    }

    private async Task ChooseSelectionParams(Telegram.Bot.Types.Message message, Telegram.Bot.Types.CallbackQuery? callbackQuery)
    {
        if (callbackQuery != null)
            throw new ArgumentException("CallbackQuery does not supported in this request", nameof(message));

        // Save new value
        using var db = new DatabaseContext();
        var chat = db.Chats.Where(s => s.ChatId == message.Chat.Id).First();
        var selection = db.Selections.OrderByDescending(s => s.CreatedAt).First(s => s.Chat.ChatId == chat.ChatId);
        var param = db.SelectionParams.Where(s => s.Selection == selection && s.Value == null).First();
        param.Value = message.Text;
        await db.SaveChangesAsync();

        // Check for the rest empty request fields.
        if (db.SelectionParams.Where(s => s.Selection == selection && s.Value == null).Any())
        {
            var requestedField = DataField.GetDataField(db.SelectionParams.Where(s => s.Selection == selection && s.Value == null).First().Field);
            await _botManager.Client.SendTextMessageAsync(chat.ChatId, $"Eneter value for the field \"{requestedField}\"");
        }
        else
        {
            await _botManager.Client.SendTextMessageAsync(chat.ChatId, "Select saving option for your request:", replyMarkup: _keyboardsManager.GenerateFileFormatKeyboard());
            chat.Status = (int)ChatStatus.WAIT_SELECTION_SAVING_TYPE;
            await db.SaveChangesAsync();
        }
    }

    private async Task WaitSelectionFileType(Telegram.Bot.Types.Message message, Telegram.Bot.Types.CallbackQuery? callbackQuery)
    {
        if (callbackQuery == null)
        {
            throw new ArgumentException("Not a correct answer", nameof(callbackQuery));
        }

        using var db = new DatabaseContext();
        var chat = db.Chats.Where(s => s.ChatId == message.Chat.Id).First();
        var selection = db.Selections.OrderByDescending(s => s.CreatedAt).First(s => s.Chat.ChatId == chat.ChatId);
        var recievedData = callbackQuery.Data;
        var selectionParams = db.SelectionParams
                    .Where(s => s.Selection == selection)
                    .Select(s => new { s.Field, s.Value })
                    .AsEnumerable()
                    .Select(s => new ValueTuple<string, string>(DataField.GetDataField(s.Field).Title, s.Value!));
        MemoryStream outputStream = new();
        var chatFileId = db.Files.First(s => s.FileId == selection.IdentNumberFile).ChatFileId;

        var result = new Selector(await _fileManager.DownladAttractions(chatFileId), selectionParams).Select();
        if (!result.Any())
        {
            await _botManager.Client.EditMessageTextAsync(chat.ChatId, message.MessageId, "No results by this request!", replyMarkup: InlineKeyboardMarkup.Empty());
            return;
        }

        Telegram.Bot.Types.Message sendedMessage = new();
        switch (recievedData)
        {
            case "JSON":
                outputStream = new JSONProcessing().Write(result);
                sendedMessage = await _botManager.Client.SendDocumentAsync(chat.ChatId, Telegram.Bot.Types.InputFile.FromStream(outputStream, "filename.json"));
                break;
            case "CSV":
                outputStream = new CSVProcessing().Write(result);
                sendedMessage = await _botManager.Client.SendDocumentAsync(chat.ChatId, Telegram.Bot.Types.InputFile.FromStream(outputStream, "filename.csv"));
                break;
        }
        await db.Files.AddAsync(new ChatFile()
        {
            Chat = chat,
            ChatFileId = sendedMessage.Document!.FileId,
            Description = $"Selected: {string.Join(';', db.SelectionParams.Where(s => s.Selection == selection).Select(s => DataField.GetDataField(s.Field).Title))}",
            IsSource = false
        });
        await db.SaveChangesAsync();
        await _botManager.Client.EditMessageTextAsync(chat.ChatId, message.MessageId, "Here you are!", replyMarkup: InlineKeyboardMarkup.Empty());

    }

    //
    // Sorting
    //

    private async Task WaitFileSortingOption(Telegram.Bot.Types.Message message, Telegram.Bot.Types.CallbackQuery? callbackQuery)
    {
        if (callbackQuery == null)
        {
            throw new ArgumentException("Not a correct answer", nameof(callbackQuery));
        }

        using var db = new DatabaseContext();
        var chat = db.Chats.Where(s => s.ChatId == message.Chat.Id).First();
        var sorting = new Sorting() { Chat = chat };
        switch (callbackQuery.Data)
        {
            case "SAMPLE":
                break;
            case "LOAD":
                chat.Status = (int)ChatStatus.WAIT_FILE_SORTING_LOADING;
                await _botManager.Client.EditMessageTextAsync(chat.ChatId, message.MessageId, "Load .csv or .json file: ", replyMarkup: InlineKeyboardMarkup.Empty());
                break;
            default:
                var sourceFile = db.Files.Where(s => s.FileId == int.Parse(callbackQuery.Data!)).FirstOrDefault()!;
                sorting.IdentNumberFile = sourceFile.FileId;
                await _botManager.Client.EditMessageTextAsync(chat.ChatId, message.MessageId, $"Selected revision: {sourceFile.Description}", replyMarkup: InlineKeyboardMarkup.Empty());
                chat.Status = (int)ChatStatus.CHOOSE_SORTING_FIELDS;
                await _botManager.Client.SendTextMessageAsync(chat.ChatId, $"Select fields for sorting data", replyMarkup: _keyboardsManager.GenerateFieldsKeyboard());
                break;
        }
        await db.Sortings.AddAsync(sorting);
        await db.SaveChangesAsync();
    }

    private async Task WaitFileSortingLoading(Telegram.Bot.Types.Message message, Telegram.Bot.Types.CallbackQuery? callbackQuery)
    {
        if (message.Document == null)
        {
            throw new ArgumentException("Not a correct answer", nameof(message));
        }
        var fileId = await _fileManager.DownloadAndValidateSourceFileToDatabase(message);

        using var db = new DatabaseContext();
        var sorting = db.Sortings.OrderByDescending(s => s.CreatedAt).First();
        sorting.IdentNumberFile = fileId;
        db.SaveChanges();

        var chat = db.Chats.Where(s => s.ChatId == message.Chat.Id).First();
        chat.Status = (int)ChatStatus.CHOOSE_SORTING_FIELDS;
        await _botManager.Client.SendTextMessageAsync(chat.ChatId, $"Select fields for sorting data", replyMarkup: _keyboardsManager.GenerateFieldsKeyboard());
        await db.SaveChangesAsync();
        return;
    }

    private async Task ChooseSortingFields(Telegram.Bot.Types.Message message, Telegram.Bot.Types.CallbackQuery? callbackQuery)
    {
        if (callbackQuery == null)
        {
            throw new ArgumentException("Not a correct answer", nameof(callbackQuery));
        }

        using var db = new DatabaseContext();
        var chat = db.Chats.Where(s => s.ChatId == message.Chat.Id).First();
        var sorting = db.Sortings.OrderByDescending(s => s.CreatedAt).First(s => s.Chat.ChatId == chat.ChatId);
        var recievedData = callbackQuery.Data;

        if (recievedData == "RUN")
        {
            // Запуск выбора значений.
            if (!db.SortingParams.Where(s => s.Sorting == sorting).Any())
            {
                await _botManager.Client.AnswerCallbackQueryAsync(callbackQuery.Id, $"Nothing was selected for selection. Choose any field");
                return;
            }
            await _botManager.Client.EditMessageTextAsync(chat.ChatId, message.MessageId,
                $"These fields will be required to sort data: {string.Join("; ", db.SortingParams.Where(s => s.Sorting == sorting).Select(s => DataField.GetDataField(s.Field)))}",
                replyMarkup: InlineKeyboardMarkup.Empty());
            var requestedField = DataField.GetDataField(db.SortingParams.Where(s => s.Sorting == sorting && s.IsDescending == null).First().Field);
            await _botManager.Client.SendTextMessageAsync(chat.ChatId, $"Eneter opiton for the field \"{requestedField}\"", replyMarkup: _keyboardsManager.GenerateSortingOrderKeyboard());
            chat.Status = (int)ChatStatus.CHOOSE_SORTING_PARAMS;
        }
        else
        {
            // Выбор поля
            int receivedField = int.Parse(recievedData!);
            if (!db.SortingParams.Where(s => s.Sorting == sorting && s.Field == receivedField).Any())
            {
                var sortingParam = new SortingParams() { Sorting = sorting, Field = receivedField };
                db.SortingParams.Add(sortingParam);
                await _botManager.Client.AnswerCallbackQueryAsync(callbackQuery.Id, $"Field {DataField.GetDataField(receivedField)} marked as sorting filter");
            }
            else
            {
                var param = db.SortingParams.Where(s => s.Sorting == sorting && s.Field == receivedField).First();
                db.SortingParams.Remove(param);
                await _botManager.Client.AnswerCallbackQueryAsync(callbackQuery.Id, $"Field {DataField.GetDataField(receivedField)} unmarked as sorting filter");
            }
        }
        await db.SaveChangesAsync();
    }

    private async Task ChooseSortingParams(Telegram.Bot.Types.Message message, Telegram.Bot.Types.CallbackQuery? callbackQuery)
    {
        if (callbackQuery == null)
            throw new ArgumentException("CallbackQuery expected here", nameof(message));

        // Save new value
        using var db = new DatabaseContext();
        var chat = db.Chats.Where(s => s.ChatId == message.Chat.Id).First();
        var sorting = db.Sortings.OrderByDescending(s => s.CreatedAt).First(s => s.Chat.ChatId == chat.ChatId);
        var param = db.SortingParams.Where(s => s.Sorting == sorting && s.IsDescending == null).First();
        param.IsDescending = callbackQuery.Data == "d" ? true : false;
        await _botManager.Client.EditMessageTextAsync(chat.ChatId,
            message.MessageId,
            $"Field {DataField.GetDataField(param.Field)} will be sorted" + (param.IsDescending == false ? "ascending" : "descending"));

        await db.SaveChangesAsync();

        // Check for the rest empty request fields.
        if (db.SortingParams.Where(s => s.Sorting == sorting && s.IsDescending == null).Any())
        {
            var requestedField = DataField.GetDataField(db.SortingParams.Where(s => s.Sorting == sorting && s.IsDescending == null).First().Field);
            await _botManager.Client.SendTextMessageAsync(chat.ChatId, $"Eneter opiton for the field \"{requestedField}\"", replyMarkup: _keyboardsManager.GenerateSortingOrderKeyboard());
        }
        else
        {
            await _botManager.Client.SendTextMessageAsync(chat.ChatId, "Select saving option for your request:", replyMarkup: _keyboardsManager.GenerateFileFormatKeyboard());
            chat.Status = (int)ChatStatus.WAIT_SORTING_SAVING_TYPE;
            await db.SaveChangesAsync();
        }
    }

    private async Task WaitSortingSavingType(Telegram.Bot.Types.Message message, Telegram.Bot.Types.CallbackQuery? callbackQuery)
    {
        if (callbackQuery == null)
        {
            throw new ArgumentException("Not a correct answer", nameof(callbackQuery));
        }

        using var db = new DatabaseContext();
        var chat = db.Chats.Where(s => s.ChatId == message.Chat.Id).First();
        var sorting = db.Sortings.OrderByDescending(s => s.CreatedAt).First(s => s.Chat.ChatId == chat.ChatId);
        var recievedData = callbackQuery.Data;
        var sortingParams = db.SortingParams
                    .OrderBy(s => s.CreatedAt)
                    .Where(s => s.Sorting == sorting)
                    .Select(s => new { s.Field, s.IsDescending })
                    .AsEnumerable()
                    .Select(s => new ValueTuple<string, bool>(DataField.GetDataField(s.Field).Title, s.IsDescending ?? false));
        MemoryStream outputStream = new();
        var chatFileId = db.Files.First(s => s.FileId == sorting.IdentNumberFile).ChatFileId;

        var result = new Sorter(await _fileManager.DownladAttractions(chatFileId), sortingParams).Sort();

        Telegram.Bot.Types.Message sendedMessage = new();
        switch (recievedData)
        {
            case "JSON":
                outputStream = new JSONProcessing().Write(result);
                sendedMessage = await _botManager.Client.SendDocumentAsync(chat.ChatId, Telegram.Bot.Types.InputFile.FromStream(outputStream, "filename.json"));
                break;
            case "CSV":
                outputStream = new CSVProcessing().Write(result);
                sendedMessage = await _botManager.Client.SendDocumentAsync(chat.ChatId, Telegram.Bot.Types.InputFile.FromStream(outputStream, "filename.csv"));
                break;
        }
        await db.Files.AddAsync(new ChatFile()
        {
            Chat = chat,
            ChatFileId = sendedMessage.Document!.FileId,
            Description = $"Sorted: {string.Join(';', db.SortingParams.Where(s => s.Sorting == sorting).Select(s => DataField.GetDataField(s.Field).Title))}",
            IsSource = false
        });
        await db.SaveChangesAsync();
        await _botManager.Client.EditMessageTextAsync(chat.ChatId, message.MessageId, "Here you are!", replyMarkup: InlineKeyboardMarkup.Empty());
    }
}
