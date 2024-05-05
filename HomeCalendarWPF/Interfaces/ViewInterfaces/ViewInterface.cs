using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalendarWPF.Interfaces.ViewInterfaces
{
    public interface ViewInterface
    {
        /// <summary>
        /// Shows error messages to the user
        /// </summary>
        /// <param name="message">Error message</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// ShowMessage("Error, you're bad.");
        /// ]]>
        /// </code></example>
        void ShowMessage(string message);
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
        void ShowError(string error);
    }
}
