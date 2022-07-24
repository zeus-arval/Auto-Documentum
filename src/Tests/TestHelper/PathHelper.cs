namespace Tests.TestHelper
{
    public static class PathHelper
    {
        /// <summary>
        /// Extension method for getting directory path move back <code>descendantAmount</code> directories
        /// </summary>
        /// <param name="path">Full path of current directory</param>
        /// <param name="descendantAmount">Amount of directories to go back</param>
        /// <returns>string: Full path</returns>
        internal static string GetDirectoryParentPath(this string path, in int descendantAmount)
        {
            if (path is null || path.Trim() == string.Empty || descendantAmount < 1) return string.Empty;
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                for (int i = 0; i < descendantAmount; i++)
                {
                    directoryInfo = directoryInfo.Parent!;
                }
                return directoryInfo.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Returns string path to a currentPath + childPath
        /// </summary
        /// <returns>string: Full path</returns>
        internal static string GoToChildsDirectory(this string currentPath, in string childPath)
        { 
            if (currentPath is null || currentPath.Trim() == string.Empty) return string.Empty;

            string[] directories = childPath.Split('/', '\\');

            try
            {
                string directoryPath = currentPath;

                for (int i = 0; i < directories.Length; i++)
                {
                    directoryPath = Path.Combine(directoryPath, directories[i]);
                }

                return directoryPath;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
