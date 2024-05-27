using Xunit;
using Calendar;
using HomeCalendarWPF.Interfaces.Views;
using HomeCalendarWPF.Presenters;
using DocumentFormat.OpenXml.EMMA;

namespace CalendarWPFTesting
{
    public class TestCategoriesPresenter : CategoriesViewInterface
    {
        public bool calledResetCategoriesForm;
        public string lastMessage;
        public string lastError;
        public Category.CategoryType[] comboBoxOptions;
        public HomeCalendar testcalendar = new HomeCalendar("test.db");

        public void ShowMessage(string msg)
        {
            lastMessage = msg;
        }

        public void ShowError(string msg)
        {
            lastError = msg;
        }

        public void ResetCategoriesForm()
        {
            calledResetCategoriesForm = true;
        }

        public void SetComboBoxOptions(Category.CategoryType[] categoryTypes)
        {
            comboBoxOptions = categoryTypes;
        }

        [Fact]
        public void AddNewCategory_EmptyDescription_ShowsError()
        {
            // Arrange
            TestCategoriesPresenter view = new TestCategoriesPresenter();
            CategoriesPresenter presenter = new CategoriesPresenter(view, testcalendar, "test.db");

            // Act
            presenter.AddNewCategory("", Category.CategoryType.Event);

            // Assert
            Assert.False(view.calledResetCategoriesForm);
            Assert.Equal("Please provide a description for the category.", view.lastError);
        }


        [Fact]
        public void GetCategoryTypes_ReturnsAllCategoryTypes()
        {
            // Arrange
            TestCategoriesPresenter view = new TestCategoriesPresenter();
            CategoriesPresenter presenter = new CategoriesPresenter(view, testcalendar, "test.db");

            // Act
            presenter.GetCategoryTypes();

            // Assert
            Assert.Equal(4, view.comboBoxOptions.Length); 
        }
    }
}
