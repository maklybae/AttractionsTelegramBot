using DataManager;
using DataManager.Mapping;
using DataManager.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot;

internal class KeyboardsManager
{
    public InlineKeyboardMarkup GenerateInlineKeyboardFiles(Chat chat)
    {
        InlineKeyboardMarkup inlineKeyboardMarkup;

        using var db = new DatabaseContext();
        var querySet = db.Files.Where(s => s.Chat == chat && !s.IsSource).
            OrderByDescending(e => e.CreatedAt).
            Take(3);
        if (querySet.Count() > 0)
        {
            var recentlyRow = querySet.
                Select(file => InlineKeyboardButton.WithCallbackData(file.Description!, file.FileId.ToString()));

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

    public InlineKeyboardMarkup GenerateFieldsKeyboard() =>
        new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(DataField.Name.Title, DataField.Name.Id.ToString()),
                InlineKeyboardButton.WithCallbackData(DataField.Photo.Title, DataField.Photo.Id.ToString()),
                InlineKeyboardButton.WithCallbackData(DataField.AdmArea.Title, DataField.AdmArea.Id.ToString())
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(DataField.District.Title, DataField.District.Id.ToString()),
                InlineKeyboardButton.WithCallbackData(DataField.Location.Title, DataField.Location.Id.ToString()),
                InlineKeyboardButton.WithCallbackData(DataField.RegistrationNumber.Title, DataField.RegistrationNumber.Id.ToString())
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(DataField.State.Title, DataField.State.Id.ToString()),
                InlineKeyboardButton.WithCallbackData(DataField.LocationType.Title, DataField.LocationType.Id.ToString()),
                InlineKeyboardButton.WithCallbackData(DataField.GlobalId.Title, DataField.GlobalId.Id.ToString())
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Run Action", "RUN")
            }
        });

    public InlineKeyboardMarkup GenerateFileFormatKeyboard() =>
        new InlineKeyboardMarkup(new[]
        {
            InlineKeyboardButton.WithCallbackData("JSON", "JSON"),
            InlineKeyboardButton.WithCallbackData("CSV", "CSV"),
        });
}
