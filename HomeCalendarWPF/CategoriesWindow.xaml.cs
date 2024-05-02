using Calendar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HomeCalendarWPF
{
    /// <summary>
    /// Interaction logic for CategoriesWindow.xaml
    /// </summary>
    public partial class CategoriesWindow : Window , CategoriesViewInterface
    {
        private readonly CategoriesPresenter presenter;

        public static int previousCategoryTypeIndex = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoriesWindow"/> class.
        /// </summary>
        /// <param name="darkMode">Specifies which theme should be picked for Window, if true then display dark mode.</param>
        public CategoriesWindow(HomeCalendar model, bool darkMode)
        {
            InitializeComponent();
            txbCalendarFileinCategories.Text = ((MainWindow)Application.Current.MainWindow).calendarFiletxb.Text;
            this.presenter = new CategoriesPresenter(this, model, txbCalendarFileinCategories.Text);

            this.Initialize();

            SetTheme(darkMode);
        }
        /// <summary>
        /// Displays a message box with the specified message.
        /// </summary>
        /// <param name="msg">THe message to display.</param>
        ///         /// <example>
        /// <code>
        /// <![CDATA[
        /// ShowMessage("You're bad.");
        /// ]]>
        /// </code></example>
        public void ShowMessage(string msg)
        {
            MessageBox.Show(msg, "Category added", MessageBoxButton.OK, MessageBoxImage.Information);
        }
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
        public void ShowError(string msg)
        {
            MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        /// <summary>
        /// Resets the categories form by clearing the text box for category description.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// ResetCategoriesForm();
        /// ]]>
        /// </code></example>
        public void ResetCategoriesForm()
        {
            txbCategoryDescription.Text = "";
        }
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
        public void SetComboBoxOptions(Category.CategoryType[] categoryTypes)
        {
            categoryTypesCmb.SelectedIndex = previousCategoryTypeIndex;
            categoryTypesCmb.ItemsSource = categoryTypes;
        }
        private void Btn_Click_Cancel_Category(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Btn_Click_Add_Category(object sender, RoutedEventArgs e)
        {
            int index = categoryTypesCmb.SelectedIndex + 1;
            presenter.AddNewCategory(txbCategoryDescription.Text, (Category.CategoryType)index);
            Close();
        }
        private void Initialize()
        {
            this.presenter.GetCategoryTypes();
        }
        private void SetTheme(bool darkmode)
        {
            if (darkmode)
            {
                child_window_background_theme.ImageSource = new BitmapImage(new Uri("../../../images/stardew-backdrop-dark.jpg", UriKind.Relative));
                menu_gradient.Color = Colors.Gray;
                light_theme_star.Visibility = Visibility.Collapsed;
                dark_theme_star.Visibility = Visibility.Visible;
            }
            else
            {
                child_window_background_theme.ImageSource = new BitmapImage(new Uri("../../../images/stardew-backdrop.jpg", UriKind.Relative));
                menu_gradient.Color = Colors.LightGreen;
                light_theme_star.Visibility = Visibility.Visible;
                dark_theme_star.Visibility = Visibility.Collapsed;
            }
        }
    }
}
