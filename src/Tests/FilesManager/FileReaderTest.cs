using AD.Factories;
using AD.FilesManager;
using Microsoft.Extensions.Logging;
using Tests.TestHelper;

namespace Tests.FilesManager
{
    [TestFixture]
    internal class FileReaderTest
    {
        private class FileReaderMock : FileReader
        {
            public FileReaderMock(ILogger<FileReaderMock> logger) : base(logger) { }

            public void ReadFileForTest(in string filePath)
            {
                ReadFileImpl(filePath);
            }
        }

        #region Content of TestFile
        private List<string> fileLinesList = new List<string>
        {
"namespace Tests.TestCases",
"{",
    "public class TestFile",
    "{",
        "private int _testFieldInt;",
        "public TestFile()",
        "{",
            "_testFieldInt = 1;",
        "}",
    "}",
"}",
        };
        #endregion

        private Queue<string>? fileLineQueue;
        private FileReaderMock _fileReader;
        private string TEST_DIRECTORY_PATH = AppDomain.CurrentDomain.BaseDirectory.GetDirectoryParentPath( 3).GoToChildsDirectory("TestCases") ;

        [SetUp]
        public void SetUp()
        {
            MainFactory factory = new();
            _fileReader = new FileReaderMock(factory.CreateStubLogger<FileReaderMock>());
            _fileReader.FileLineSent += CompareCodeLines;
        }

        private void CompareCodeLines(object? sender, FileLineEventArgs e)
        {
            fileLineQueue!.TryDequeue(out var fileLine);
            fileLine ??= string.Empty;
            Assert.That(fileLine, Is.EqualTo(e.FileLine.Trim()));
        }

        private IEnumerable<string> ReturnTestFileLines()
        {
            foreach (string line in fileLinesList)
            {
                yield return line;
            }
        }

        [TestCase(@"WrongFile.txt", typeof(FileNotFoundException), TestName = "Wrong File Reading")]
        [TestCase(@"TestEmptyFile.txt", typeof(FileReader.EmptyFileException), TestName = "Empty File Reading")]
        [TestCase(@"TestFile.txt", TestName = "Test File Reading")]
        public void TestReadFile(string filePath, Type? exceptionType = null)
        {
            string fullPath = TEST_DIRECTORY_PATH.GoToChildsDirectory(filePath);
            if (exceptionType is null)
            {
                fileLineQueue = new(ReturnTestFileLines());
                _fileReader.ReadFileForTest(fullPath);
            }
            else
            {
                Assert.Throws(exceptionType, () => _fileReader.ReadFileForTest(fullPath));
            }
        }
    }
}
