using Microsoft.Extensions.Logging;

namespace AD.FilesManager.Common
{
    public class FileReader
    {
        public class EmptyFileException : Exception
        {
            public EmptyFileException(in string fileName) : base(string.Format(LogMessages.EMPTY_FILE_EXCEPTION, fileName)) { }
        }

        private readonly ILogger<FileReader> _logger;

        public FileReader(ILogger<FileReader> logger)
        {
            _logger = logger;
        }

        public FileContext ReadFile(in string filePath)
        {
            try
            {
                return ReadFileImpl(filePath);
            }
            catch (EmptyFileException ex)
            {
                _logger.LogError(ex.Message);
                return new FileContext(string.Empty, ex.Message, filePath, -1);
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogError(LogMessages.WRONG_FILE_PATH_EXCEPTION, ex.Message);
                return new FileContext(string.Empty, ex.Message, filePath, -1);
            }
            catch (Exception ex)
            {
                _logger.LogError(LogMessages.FAILED_TO_READ_FILE, filePath, ex.Message);
                return new FileContext(string.Empty, ex.Message, filePath, -1);
            }
        }

        protected virtual FileContext ReadFileImpl(in string filePath)
        {
            string fileContent = string.Empty;

            using (StreamReader reader = new(filePath))
            {
                string? fileLine = null;
                int linesCount = 0;

                while ((fileLine = reader.ReadLine()) != null)
                {
                    if (fileLine.Trim() == string.Empty)
                    {
                        continue;
                    }

                    if (fileLine.IsNormalized())
                    {
                        Validate(ref fileLine);
                        fileContent += fileLine;
                        linesCount++;
                    }
                }

                if (linesCount == 0)
                {
                    throw new EmptyFileException(filePath);
                }

                return new FileContext(fileContent, string.Empty, filePath, -1);
            };
        }

        protected virtual void Validate(ref string stringLine) 
        {
            if (stringLine.Contains("//"))
            {
                stringLine += "\n";
            }
        }
    }

    public class FileLineEventArgs : EventArgs
    {
        public string FileLine { get; init; }
        public FileLineEventArgs(string fileLine) => FileLine = fileLine;
    }
}
