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
        void ShowMessage(string msg);
        void ShowError(string msg);
        void ResetCategoriesForm();
        void SetComboBoxOptions(Category.CategoryType[] categoryTypes);
    }
}
