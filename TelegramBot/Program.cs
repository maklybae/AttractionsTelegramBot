using Models;

namespace TelegramBot
{
    internal class Program
    {
        static void Main()
        {
            var dotenv = Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "..", ".env");
            DotEnv.Load(dotenv);

            Console.WriteLine(Environment.GetEnvironmentVariable("ACCESS_TOKEN"));
            
            var csvProcessor = new CSVProcessing();
            var jsonProcessor = new JSONProcessing();
            var data = jsonProcessor.Read(new FileStream("D:\\VisualStudio\\AttractionsTelegramBot\\TelegramBot\\bin\\Debug\\net6.0\\test.json", FileMode.Open));
            var fstream = new FileStream("test-new.json", FileMode.Create);
            using (var stream = jsonProcessor.Write(data))
            {
                stream.WriteTo(fstream);
            }
            fstream.Close();
        }
    }
}