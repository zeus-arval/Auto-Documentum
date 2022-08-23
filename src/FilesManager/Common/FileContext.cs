namespace AD.FilesManager.Common
{
    public class FileContext
    {
        /// <summary>
        /// Content of file in string type
        /// </summary>
        public string FileContent { get; init; }
        /// <summary>
        /// If FileContent reading was successful, is string.Empty
        /// </summary>
        public string ErrorMessage { get; init; }
        public string FilePath { get; init; }
        public int LinesCount { get; init; }

        public FileContext(string fileContent, string errorMessage, string filePath, int linesCount)
        {
            FileContent = fileContent;
            ErrorMessage = errorMessage;
            FilePath = filePath;
            LinesCount = linesCount;
        }
    }
}
