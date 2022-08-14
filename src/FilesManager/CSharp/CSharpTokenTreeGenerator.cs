using AD.Aids.Factories;
using AD.FilesManager.Common;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;

namespace AD.FilesManager.CSharp
{
    public class CSharpTokenTreeGenerator
    {
        private class CSharpFile
        {
            public string FilePath { get; init; }
            public string Content { get; init; }
            public CSharpFile(string filePath, string content)
            {
                FilePath = filePath;
                Content = content;
            }
        }

        private const string CSharpFileFormat = "*.cs";

        private readonly ILogger _logger;
        private readonly FileReader _filesReader;
        private readonly FilesController _filesController;

        public CSharpTokenTreeGenerator()
        {
            MainFactory factory = new();

            _logger = factory.CreateLogger<CSharpTokenTreeGenerator>();
            _filesReader = new FileReader(factory.CreateLogger<FileReader>());
            _filesController = new FilesController(factory.CreateLogger<FilesController>());
        }

        public IEnumerable<CSharpTokenTree> CreateSyntaxTreeArray(string directoryPath)
        {
            CSharpFile[] fileContentArray = GetFileContentArray(directoryPath);

            if (fileContentArray.Length == 0)
            {
                return new CSharpTokenTree[1]{ (new CSharpTokenTree(LogMessages.FILE_CONTENT_ARRAY_LENGTH_IS_ZERO, null)) };
            }

            CSharpTokenTree[] cSharpSyntaxTreeArray = new CSharpTokenTree[fileContentArray.Length];

            for(int i = 0; i < cSharpSyntaxTreeArray.Length; i++)
            {
                cSharpSyntaxTreeArray[i] = new CSharpTokenTree(string.Empty, CSharpSyntaxTree.ParseText(text: fileContentArray[i].Content, path: fileContentArray[i].FilePath));
            }

            return cSharpSyntaxTreeArray;
        }


        private CSharpFile[] GetFileContentArray(in string directoryPath)
        {
            IEnumerable<string> filePaths = _filesController.ReturnFilePathArray(directoryPath, CSharpFileFormat);
            List<CSharpFile> fileContentList = new List<CSharpFile>();

            foreach (string path in filePaths)
            {
                FileContext fileContext = _filesReader.ReadFile(path);

                if (fileContext.ErrorMessage == String.Empty)
                {
                    _logger.LogInformation("File [{filePath}] reading was successfull. {linesCount} lines of code were read.", fileContext.FilePath, fileContext.LinesCount);
                    fileContentList.Add(new CSharpFile(fileContext.FilePath, fileContext.FileContent));
                    _logger.LogInformation("Added File [{filePath}] content to FileContentList ({count})", fileContext.FilePath, fileContentList.Count);
                }
            }

            return fileContentList.ToArray();
        }
    }
}
