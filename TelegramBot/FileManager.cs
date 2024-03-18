using DataManager.Models;
using DataManager;
using Telegram.Bot;
using Models.DataFormatProcessors;
using Models;
using System.IO;
using Telegram.Bot.Types;

namespace TelegramBot
{
    internal class FileManager
    {
        private readonly BotManager _botManager;

        public FileManager(BotManager botManager) =>
            _botManager = botManager;

        public async Task<int> DownloadAndValidateSourceFileToDatabase(Telegram.Bot.Types.Message message)
        {
            using (var db = new DatabaseContext())
            {
                var chat = db.Chats.Where(s => s.ChatId == message.Chat.Id).First();
                using MemoryStream stream = new MemoryStream();
                var fileInfo = await _botManager.Client.GetInfoAndDownloadFileAsync(fileId: message.Document!.FileId, stream);

                var validator = new DataFormatValidator();
                if (Path.GetExtension(fileInfo.FilePath)?.ToLower() == ".json")
                    validator.ValidateJson(stream);
                else if (Path.GetExtension(fileInfo.FilePath)?.ToLower() == ".csv")
                    validator.ValidateCsv(stream);
                else
                    throw new ArgumentException("Not a correct file extension");

                var file = new ChatFile() { ChatFileId = message.Document!.FileId, Chat = chat, IsSource = true };
                await db.Files.AddAsync(file);
                db.SaveChanges();
                return file.FileId;
            }
        }

        public async Task<List<Attraction>> DownladAttractions(string fileId)
        {
            using MemoryStream stream = new MemoryStream();
            var fileInfo = await _botManager.Client.GetInfoAndDownloadFileAsync(fileId, stream);

            if (Path.GetExtension(fileInfo.FilePath)?.ToLower() == ".json")
                return new JSONProcessing().Read(stream);
            else
                return new CSVProcessing().Read(stream);
        }

        public async Task<Message> SendProcessedDocument(long chatId, Stream outputStream, string fileName) =>
            await _botManager.Client.SendDocumentAsync(chatId, InputFile.FromStream(outputStream, fileName));
    }
}
