using DataManager.Mapping;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot;

/// <summary>
/// Manages the dialogs and messages sent by the bot to users.
/// </summary>
internal class DialogManager
{
    private readonly BotManager _botManager;
    private readonly KeyboardsManager _keyboardsManager = new();

    /// <summary>
    /// Initializes a new instance of the DialogManager class with the specified bot manager.
    /// </summary>
    /// <param name="botManager">The bot manager instance.</param>
    public DialogManager(BotManager botManager)
    {
        _botManager = botManager;
    }

    // Methods for sending different types of messages to users...

    /// <summary>
    /// Sends a message for unknown commands to the specified chat ID.
    /// </summary>
    public Task SendUnknownCommandMessage(long chatId) =>
        _botManager.Client.SendTextMessageAsync(
            chatId,
            @"🔮Strange command🔮

Send appropriate response to the bot or...

ℹ️ Check out /help page to see the full list of commands"
            );

    /// <summary>
    /// Sends a welcome message to the specified chat ID.
    /// </summary>
    public Task SendWelcomeMessage(long chatId) =>
        _botManager.Client.SendTextMessageAsync(
            chatId,
            @"🎠🛝🎡 Welcome to AttractionsProcessingBot 🎢🎪🎆

ℹ️ To continue using the bot, it is suggested to read /help page briefly!"
            );

    /// <summary>
    /// Sends a help message to the specified chat ID.
    /// </summary>
    public Task SendHelpMessage(long chatId) =>
        _botManager.Client.SendTextMessageAsync(
            chatId,
            @"ℹ️ The bot is designed to process files of specific format

👀 Below are the list of features to process your file:

1️⃣ Sort attractions by several parameters and flexible settings for each field with /sorting command

2️⃣ Select attractions by several parameters with values for each of them with /selection command"
            );

    /// <summary>
    /// Sends a help message to the specified chat ID.
    /// </summary>
    public Task SendSelectionFileOptionMessage(long chatId) =>
        _botManager.Client.SendTextMessageAsync(
            chatId,
            @"🔎 Let's find some attractions

☝️ Please, select the source, from which attractions will be taken:

You can choose:
🕧 Recently processed files (don't worry, if it's your first request)

💽 Sample file

💾 File from your device
",
            replyMarkup: _keyboardsManager.GenerateInlineKeyboardFiles(chatId)
            );

    /// <summary>
    /// Edits the loading message for file processing.
    /// </summary>
    public Task EditFileLoadingMessage(long chatId, int messageId, string description="", bool isNewFile=false) =>
        _botManager.Client.EditMessageTextAsync(
            chatId,
            messageId,
            isNewFile ?
            @"💾 Upload file with your attractions using one of listed formats below:

📍 .JSON file

📍 .CSV file" :
            string.Format(@"🕧 File revision ""{0}"" has been selected", description),
            replyMarkup: InlineKeyboardMarkup.Empty()
            );

    /// <summary>
    /// Sends a message to prompt the user to select the source for finding attractions.
    /// </summary>
    public Task SendFieldsKeyboard(long chatId) =>
        _botManager.Client.SendTextMessageAsync(
            chatId,
            @"✌️ Select fields for your processing request",
            replyMarkup: _keyboardsManager.GenerateFieldsKeyboard()
            );

    /// <summary>
    /// Edits the message to indicate the selected file or file revision for loading attractions.
    /// </summary>
    public Task AnswerFieldSelectedCallbackQuery(string callbackQueryId, int field, bool isUnmarked=false) =>
        isUnmarked ?
        _botManager.Client.AnswerCallbackQueryAsync(callbackQueryId, $"Field {DataField.GetDataField(field)} unmarked as processing filter") :
        _botManager.Client.AnswerCallbackQueryAsync(callbackQueryId, $"Field {DataField.GetDataField(field)} marked as processing filter");

    /// <summary>
    /// Sends a message to prompt the user to select fields for processing requests.
    /// </summary>
    public Task AnswerNothingSelectedCallbackQuery(string callbackQueryId) =>
        _botManager.Client.AnswerCallbackQueryAsync(callbackQueryId, $"Nothing was selected for selection. Choose any field");

