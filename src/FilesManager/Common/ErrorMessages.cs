namespace AD.FilesManager.Common
{
    public static class ErrorMessages
    {
#region File Exceptions
        public const string FILE_CONTENT_ARRAY_LENGTH_IS_ZERO = "No one file in [{directoryPath}] was read";
        public const string FAILED_TO_READ_FILE = "Failed to read file {0}. {1}";
        public const string EMPTY_FILE_EXCEPTION = "File {0} is empty";
        public const string WRONG_FILE_PATH_EXCEPTION = "Wrong file path [{filePath}]. ";
#endregion

    }
}
