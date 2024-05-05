using Xunit;
using Calendar;
using HomeCalendarWPF.Interfaces.Views;
using HomeCalendarWPF.Presenters;

namespace CalendarWPFTesting
{
    public class TestCategoriesPresenter : CategoriesViewInterface
    {
        public bool calledResetCategoriesForm;
        public string lastMessage;
        public string lastError;
        public Category.CategoryType[] comboBoxOptions;

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

        //TODO: Bugs when trying to add cat to model
        [Fact]
        public void AddNewCategory_ValidInput_AddsCategory()
        {
            // Arrange
            TestCategoriesPresenter view = new TestCategoriesPresenter();
            CategoriesPresenter presenter = new CategoriesPresenter(view, "./../../../test.db");

            // Act
            presenter.AddNewCategory("Work", Category.CategoryType.Event);

            // Assert
            Assert.True(view.calledResetCategoriesForm);
            Assert.Equal("Category successfully added!", view.lastMessage);
        }

        [Fact]
        public void AddNewCategory_EmptyDescription_ShowsError()
        {
            // Arrange
            TestCategoriesPresenter view = new TestCategoriesPresenter();
            CategoriesPresenter presenter = new CategoriesPresenter(view, "test.db");

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
            CategoriesPresenter presenter = new CategoriesPresenter(view, "test.db");

            // Act
            presenter.GetCategoryTypes();

            // Assert
            Assert.Equal(4, view.comboBoxOptions.Length); 
        }
    }
}
