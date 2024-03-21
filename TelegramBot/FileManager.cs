using DataManager.Models;
using DataManager;
using Telegram.Bot;
using Models.DataFormatProcessors;
using Models;
using Telegram.Bot.Types;
using System.Text.Json;
using CsvHelper;

namespace TelegramBot;

/// <summary>
/// Handles file management operations for the Telegram bot.
/// </summary>
internal class FileManager
{
    private readonly BotManager _botManager;

    /// <summary>
    /// Initializes a new instance of the FileManager class.
    /// </summary>
    /// <param name="botManager">The BotManager instance for bot interaction.</param>
    public FileManager(BotManager botManager) =>
        _botManager = botManager;

    /// <summary>
    /// Downloads and validates the source file to the database based on the message.
    /// </summary>
    /// <param name="message">The Telegram message containing the file to download and validate.</param>
    /// <returns>The file ID in the database.</returns>
    public async Task<int> DownloadAndValidateSourceFileToDatabase(Telegram.Bot.Types.Message message)
    {
        using (var db = new DatabaseContext())
        {
            var chat = db.Chats.Where(s => s.ChatId == message.Chat.Id).First();
            using MemoryStream stream = new MemoryStream();
            var fileInfo = await _botManager.Client.GetInfoAndDownloadFileAsync(fileId: message.Document!.FileId, stream);

            try
            {
                var validator = new DataFormatValidator(stream);
                if (Path.GetExtension(fileInfo.FilePath)?.ToLower() == ".json")
                    validator.ValidateJson();
                else if (Path.GetExtension(fileInfo.FilePath)?.ToLower() == ".csv")
                    validator.ValidateCsv();
                else
                    throw new FormatException("Not a correct file extension");
            }
            catch (Exception ex) when (ex is JsonException || ex is FormatException || ex is NotSupportedException || ex is BadDataException || ex is ReaderException)
            {
                throw new FormatException("Incorrect file format");
            }

            var file = new ChatFile() { ChatFileId = message.Document!.FileId, Chat = chat, IsSource = true };
            await db.Files.AddAsync(file);
            db.SaveChanges();
            return file.FileId;
        }
    }

    /// <summary>
    /// Downloads attractions data from a file based on the file ID.
    /// </summary>
    /// <param name="fileId">The ID of the file to download attractions from.</param>
    /// <returns>A list of attractions.</returns>
    public async Task<List<Attraction>> DownladAttractions(string fileId)
    {
        using MemoryStream stream = new MemoryStream();
        var fileInfo = await _botManager.Client.GetInfoAndDownloadFileAsync(fileId, stream);

        if (Path.GetExtension(fileInfo.FilePath)?.ToLower() == ".json")
            return new JSONProcessing().Read(stream);
        else
            return new CSVProcessing().Read(stream);
    }

    /// <summary>
    /// Sends the processed document to the specified chat.
    /// </summary>
    /// <param name="chatId">The ID of the chat to send the processed document to.</param>
    /// <param name="outputStream">The output stream containing the processed document.</param>
    /// <param name="fileName">The name of the file to be sent.</param>
    /// <returns>The sent message.</returns>
    public async Task<Message> SendProcessedDocument(long chatId, Stream outputStream, string fileName) =>
        await _botManager.Client.SendDocumentAsync(chatId, InputFile.FromStream(outputStream, fileName));
}
