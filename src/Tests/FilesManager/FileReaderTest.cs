using AD.Aids.Factories;
using AD.FilesManager.Common;
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

            public FileContext ReadFileForTest(in string filePath)
            {
                return ReadFileImpl(filePath);
            }
            protected override void Validate(ref string fileLine)
            {
                fileLine = fileLine.Trim();
            }
        }

        #region Content of TestFile
        private const string EXPECTED_FILE_CONTENT = "namespace Tests.TestCases"+
"{"+
    "public class TestFile"+
    "{"+
        "private int _testFieldInt;"+
        "public TestFile()"+
        "{"+
            "_testFieldInt = 1;"+
        "}"+
    "}"+
"}";
        #endregion

        private FileReaderMock _fileReader;
        private string TEST_DIRECTORY_PATH = AppDomain.CurrentDomain.BaseDirectory.GetDirectoryParentPath( 3).GoToChildsDirectory("TestCases") ;

        [SetUp]
        public void SetUp()
        {
            MainFactory factory = new();
            _fileReader = new FileReaderMock(factory.CreateStubLogger<FileReaderMock>());
        }

        [TestCase(@"WrongFile.txt", typeof(FileNotFoundException), TestName = "Wrong File Reading")]
        [TestCase(@"TestEmptyFile.txt", typeof(FileReader.EmptyFileException), TestName = "Empty File Reading")]
        [TestCase(@"TestFile.txt", null, 11, TestName = "Test File Reading")]
        public void TestReadFile(string filePath, Type? exceptionType = null, int? linesCount = null)
        {
            string fullPath = TEST_DIRECTORY_PATH.GoToChildsDirectory(filePath);
            if (exceptionType is null)
            {
                FileContext actualContext = _fileReader.ReadFileForTest(fullPath);
                Assert.That(actualContext.FileContent, Is.EqualTo(EXPECTED_FILE_CONTENT));

                if (linesCount is not null)
                {
                    Assert.That(actualContext.LinesCount, Is.EqualTo(linesCount));
                }
            }
            else
            {
                Assert.Throws(exceptionType, () => _fileReader.ReadFileForTest(fullPath));
            }
        }
    }
}
