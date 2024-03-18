using DataManager.Mapping;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot;

internal class DialogManager
{
    private readonly BotManager _botManager;
    private readonly KeyboardsManager _keyboardsManager = new();

    public DialogManager(BotManager botManager)
    {
        _botManager = botManager;
    }

    public Task SendUnknownCommandMessage(long chatId) =>
        _botManager.Client.SendTextMessageAsync(
            chatId,
            @"🔮Strange command🔮

Send appropriate response to the bot or...

ℹ️ Check out /help page to see the full list of commands"
            );

    public Task SendWelcomeMessage(long chatId) =>
        _botManager.Client.SendTextMessageAsync(
            chatId,
            @"🎠🛝🎡 Welcome to AttractionsProcessingBot 🎢🎪🎆

ℹ️ To continue using the bot, it is suggested to read /help page briefly!"
            );

    public Task SendHelpMessage(long chatId) =>
        _botManager.Client.SendTextMessageAsync(
            chatId,
            @"ℹ️ The bot is designed to process files of specific format

👀 Below are the list of features to process your file:

1️⃣ Sort attractions by several parameters and flexible settings for each field with /sorting command

2️⃣ Select attractions by several parameters with values for each of them with /selection command"
            );

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

    public Task SendFieldsKeyboard(long chatId) =>
        _botManager.Client.SendTextMessageAsync(
            chatId,
            @"✌️ Select fields for your processing request",
            replyMarkup: _keyboardsManager.GenerateFieldsKeyboard()
            );

    public Task AnswerFieldSelectedCallbackQuery(string callbackQueryId, int field, bool isUnmarked=false) =>
        isUnmarked ?
        _botManager.Client.AnswerCallbackQueryAsync(callbackQueryId, $"Field {DataField.GetDataField(field)} unmarked as processing filter") :
        _botManager.Client.AnswerCallbackQueryAsync(callbackQueryId, $"Field {DataField.GetDataField(field)} marked as processing filter");

    public Task AnswerNothingSelectedCallbackQuery(string callbackQueryId) =>
        _botManager.Client.AnswerCallbackQueryAsync(callbackQueryId, $"Nothing was selected for selection. Choose any field");

    public Task EditListedSelectedFieldsMessage(long chatId, int messageId, IEnumerable<string> selectionFields)
    {
        string enumerateString = string.Join(Environment.NewLine, selectionFields.Select(s => $"🪄 {s}"));
        return _botManager.Client.EditMessageTextAsync(chatId,
            messageId,
            string.Format(@"Below are selected fields to continue processing action:

{0}", enumerateString),
            replyMarkup: InlineKeyboardMarkup.Empty());
    }

    public Task SendSelectingValueMessage(long chatId, string field) =>
        _botManager.Client.SendTextMessageAsync(
            chatId,
            $"👀 Enter value for the field \"{field}\""
            );

    public Task SendDownloadProcessedMessage(long chatId) =>
        _botManager.Client.SendTextMessageAsync(
            chatId,
            "💿 Select saving option for your request: ",
            replyMarkup: _keyboardsManager.GenerateFileFormatKeyboard());

    public Task SendNoResultsMessage(long chatId) =>
        _botManager.Client.SendTextMessageAsync(
            chatId,
            "🥲 No results for this request!"
            );

    public Task SendSuccessMessage(long chatId) =>
        _botManager.Client.SendTextMessageAsync(
            chatId,
            @"🎉 Here is your processed file 🎉"
            );

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

    public Task SendSortingOptionMessage(long chatId, string field) =>
        _botManager.Client.SendTextMessageAsync(
            chatId,
            $"⬆️⬇️Enter sorting option for the field \"{field}\"",
            replyMarkup: _keyboardsManager.GenerateSortingOrderKeyboard());

    public Task EditSortingOptionMessage(long chatId, int messageId, string field, bool isDescending) =>
        _botManager.Client.EditMessageTextAsync(
            chatId,
            messageId,
            $"Field {field} will be sorted " + (isDescending ? "descending⬇️" : "ascending⬆️"),
            replyMarkup: InlineKeyboardMarkup.Empty()
            );

    public Task EditFileFormatMessage(long chatId, int messageId, bool isJson) =>
        _botManager.Client.EditMessageTextAsync(
            chatId,
            messageId,
            string.Format(@"💿 Chosen {0} file format", isJson ? ".JSON" : ".CSV"),
            replyMarkup: InlineKeyboardMarkup.Empty()
            );

    public Task SendFileNameMessage(long chatId) =>
        _botManager.Client.SendTextMessageAsync(
            chatId,
            @"🗣️ Enter the desired name of your file (without extension, it will be added automatically):");
    public Task SendFormatExceptionMessage(long chatId) =>
        _botManager.Client.SendTextMessageAsync(
            chatId,
            @"🥶 Wrong file format"
            );
}
