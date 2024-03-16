using DataManager;
using DataManager.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot;

internal class KeyboardsManager
{
    public InlineKeyboardMarkup GenerateInlineKeyboardFiles(Chat chat)
    {
        InlineKeyboardMarkup inlineKeyboardMarkup;

        using var db = new DatabaseContext();
        db.Files.Select(s => s).ToList();
        var querySet = db.Files.Where(s => s.Chat == chat && !s.IsSource).
            OrderByDescending(e => e.CreatedAt).
            Take(3);
        if (querySet.Count() > 0)
        {
            var recentlyRow = querySet.
                Select(file => InlineKeyboardButton.WithCallbackData(file.Description, file.ChatFileId));

            inlineKeyboardMarkup = new InlineKeyboardMarkup(new[]
            {
                recentlyRow.ToArray(),
                new[] { InlineKeyboardButton.WithCallbackData("Process the sample file", "SAMPLE") },
                new[] { InlineKeyboardButton.WithCallbackData("Load file from your device", "LOAD") }
            });
        }
        else
        {
            inlineKeyboardMarkup = new InlineKeyboardMarkup(new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData("Process the sample file", "SAMPLE") },
                new[] { InlineKeyboardButton.WithCallbackData("Load file from your device", "LOAD") }
            });
        }

        return inlineKeyboardMarkup;
    }

    public InlineKeyboardMarkup GenerateFieldsKeyboard()
    {
        return new(new[]
        {
            new[] { InlineKeyboardButton.WithCallbackData("Process"), },
            new[] { InlineKeyboardButton.WithCallbackData("") }
        })
    }
}
