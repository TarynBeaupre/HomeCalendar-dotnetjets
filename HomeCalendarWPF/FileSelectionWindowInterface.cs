using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalendarWPF
{
    /// <summary>
    /// Represents the interface for the View component in MVP design.
    /// </summary>
    public interface FileSelectionWindowInterface
    {
        /// <summary>
        /// Sets the file path for the calendar and updates the path text block.
        /// </summary>
        /// <param name="path">THe path to the chosen file.</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// SetDirectoryText("./hello.db");
        /// ]]>
        /// </code></example>
        void SetDirectoryText(string path);
        /// <summary>
        ///Enables confirm button, once file is chosen or created
        /// </summary>
        /// <summary>
        /// Sets the filePath and if new database to params
        /// </summary>
        /// <param name="filePath">Path to the chosen file</param>
        /// <param name="newDB">Indicates if need new database, if true create new database</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// if (filepath && newDB)
        ///     SetInitializationParams(filepath, newDB);
        /// ]]>
        /// </code></example>
        void SetInitializationParams(string filePath, bool newDB);
        /// <summary>
        /// Shows error messages to the user
        /// </summary>
        /// <param name="message">Error message</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// if (filepath && newDB)
        ///     SetInitializationParams(filepath, newDB);
        /// else
        ///     ShowError("Error: FilePath is null");
        /// ]]>
        /// </code></example>
        void ShowError(string message);
        /// <summary>
        /// Closes the current window
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// if (filepath && newDB)
        ///     SetInitializationParams(filepath, newDB);
        /// else
        ///     ShowError("Error: FilePath is null");
        ///     CloseWindow();
        /// ]]>
        /// </code></example>
        void CloseWindow();
        /// <summary>
        /// Gets the path of the directory depending on path text block
        /// </summary>
        /// <returns>A string of the path to the path</returns>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// if (filepath && newDB)
        ///     SetInitializationParams(filepath, newDB);
        /// path = GetFilePath()
        /// ]]>
        /// </code></example>
        string GetFilePath();
        /// <summary>
        /// Enables confirm button 
        /// </summary>
        /// <example>
        /// <code>
        /// <!<![CDATA[
        /// EnableConfirmButton();
        /// ]]>
        /// </code>
        /// </example>
        void EnableConfirmButton();
    }
}
