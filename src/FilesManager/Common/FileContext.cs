namespace AD.FilesManager.Common
{
    public class FileContext
    {
        public string FileContent { get; init; }
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
