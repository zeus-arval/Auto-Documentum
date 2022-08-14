namespace AD.FilesManager.Common
{
    public static class LogMessages
    {
        public const string GENERIC_ERROR_OCCURED = "Error occured. {0}";

#region File Exceptions
        public const string FILE_CONTENT_ARRAY_LENGTH_IS_ZERO = "No one file in [{directoryPath}] was read";
        public const string FAILED_TO_READ_FILE = "Failed to read file {0}. {1}";
        public const string EMPTY_FILE_EXCEPTION = "File {0} is empty";
        public const string WRONG_FILE_PATH_EXCEPTION = "Wrong file path [{filePath}]. ";
        public const string DIRECTORY_DOESNOT_EXIST = "Directory [{directoryPath}] doesn't exist";

#endregion

        #region AST Reading Exceptions
        public const string CANNOT_GET_CLASS_NAME = "Can not get class name";
        public const string CANNOT_GET_NAMESPACE_NAME = "Can not get namespace";

        public const string FILE_DOESNOT_CONTAIN_ANY_NAMESPACE = "{filePath} doesn't contain any namespace.";
        public const string FILE_DOESNOT_CONTAIN_ANY_CLASS = "{filePath} doesn't contain any class";

#endregion
    }
}
