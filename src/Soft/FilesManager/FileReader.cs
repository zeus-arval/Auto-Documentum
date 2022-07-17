using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace AD.Soft.FilesManager
{
    internal class FileReader
    {
        private class EmptyFileException : Exception
        {
            public EmptyFileException(in string fileName) : base(String.Format("File {0} is empty", fileName)) { }
        }

        private readonly ILogger<FileReader> _logger;

        public FileReader(ILogger<FileReader> logger)
        {
            _logger = logger;    
        }

        public string ReadFile(in string filePath)
        {
            try
            {
                string fileContent = string.Empty;

                using (StreamReader reader = new(filePath))
                {
                    string? fileLine = null;

                    while ((fileLine = reader.ReadLine()) != null)
                    {
                        if (fileLine.IsNormalized())
                        {
                            fileContent = fileLine.Trim();
                        }
                    }

                    if (fileContent == string.Empty)
                    {
                        throw new EmptyFileException(filePath);
                    }

                    return fileContent;
                };
            }
            catch(EmptyFileException ex)
            {
                _logger.LogError(ex.Message);
                return string.Empty;
            }
            catch (FileNotFoundException ex) 
            {
                _logger.LogError($"Wrong file path [{filePath}]. ", ex.Message);
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError("Couldn't read a file. ", ex.Message);
                return string.Empty;
            }
        }
    }
}
