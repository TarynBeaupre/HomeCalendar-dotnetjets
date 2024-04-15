using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HomeCalendarWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            string baseDir = Environment.CurrentDirectory;
            InitializeComponent();
            calendarFiletxb.Text = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\..\\CalendarApp\\test.calendar")); ;
        }
        private void OpenEvent(object sender, RoutedEventArgs e)
        {
            EventsWindow eventWindow = new EventsWindow();
            eventWindow.Show();
        }

        private void Btn_OpenFileExplorer(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "C:\\Users");
        }
    }
}