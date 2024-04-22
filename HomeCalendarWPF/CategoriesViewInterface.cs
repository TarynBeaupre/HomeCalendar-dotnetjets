using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calendar;

namespace HomeCalendarWPF
{
    public interface CategoriesViewInterface
    {
        /// <summary>
        /// Displays a message box with the specified message.
        /// </summary>
        /// <param name="msg">THe message to display.</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// ShowMessage("You're bad.");
        /// ]]>
        /// </code></example>
        void ShowMessage(string msg);
        /// <summary>
        /// Displays a error box with the specified error.
        /// </summary>
        /// <param name="msg">THe error to display.</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// ShowError("Error: You're bad.");
        /// ]]>
        /// </code></example>
        void ShowError(string msg);
        /// <summary>
        /// Resets the categories form by clearing the text box for category description.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// ResetCategoriesForm();
        /// ]]>
        /// </code></example>
        void ResetCategoriesForm();
        /// <summary>
        /// Sets the options for the category types combo box.
        /// </summary>
        /// <example>
        /// For this example, assume we have a correctly implemented Category.CategoryType[] called categoryTypes
        /// <code>
        /// <![CDATA[
        /// SetComboBoxOptions(catgoryTypes);
        /// ]]>
        /// </code></example>
        void SetComboBoxOptions(Category.CategoryType[] categoryTypes);
    }
}
