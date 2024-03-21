using DataManager;
using DataManager.Mapping;
using DataManager.Models;
using Models.DataFormatProcessors;
using Models;
using Microsoft.Extensions.Logging;

namespace TelegramBot;

/// <summary>
/// Class responsible for processing different states and commands in the Telegram bot.
/// </summary>
internal class StateProcessor
{
    private readonly FileManager _fileManager;
    private readonly DialogManager _dialogManager;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="StateProcessor"/> class.
    /// </summary>
    /// <param name="botManager">The bot manager instance.</param>
    /// <param name="logger">The logger instance.</param>
    public StateProcessor(BotManager botManager, ILogger logger)
    {
        _fileManager = new FileManager(botManager);
        _dialogManager = new DialogManager(botManager);
        _logger = logger;
    }

    /// <summary>
    /// Processes the request based on the current chat state and the received message or callback query.
    /// </summary>
    /// <param name="message">The message received from the user.</param>
    /// <param name="callbackQuery">The callback query received from the user.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ProcessRequest(Telegram.Bot.Types.Message message, Telegram.Bot.Types.CallbackQuery? callbackQuery = null)
    {
        // Get currentState from chat info.
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
                    await WaitCommand(message, callbackQuery); break;

                // Selection related.
                case ChatStatus.WAIT_FILE_SELECTION_OPTION:
                    await WaitFileSelectionOption(message, callbackQuery); break;
                case ChatStatus.WAIT_FILE_SELECTION_LOADING:
                    await WaitFileSelectionLoading(message); break;
                case ChatStatus.CHOOSE_SELECTION_FIELDS:
                    await ChooseSelectionFields(message, callbackQuery); break;
                case ChatStatus.CHOOSE_SELECTION_PARAMS:
                    await ChooseSelectionParams(message, callbackQuery); break;
                case ChatStatus.WAIT_SELECTION_SAVING_TYPE:
                    await WaitSelectionFileType(message, callbackQuery); break;
                case ChatStatus.WAIT_SELECTION_FILENAME:
                    await WaitSelectionFileName(message); break;

                // Sorting related.
                case ChatStatus.WAIT_FILE_SORTING_OPTION:
                    await WaitFileSortingOption(message, callbackQuery); break;
                case ChatStatus.WAIT_FILE_SORTING_LOADING:
                    await WaitFileSortingLoading(message); break;
                case ChatStatus.CHOOSE_SORTING_FIELDS:
                    await ChooseSortingFields(message, callbackQuery); break;
                case ChatStatus.CHOOSE_SORTING_PARAMS:
                    await ChooseSortingParams(message, callbackQuery); break;
                case ChatStatus.WAIT_SORTING_SAVING_TYPE:
                    await WaitSortingSavingType(message, callbackQuery); break;
                case ChatStatus.WAIT_SORTING_FILENAME:
                    await WaitSortingFilename(message, callbackQuery); break;
            }
        }
        catch (ArgumentException e)
        {
            await _dialogManager.SendUnknownCommandMessage(chat.ChatId);
            _logger.LogError("ChatId: {0}; {1}", chat.ChatId, e.Message);
        }
        catch (FormatException)
        {
            await _dialogManager.SendFormatExceptionMessage(chat.ChatId);
            _logger.LogError("ChatId: {0}; Incorrect file format", chat.ChatId);
        }
        catch (Exception e)
        {
            await _dialogManager.SendUnknownCommandMessage(chat.ChatId);
            _logger.LogError("ChatId: {0}; Unkwnown command received or unhandled exception raised", chat.ChatId);
        }
    }

    // Below are methods to process user's states and send appropriate messages.

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

    // WAIT_COMMAND state.

    private async Task WaitCommand(Telegram.Bot.Types.Message message, Telegram.Bot.Types.CallbackQuery? callbackQuery)
    {
        using var db = new DatabaseContext();
        var chat = db.Chats.Where(s => s.ChatId == message.Chat.Id).First();

        if (message.Text == null || !message.Text.StartsWith("/"))
        {
            throw new ArgumentException("Unknown command", nameof(message));
        }

        // Process commands.
        var messageText = message.Text;
        switch (messageText)
        {
            case "/start":
                await _dialogManager.SendWelcomeMessage(chat.ChatId);
                chat.Status = (int)ChatStatus.WAIT_COMMAND;
                _logger.LogInformation("ChatId: {0}; Command /start received", chat.ChatId);
                break;
            case "/help":
                await _dialogManager.SendHelpMessage(chat.ChatId);
                chat.Status = (int)ChatStatus.WAIT_COMMAND;
                _logger.LogInformation("ChatId: {0}; Command /help received", chat.ChatId);
                break;
            case "/selection":
                await _dialogManager.SendSelectionFileOptionMessage(chat.ChatId);
                chat.Status = (int)ChatStatus.WAIT_FILE_SELECTION_OPTION;
                _logger.LogInformation("ChatId: {0}; Command /selection received", chat.ChatId);
                break;
            case "/sorting":
                await _dialogManager.SendSortingFileOptionMessage(chat.ChatId);
                chat.Status = (int)ChatStatus.WAIT_FILE_SORTING_OPTION;
                _logger.LogInformation("ChatId: {0}; Command /sorting received", chat.ChatId);
                break;
            default:
                await _dialogManager.SendUnknownCommandMessage(chat.ChatId);
                _logger.LogError("ChatId: {0}; Unknown command received", chat.ChatId);
                break;
        }
        await db.SaveChangesAsync();
    }
    
    /// <summary>
    /// WAIT_FILE_SELECTION_OPTION state.
    /// </summary>
    private async Task WaitFileSelectionOption(Telegram.Bot.Types.Message message, Telegram.Bot.Types.CallbackQuery? callbackQuery)
    {
        if (callbackQuery == null)
            throw new ArgumentException("Not a correct selection option chosen", nameof(message));

        // Create new selection.
        using var db = new DatabaseContext();
        var chat = db.Chats.Where(s => s.ChatId == message.Chat.Id).First();
        var selection = new Selection() { Chat = chat };

        // File loading option.
        switch (callbackQuery!.Data)
        {
            // Loading file from local device.
            case "LOAD":
                chat.Status = (int)ChatStatus.WAIT_FILE_SELECTION_LOADING;
                await _dialogManager.EditFileLoadingMessage(chat.ChatId, message.MessageId, isNewFile: true);
                _logger.LogInformation("ChatId: {0}; File uploading selection option chosen", chat.ChatId);
                break;
            // File from database.
            default:
                var sourceFile = db.Files.Where(s => s.FileId == int.Parse(callbackQuery.Data!)).FirstOrDefault()!;
                selection.IdentNumberFile = sourceFile.FileId;
                await _dialogManager.EditFileLoadingMessage(chat.ChatId, message.MessageId, description: sourceFile.Description ?? "", isNewFile: false);
                chat.Status = (int)ChatStatus.CHOOSE_SELECTION_FIELDS;
                _logger.LogInformation("ChatId: {0}; Previous file revision as a source for selection chosen", chat.ChatId);

                await _dialogManager.SendFieldsKeyboard(chat.ChatId);
                break;
        }

        // Saving new selection to db.
        await db.Selections.AddAsync(selection);
        await db.SaveChangesAsync();
    }

    /// <summary>
    /// WAIT_FILE_SELECTION_LOADING
    /// </summary>
    private async Task WaitFileSelectionLoading(Telegram.Bot.Types.Message message)
    {
        if (message.Document == null)
            throw new ArgumentException("Not a file", nameof(message));

        // Downloading and validating the file.
        var fileId = await _fileManager.DownloadAndValidateSourceFileToDatabase(message);

        // Saving the source file to specific selection.
        using var db = new DatabaseContext();
        var selection = db.Selections.OrderByDescending(s => s.CreatedAt).First();
        selection.IdentNumberFile = fileId;
        db.SaveChanges();

        var chat = db.Chats.Where(s => s.ChatId == message.Chat.Id).First();
        chat.Status = (int)ChatStatus.CHOOSE_SELECTION_FIELDS;
        await _dialogManager.SendFieldsKeyboard(chat.ChatId);

        _logger.LogInformation("ChatId: {0}; File sucsessfully uploaded", chat.ChatId);
        await db.SaveChangesAsync();
        return;
    }

    /// <summary>
    /// CHOOSE_SELECTION_FIELDS state.
    /// </summary>
    private async Task ChooseSelectionFields(Telegram.Bot.Types.Message message, Telegram.Bot.Types.CallbackQuery? callbackQuery)
    {
        if (callbackQuery == null)
            throw new ArgumentException("Not a correct answer", nameof(callbackQuery));

        using var db = new DatabaseContext();
        var chat = db.Chats.Where(s => s.ChatId == message.Chat.Id).First();
        var selection = db.Selections.OrderByDescending(s => s.CreatedAt).First(s => s.Chat.ChatId == chat.ChatId);
        var recievedData = callbackQuery.Data;
        if (recievedData == "RUN")
        {
            // Run up selection parameters look up.
            if (!db.SelectionParams.Where(s => s.Selection == selection).Any())
            {
                await _dialogManager.AnswerNothingSelectedCallbackQuery(callbackQuery.Id);
                _logger.LogError("ChatId: {0}; Attempt to selecet without any field chosen", chat.ChatId);
                return;
            }
            await _dialogManager.EditListedSelectedFieldsMessage(chat.ChatId, message.MessageId,
                db.SelectionParams.Where(s => s.Selection == selection).Select(s => DataField.GetDataField(s.Field).Title));

            var requestedField = DataField.GetDataField(db.SelectionParams.Where(s => s.Selection == selection && s.Value == null).First().Field);
            await _dialogManager.SendSelectingValueMessage(chat.ChatId, requestedField.Title);
            _logger.LogInformation("ChatId: {0}; Choosing fields for selection finished", chat.ChatId);
            chat.Status = (int)ChatStatus.CHOOSE_SELECTION_PARAMS;
        }
        else
        {
            // Field selection.
            int receivedField = int.Parse(recievedData!);
            if (!db.SelectionParams.Where(s => s.Selection == selection && s.Field == receivedField).Any())
            {
                // Mark.
                var selectionParam = new SelectionParams() { Selection = selection, Field = receivedField };
                db.SelectionParams.Add(selectionParam);
                await _dialogManager.AnswerFieldSelectedCallbackQuery(callbackQuery.Id, receivedField);
                _logger.LogInformation("ChatId: {0}; Field {1} marked as to select", chat.ChatId, DataField.GetDataField(receivedField).Title);
            }
            else
            {
                // Unmark.
                var param = db.SelectionParams.Where(s => s.Selection == selection && s.Field == receivedField).First();
                db.SelectionParams.Remove(param);
                await _dialogManager.AnswerFieldSelectedCallbackQuery(callbackQuery.Id, receivedField, isUnmarked: true);
                _logger.LogInformation("ChatId: {0}; Field {1} unmarked as to select", chat.ChatId, DataField.GetDataField(receivedField).Title);
            }
        }
        await db.SaveChangesAsync();
    }

    /// <summary>
    /// CHOOSE_SELECTION_PARAMS state.
    /// </summary>
    private async Task ChooseSelectionParams(Telegram.Bot.Types.Message message, Telegram.Bot.Types.CallbackQuery? callbackQuery)
    {
        if (callbackQuery != null)
            throw new ArgumentException("CallbackQuery does not supported in entering selection values", nameof(message));

        // Save new value
        using var db = new DatabaseContext();
        var chat = db.Chats.Where(s => s.ChatId == message.Chat.Id).First();
        var selection = db.Selections.OrderByDescending(s => s.CreatedAt).First(s => s.Chat.ChatId == chat.ChatId);
        var param = db.SelectionParams.Where(s => s.Selection == selection && s.Value == null).First();
        param.Value = message.Text;
        _logger.LogInformation("ChatId: {0}; Field {1} filled for selection with value {2}", chat.ChatId, DataField.GetDataField(param.Field).Title,
            message.Text);
        await db.SaveChangesAsync();

        // Check for the rest empty request fields.
        if (db.SelectionParams.Where(s => s.Selection == selection && s.Value == null).Any())
        {
            var requestedField = DataField.GetDataField(db.SelectionParams.Where(s => s.Selection == selection && s.Value == null).First().Field);
            await _dialogManager.SendSelectingValueMessage(chat.ChatId, requestedField.Title);
        }
        else
        {
            await _dialogManager.SendDownloadProcessedMessage(chat.ChatId);
            chat.Status = (int)ChatStatus.WAIT_SELECTION_SAVING_TYPE;
            _logger.LogInformation("ChatId: {0}; Choosing parameters completed. Waiting for file type", chat.ChatId);
            await db.SaveChangesAsync();
        }
    }

    /// <summary>
    /// WAIT_SELECTION_FILE_TYPE state.
    /// </summary>
    private async Task WaitSelectionFileType(Telegram.Bot.Types.Message message, Telegram.Bot.Types.CallbackQuery? callbackQuery)
    {
        if (callbackQuery == null)
        {
            throw new ArgumentException("Not a correct answer for waiting selection file type", nameof(callbackQuery));
        }

        using var db = new DatabaseContext();
        var chat = db.Chats.Where(s => s.ChatId == message.Chat.Id).First();
        var selection = db.Selections.OrderByDescending(s => s.CreatedAt).First(s => s.Chat.ChatId == chat.ChatId);
        var recievedData = callbackQuery.Data;

        selection.IsJson = recievedData == "JSON";
        _logger.LogInformation("ChatId: {0}; Saving parameter for selection {1} chosen", chat.ChatId, recievedData);
        chat.Status = (int)ChatStatus.WAIT_SELECTION_FILENAME;
        await db.SaveChangesAsync();

        await _dialogManager.EditFileFormatMessage(chat.ChatId, message.MessageId, selection.IsJson ?? false);
        await _dialogManager.SendFileNameMessage(chat.ChatId);
    }

    /// <summary>
    /// WAIT_SELECTION_FILENAME state.
    /// </summary>
    private async Task WaitSelectionFileName(Telegram.Bot.Types.Message message)
    {
        if (message.Text == null)
            throw new ArgumentException("Not s correct message");

        using var db = new DatabaseContext();
        var chat = db.Chats.Where(s => s.ChatId == message.Chat.Id).First();
        var selection = db.Selections.OrderByDescending(s => s.CreatedAt).First(s => s.Chat.ChatId == chat.ChatId);

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
            await _dialogManager.SendNoResultsMessage(chat.ChatId);
            chat.Status = (int)ChatStatus.WAIT_COMMAND;
            await db.SaveChangesAsync();
            return;
        }

        Telegram.Bot.Types.Message sendedMessage;
        if (selection.IsJson ?? false)
        {
            _logger.LogInformation("ChatId: {0}; Processed file sended in JSON format", chat.ChatId);
            outputStream = new JSONProcessing().Write(result);
            sendedMessage = await _fileManager.SendProcessedDocument(chat.ChatId, outputStream, $"{message.Text}.json");
        }
        else
        {
            _logger.LogInformation("ChatId: {0}; Processed file sended in CSV format", chat.ChatId);
            outputStream = new CSVProcessing().Write(result);
            sendedMessage = await _fileManager.SendProcessedDocument(chat.ChatId, outputStream, $"{message.Text}.csv");
        }

        _logger.LogInformation("ChatId: {0}; Processed file saved to database to continue using it in next requests", chat.ChatId);
        await db.Files.AddAsync(new ChatFile()
        {
            Chat = chat,
            ChatFileId = sendedMessage.Document!.FileId,
            Description = $"Selected: {string.Join(';', db.SelectionParams.Where(s => s.Selection == selection).Select(s => DataField.GetDataField(s.Field).Title))}",
            IsSource = false
        });

        chat.Status = (int)ChatStatus.WAIT_COMMAND;
        await db.SaveChangesAsync();
        await _dialogManager.SendSuccessMessage(chat.ChatId);
        await _dialogManager.SendHelpMessage(chat.ChatId);
    }

    //
    // Sorting
    //


    /// <summary>
    /// WAIT_FILE_SORTING_OPTION state.
    /// </summary>
    private async Task WaitFileSortingOption(Telegram.Bot.Types.Message message, Telegram.Bot.Types.CallbackQuery? callbackQuery)
    {
        if (callbackQuery == null)
        {
            throw new ArgumentException("Soritng option should consist of callback query response", nameof(callbackQuery));
        }

        using var db = new DatabaseContext();
        var chat = db.Chats.Where(s => s.ChatId == message.Chat.Id).First();
        var sorting = new Sorting() { Chat = chat };
        switch (callbackQuery.Data)
        {
            case "LOAD":
                chat.Status = (int)ChatStatus.WAIT_FILE_SORTING_LOADING;
                await _dialogManager.EditFileLoadingMessage(chat.ChatId, message.MessageId, isNewFile: true);
                _logger.LogInformation("ChatId: {0}; File uploading selection option chosen", chat.ChatId);
                break;
            default:
                var sourceFile = db.Files.Where(s => s.FileId == int.Parse(callbackQuery.Data!)).FirstOrDefault()!;
                sorting.IdentNumberFile = sourceFile.FileId;
                _logger.LogInformation("ChatId: {0}; Previous file revision as a source for selection chosen", chat.ChatId);
                await _dialogManager.EditFileLoadingMessage(chat.ChatId, message.MessageId, description: sourceFile.Description ?? "", isNewFile: false);
                chat.Status = (int)ChatStatus.CHOOSE_SORTING_FIELDS;
                await _dialogManager.SendFieldsKeyboard(chat.ChatId);
                break;
        }
        await db.Sortings.AddAsync(sorting);
        await db.SaveChangesAsync();
    }

    /// <summary>
    /// WAIT_FILE_SORTING_LOADING state.
    /// </summary>
    private async Task WaitFileSortingLoading(Telegram.Bot.Types.Message message)
    {
        if (message.Document == null)
        {
            throw new ArgumentException("Message does not consist of file", nameof(message));
        }
        var fileId = await _fileManager.DownloadAndValidateSourceFileToDatabase(message);

        using var db = new DatabaseContext();
        var sorting = db.Sortings.OrderByDescending(s => s.CreatedAt).First();
        sorting.IdentNumberFile = fileId;
        db.SaveChanges();

        var chat = db.Chats.Where(s => s.ChatId == message.Chat.Id).First();
        _logger.LogInformation("ChatId: {0}; File sucsessfully uploaded", chat.ChatId);
        chat.Status = (int)ChatStatus.CHOOSE_SORTING_FIELDS;
        await _dialogManager.SendFieldsKeyboard(chat.ChatId);
        await db.SaveChangesAsync();
        return;
    }

    /// <summary>
    /// CHOOSE_SORTING_FIELD state.
    /// </summary>
    private async Task ChooseSortingFields(Telegram.Bot.Types.Message message, Telegram.Bot.Types.CallbackQuery? callbackQuery)
    {
        if (callbackQuery == null)
        {
            throw new ArgumentException("Response to choosing field should be a callback query", nameof(callbackQuery));
        }

        using var db = new DatabaseContext();
        var chat = db.Chats.Where(s => s.ChatId == message.Chat.Id).First();
        var sorting = db.Sortings.OrderByDescending(s => s.CreatedAt).First(s => s.Chat.ChatId == chat.ChatId);
        var recievedData = callbackQuery.Data;

        if (recievedData == "RUN")
        {
            // Run sorting by only selected fields.
            if (!db.SortingParams.Where(s => s.Sorting == sorting).Any())
            {
                await _dialogManager.AnswerNothingSelectedCallbackQuery(callbackQuery.Id);
                _logger.LogError("ChatId: {0}; Attempt to selecet without any field chosen", chat.ChatId);
                return;
            }
            await _dialogManager.EditListedSelectedFieldsMessage(chat.ChatId, message.MessageId,
                db.SortingParams.Where(s => s.Sorting == sorting).Select(s => DataField.GetDataField(s.Field).Title));

            var requestedField = DataField.GetDataField(db.SortingParams.Where(s => s.Sorting == sorting && s.IsDescending == null).First().Field);
            _logger.LogInformation("ChatId: {0}; Choosing sorting option for sort by field", chat.ChatId);
            await _dialogManager.SendSortingOptionMessage(chat.ChatId, requestedField.Title);
            chat.Status = (int)ChatStatus.CHOOSE_SORTING_PARAMS;
        }
        else
        {
            // Field choosing.
            int receivedField = int.Parse(recievedData!);
            if (!db.SortingParams.Where(s => s.Sorting == sorting && s.Field == receivedField).Any())
            {
                var sortingParam = new SortingParams() { Sorting = sorting, Field = receivedField };
                db.SortingParams.Add(sortingParam);
                await _dialogManager.AnswerFieldSelectedCallbackQuery(callbackQuery.Id, receivedField);
            }
            else
            {
                var param = db.SortingParams.Where(s => s.Sorting == sorting && s.Field == receivedField).First();
                db.SortingParams.Remove(param);
                await _dialogManager.AnswerFieldSelectedCallbackQuery(callbackQuery.Id, receivedField, true);
            }
        }
        await db.SaveChangesAsync();
    }

    /// <summary>
    /// CHOOSE_SORTING_PARAMS state.
    /// </summary>
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
        _logger.LogInformation("ChatId: {0}; Sorting parameter {1} was slected for sort by field {2}", chat.ChatId,
            callbackQuery.Data == "d" ? "Descending" : "Ascending", DataField.GetDataField(param.Field).Title);
        await _dialogManager.EditSortingOptionMessage(chat.ChatId, message.MessageId, DataField.GetDataField(param.Field).Title, param.IsDescending ?? false);

        await db.SaveChangesAsync();

        // Check for the rest empty request fields.
        if (db.SortingParams.Where(s => s.Sorting == sorting && s.IsDescending == null).Any())
        {
            var requestedField = DataField.GetDataField(db.SortingParams.Where(s => s.Sorting == sorting && s.IsDescending == null).First().Field);
            await _dialogManager.SendSortingOptionMessage(chat.ChatId, requestedField.Title);
        }
        else
        {
            await _dialogManager.SendDownloadProcessedMessage(chat.ChatId);
            _logger.LogInformation("ChatId: {0}; Choosing parameters completed. Waiting for file type", chat.ChatId);
            chat.Status = (int)ChatStatus.WAIT_SORTING_SAVING_TYPE;
            await db.SaveChangesAsync();
        }
    }

    /// <summary>
    /// WAIT_SORTING_SAVING_TYPE
    /// </summary>
    private async Task WaitSortingSavingType(Telegram.Bot.Types.Message message, Telegram.Bot.Types.CallbackQuery? callbackQuery)
    {
        if (callbackQuery == null)
        {
            throw new ArgumentException("Sorting saving type should be a callback query resonse", nameof(callbackQuery));
        }

        using var db = new DatabaseContext();
        var chat = db.Chats.Where(s => s.ChatId == message.Chat.Id).First();
        var sorting = db.Sortings.OrderByDescending(s => s.CreatedAt).First(s => s.Chat.ChatId == chat.ChatId);
        var recievedData = callbackQuery.Data;

        sorting.IsJson = recievedData == "JSON";
        _logger.LogInformation("ChatId: {0}; Saving parameter for selection {1} chosen", chat.ChatId, recievedData);
        chat.Status = (int)ChatStatus.WAIT_SORTING_FILENAME;
        await db.SaveChangesAsync();

        await _dialogManager.EditFileFormatMessage(chat.ChatId, message.MessageId, sorting.IsJson ?? false);
        await _dialogManager.SendFileNameMessage(chat.ChatId);
    }

    /// <summary>
    /// WAIT_SORTING_FILENAME state.
    /// </summary>
    private async Task WaitSortingFilename(Telegram.Bot.Types.Message message, Telegram.Bot.Types.CallbackQuery? callbackQuery)
    {
        if (message.Text == null)
            throw new ArgumentException("Not s correct message");

        using var db = new DatabaseContext();
        var chat = db.Chats.Where(s => s.ChatId == message.Chat.Id).First();
        var sorting = db.Sortings.OrderByDescending(s => s.CreatedAt).First(s => s.Chat.ChatId == chat.ChatId);

        var sortingParams = db.SortingParams
        .OrderBy(s => s.CreatedAt)
                    .Where(s => s.Sorting == sorting)
                    .Select(s => new { s.Field, s.IsDescending })
                    .AsEnumerable()
                    .Select(s => new ValueTuple<string, bool>(DataField.GetDataField(s.Field).Title, s.IsDescending ?? false));
        MemoryStream outputStream = new();
        var chatFileId = db.Files.First(s => s.FileId == sorting.IdentNumberFile).ChatFileId;

        var result = new Sorter(await _fileManager.DownladAttractions(chatFileId), sortingParams).Sort();

        Telegram.Bot.Types.Message sendedMessage;
        if (sorting.IsJson ?? false)
        {
            outputStream = new JSONProcessing().Write(result);
            _logger.LogInformation("ChatId: {0}; Processed file sended in JSON format", chat.ChatId);
            sendedMessage = await _fileManager.SendProcessedDocument(chat.ChatId, outputStream, $"{message.Text}.json");
        }
        else
        {
            outputStream = new CSVProcessing().Write(result);
            _logger.LogInformation("ChatId: {0}; Processed file sended in CSV format", chat.ChatId);
            sendedMessage = await _fileManager.SendProcessedDocument(chat.ChatId, outputStream, $"{message.Text}.csv");
        }

        _logger.LogInformation("ChatId: {0}; Processed file saved to database to continue using it in next requests", chat.ChatId);
        await db.Files.AddAsync(new ChatFile()
        {
            Chat = chat,
            ChatFileId = sendedMessage.Document!.FileId,
            Description = $"Sorted: {string.Join(';', db.SortingParams.Where(s => s.Sorting == sorting).Select(s => DataField.GetDataField(s.Field).Title))}",
            IsSource = false
        });

        chat.Status = (int)ChatStatus.WAIT_COMMAND;
        await db.SaveChangesAsync();
        await _dialogManager.SendSuccessMessage(chat.ChatId);
        await _dialogManager.SendHelpMessage(chat.ChatId);
    }
}