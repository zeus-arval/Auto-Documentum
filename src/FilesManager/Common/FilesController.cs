using AD.FilesManager.CSharp;
using Microsoft.Extensions.Logging;
using static AD.FilesManager.Common.LogMessages;

namespace AD.FilesManager.Common
{
    public class FilesController
    {
        private readonly ILogger _logger;

        public string FileFormatPattern = CSharpFormats.CSHARP_FILE_FORMAT;
        public FilesController(ILogger<FilesController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Returns Array of C# files' paths
        /// </summary>
        /// <param name="fileFormatPattern">Format of files, which are being searched</param>
        /// <returns></returns>
        public IEnumerable<string> ReturnFilePathArray(in string directoryPath)
        {
            if (Directory.Exists(directoryPath) == false)
            {
                _logger.LogWarning(DIRECTORY_DOESNOT_EXIST, directoryPath);
                return Array.Empty<string>();
            }

            List<string> filePathsList = new List<string>();

            var files = Directory.GetFiles(directoryPath, FileFormatPattern, SearchOption.AllDirectories);
            filePathsList.AddRange(files);

            return filePathsList;
        }

        #region COULD BE USABLE
        private IEnumerable<string> ReturnDirectoriesInDirectory(in string directoryPath)
        {
            List<string> directoryPathsList = new List<string>();
            _logger.LogInformation($"Opening directory - {directoryPath}");

            var directories = Directory.GetDirectories(directoryPath);

            directoryPathsList.AddRange(directories);

            if (directories.Length == 0)
            {
                return Array.Empty<string>();
            }

            for (int i = 0; i < directories.Length; i++)
            {
                directoryPathsList.AddRange(ReturnDirectoriesInDirectory(directories[i]).Where(directory => ValidateDirectory(directory)));
            }

            _logger.LogInformation($"Directories count: {directoryPathsList.Count}");

            return directoryPathsList;
        }

        private bool ValidateDirectory(in string directory)
        {
            //Rules for validation
            return true;
        }
        #endregion
    }
}
