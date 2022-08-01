using Microsoft.Extensions.Logging;

namespace AD.FilesManager.Common
{
    public class FilesController
    {
        private readonly ILogger _logger;
        public FilesController(ILogger<FilesController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Returns Array of C# files' paths
        /// </summary>
        /// <param name="fileFormatPattern">Format of files, which are being searched</param>
        /// <returns></returns>
        public IEnumerable<string> ReturnFilePathArray(in string directoryPath, in string fileFormatPattern)
        {
            if (Directory.Exists(directoryPath) == false)
            {
                _logger.LogWarning($"Directory [{directoryPath}] doesn't exist");
                return Array.Empty<string>();
            }

            List<string> filePathsList = new List<string>();

            var files = Directory.GetFiles(directoryPath, fileFormatPattern, SearchOption.AllDirectories);
            filePathsList.AddRange(files);

            foreach (var fileName in filePathsList)
            {
                _logger.LogInformation($"File = {fileName}");
            }

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
