namespace Models.DataFormatProcessors
{
    public class DataFormatValidator
    {
        public void ValidateJson(Stream stream) =>
            new JSONProcessing().Read(stream);

        public void ValidateCsv(Stream stream) =>
            new CSVProcessing().Read(stream);

    }
}
