using DataManager.Models;
using DataManager;

namespace TelegramBot
{
    internal class FileManager
    {
        public async Task<string> DownloadSourceFile(Telegram.Bot.Types.Message message)
        {
            using (var db = new DatabaseContext())
            {
                var chat = db.Chats.Where(s => s.ChatId == message.Chat.Id).First();

                var file = new ChatFile() { ChatFileId = message.Document!.FileId, Chat = chat, IsSource = true };
                await db.Files.AddAsync(file);
                db.SaveChanges();
                return file.ChatFileId;
            }
        }

        //public async Task SendProcessedFile()
    }
}