    /// <summary>
    /// Edits the message to show the selected fields for continuing the processing action.
    /// </summary>
    public Task EditListedSelectedFieldsMessage(long chatId, int messageId, IEnumerable<string> selectionFields)
    {
        string enumerateString = string.Join(Environment.NewLine, selectionFields.Select(s => $"🪄 {s}"));
        return _botManager.Client.EditMessageTextAsync(chatId,
            messageId,
            string.Format(@"Below are selected fields to continue processing action:

{0}", enumerateString),
            replyMarkup: InlineKeyboardMarkup.Empty());
    }

    /// <summary>
    /// Sends a message to prompt the user to enter a value for a selected field.
    /// </summary>
    public Task SendSelectingValueMessage(long chatId, string field) =>
        _botManager.Client.SendTextMessageAsync(
            chatId,
            $"👀 Enter value for the field \"{field}\""
            );

    /// <summary>
    /// Sends a message to prompt the user to select the saving option for the processed request.
    /// </summary>
    public Task SendDownloadProcessedMessage(long chatId) =>
        _botManager.Client.SendTextMessageAsync(
            chatId,
            "💿 Select saving option for your request: ",
            replyMarkup: _keyboardsManager.GenerateFileFormatKeyboard());

    /// <summary>
    /// Sends a message to inform the user that no results were found for the request.
    /// </summary>
    public Task SendNoResultsMessage(long chatId) =>
        _botManager.Client.SendTextMessageAsync(
            chatId,
            "🥲 No results for this request!"
            );

    /// <summary>
    /// Sends a message to inform the user about the successful completion of a task.
    /// </summary>
    public Task SendSuccessMessage(long chatId) =>
        _botManager.Client.SendTextMessageAsync(
            chatId,
            @"🎉 Here is your processed file 🎉"
            );

    /// <summary>
    /// Sends a message to prompt the user to select the source for sorting attractions.
    /// </summary>
    public Task SendSortingFileOptionMessage(long chatId) =>
        _botManager.Client.SendTextMessageAsync(
            chatId,
            @"🥇🥈🥉 Let's sort some attractions

☝️ Please, select the source, from which attractions will be taken:

You can choose:
🕧 Recently processed files (don't worry, if it's your first request)

💾 File from your device
",
            replyMarkup: _keyboardsManager.GenerateInlineKeyboardFiles(chatId)
            );

    /// <summary>
    /// Sends a message to prompt the user to enter the sorting option for a specific field.
    /// </summary>
    public Task SendSortingOptionMessage(long chatId, string field) =>
        _botManager.Client.SendTextMessageAsync(
            chatId,
            $"⬆️⬇️Enter sorting option for the field \"{field}\"",
            replyMarkup: _keyboardsManager.GenerateSortingOrderKeyboard());

    /// <summary>
    /// Edits the message to show the selected sorting option for a field.
    /// </summary>
    public Task EditSortingOptionMessage(long chatId, int messageId, string field, bool isDescending) =>
        _botManager.Client.EditMessageTextAsync(
            chatId,
            messageId,
            $"Field {field} will be sorted " + (isDescending ? "descending⬇️" : "ascending⬆️"),
            replyMarkup: InlineKeyboardMarkup.Empty()
            );

    /// <summary>
    /// Edits the message to show the selected file format.
    /// </summary>
    public Task EditFileFormatMessage(long chatId, int messageId, bool isJson) =>
        _botManager.Client.EditMessageTextAsync(
            chatId,
            messageId,
            string.Format(@"💿 Chosen {0} file format", isJson ? ".JSON" : ".CSV"),
            replyMarkup: InlineKeyboardMarkup.Empty()
            );

    /// <summary>
    /// Sends a message to enter the desired name of the processed file.
    /// </summary>
    public Task SendFileNameMessage(long chatId) =>
        _botManager.Client.SendTextMessageAsync(
            chatId,
            @"🗣️ Enter the desired name of your file (without extension, it will be added automatically):");

    /// <summary>
    /// Sends a message for an exception related to file format.
    /// </summary>
    public Task SendFormatExceptionMessage(long chatId) =>
        _botManager.Client.SendTextMessageAsync(
            chatId,
            @"🥶 Wrong file format"
            );
}
