using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calendar;
using HomeCalendarWPF.Interfaces.ViewInterfaces;

namespace HomeCalendarWPF.Interfaces.Views
{
    public interface CategoriesViewInterface : ViewInterface
    {
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
