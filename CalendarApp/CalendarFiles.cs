// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Calendar
{
    /// <summary>
    /// Manage the files used in the Calendar project.
    /// </summary>
    public class CalendarFiles
    {
        private static String DefaultSavePath = @"Calendar\";
        private static String DefaultAppData = @"%USERPROFILE%\AppData\Local\";

        // ====================================================================
        // verify that the name of the file, or set the default file, and 
        // is it readable?
        // throws System.IO.FileNotFoundException if file does not exist
        // ====================================================================
        /// <summary>
        /// Verifies the validity of a file name and path to read data from.
        /// </summary>
        /// <param name="FilePath">The full file path of the file to read. If null, uses the default file path in AppData.</param>
        /// <param name="DefaultFileName">The default file name to read Calendar data from.</param>
        /// <returns>A valid file path.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the file does not exist.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the path provided is null.</exception>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// try
        /// {
        ///     string filePath = @"C:\CustomPath\MyCalendarData.txt";
        ///     string defaultFileName = "calendar_data.txt";
        ///     string validReadFilePath = Calendar.CalendarFiles.VerifyReadFromFileName(filePath, defaultFileName);
        /// }
        /// catch (FileNotFoundException ex){
        ///     Console.WriteLine(ex.Message);
        /// }]]>
        /// </code></example> 
        /// 
        public static String VerifyReadFromFileName(String? FilePath, String DefaultFileName)
        {

            // ---------------------------------------------------------------
            // if file path is not defined, use the default one in AppData
            // ---------------------------------------------------------------
            if (FilePath == null)
            {
                FilePath = Environment.ExpandEnvironmentVariables(DefaultAppData + DefaultSavePath + DefaultFileName);
            }

            // ---------------------------------------------------------------
            // does FilePath exist?
            // ---------------------------------------------------------------
            if (!File.Exists(FilePath))
            {
                throw new FileNotFoundException("ReadFromFileException: FilePath (" + FilePath + ") does not exist");
            }

            // ----------------------------------------------------------------
            // valid path
            // ----------------------------------------------------------------
            return FilePath;

        }

        // ====================================================================
        // verify that the name of the file, or set the default file, and 
        // is it writable
        // ====================================================================
        /// <summary>
        /// Verifies the validity of a file name and path to write data to.
        /// </summary>
        /// <param name="FilePath">The path of the file leading to the file to write data to. Can be null.</param>
        /// <param name="DefaultFileName">The default file name of the file to write data to.</param>
        /// <remarks> If <paramref name="FilePath"/> is null, sets the file path to the default and creates the necessary default directories if they do not exist.</remarks>
        /// <returns>A valid file path to write data to.</returns>
        /// <exception cref="Exception">Thrown if the program was unable to write to the file.</exception>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// try
        /// {
        ///     string filePath = @"C:\CustomPath\MyCalendarData.txt";
        ///     string defaultFileName = "calendar_data.txt";
        ///     string validWriteFilePath = Calendar.CalendarFiles.VerifyWriteToFileName(filePath, defaultFileName);
        /// }
        /// catch (Exception ex)
        /// {
        ///     Console.WriteLine(ex.Message);
        /// }]]>
        /// </code></example>
        public static String VerifyWriteToFileName(String? FilePath, String DefaultFileName)
        {
            // ---------------------------------------------------------------
            // if the directory for the path was not specified, then use standard application data
            // directory
            // ---------------------------------------------------------------
            if (FilePath == null)
            {
                // create the default appdata directory if it does not already exist
                String tmp = Environment.ExpandEnvironmentVariables(DefaultAppData);
                if (!Directory.Exists(tmp))
                {
                    Directory.CreateDirectory(tmp);
                }

                // create the default Calendar directory in the appdirectory if it does not already exist
                tmp = Environment.ExpandEnvironmentVariables(DefaultAppData + DefaultSavePath);
                if (!Directory.Exists(tmp))
                {
                    Directory.CreateDirectory(tmp);
                }

                FilePath = Environment.ExpandEnvironmentVariables(DefaultAppData + DefaultSavePath + DefaultFileName);
            }

            // ---------------------------------------------------------------
            // does directory where you want to save the file exist?
            // ... this is possible if the user is specifying the file path
            // ---------------------------------------------------------------
            String? folder = Path.GetDirectoryName(FilePath);
            String delme = Path.GetFullPath(FilePath);
            if (!Directory.Exists(folder))
            {
                throw new Exception("SaveToFileException: FilePath (" + FilePath + ") does not exist");
            }

            // ---------------------------------------------------------------
            // can we write to it?
            // ---------------------------------------------------------------
            if (File.Exists(FilePath))
            {
                FileAttributes fileAttr = File.GetAttributes(FilePath);
                if ((fileAttr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    throw new Exception("SaveToFileException:  FilePath(" + FilePath + ") is read only");
                }
            }

            // ---------------------------------------------------------------
            // valid file path
            // ---------------------------------------------------------------
            return FilePath;
        }
    }
}
