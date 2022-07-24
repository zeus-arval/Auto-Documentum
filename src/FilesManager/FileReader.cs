using Microsoft.Extensions.Logging;

namespace AD.FilesManager
{
    public class FileReader
    {
        public class EmptyFileException : Exception
        {
            public EmptyFileException(in string fileName) : base(String.Format("File {0} is empty", fileName)) { }
        }

        private readonly ILogger<FileReader> _logger;

        public FileReader(ILogger<FileReader> logger)
        {
            _logger = logger;
        }

        public void ReadFile(in string filePath)
        {
            try
            {
                ReadFileImpl(filePath);
            }
            catch(EmptyFileException ex)
            {
                _logger.LogError(ex.Message);
                return;
            }
            catch (FileNotFoundException ex) 
            {
                _logger.LogError("Wrong file path [{filePath}]. ", ex.Message);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError("Couldn't read a file. ", ex.Message);
                return;
            }
        }

        protected virtual void ReadFileImpl(in string filePath)
        {
            string fileContent = string.Empty;

            using (StreamReader reader = new(filePath))
            {

                string? fileLine = null;
                int linesCount = 0;

                while ((fileLine = reader.ReadLine()) != null)
                {
                    if (fileLine.Trim() == String.Empty)
                    {
                        continue;
                    }

                    if (fileLine.IsNormalized())
                    {
                        Validate(fileLine, ref linesCount);
                    }
                }

                if (linesCount == 0)
                {
                    throw new EmptyFileException(filePath);
                }
            };
        }

        private void Validate(string fileLine, ref int linesCount)
        {
            FileLineEventArgs args = new FileLineEventArgs(fileLine);
            OnFileLineSent(args);
            linesCount++;
        }

        private void OnFileLineSent(FileLineEventArgs e)
        {
            EventHandler<FileLineEventArgs>? handler = FileLineSent ?? null; 

            if (handler != null)
            {
                handler!(this, e);
            }
        }

        public event EventHandler<FileLineEventArgs>? FileLineSent;
    }

    public class FileLineEventArgs : EventArgs
    {
        public string FileLine { get; init; }
        public FileLineEventArgs(string fileLine) => FileLine = fileLine;
    }
}
