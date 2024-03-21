namespace Models.DataFormatProcessors
{
    /// <summary>
    /// Provides functionality to validate data formats such as JSON and CSV.
    /// </summary>
    public class DataFormatValidator
    {
        private Stream? _stream;

        // Private constructor to prevent creating an instance without initializing the stream.
        private DataFormatValidator() { }

        /// <summary>
        /// Initializes a new instance of the DataFormatValidator class with the specified input stream.
        /// </summary>
        /// <param name="stream">The input stream to be validated.</param>
        public DataFormatValidator(Stream stream)
        {
            _stream = stream;
        }

        /// <summary>
        /// Validates the data format as JSON by reading and processing the input stream using JSONProcessing class.
        /// </summary>
        public void ValidateJson() =>
            new JSONProcessing().Read(_stream!);

        /// <summary>
        /// Validates the data format as CSV by reading and processing the input stream using CSVProcessing class.
        /// </summary>
        public void ValidateCsv() =>
            new CSVProcessing().Read(_stream!);
    }
}
