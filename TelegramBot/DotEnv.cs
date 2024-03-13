namespace TelegramBot;

internal static class DotEnv
{
    public static void Load(string filePath)
    {
        if (!File.Exists(filePath))
            throw new ArgumentException("dotenv doesn't exists", filePath);

        foreach (var line in File.ReadAllLines(filePath))
        {
            var parts = line.Split(
                '=',
                StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
                continue;

            Environment.SetEnvironmentVariable(parts[0], parts[1]);
        }

    }
}
