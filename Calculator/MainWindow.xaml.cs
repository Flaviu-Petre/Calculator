using Calculator.ViewModels;
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
using Calculator.Commands;

namespace Calculator
{
    public partial class MainWindow : Window
    {
        private readonly KeyboardInputHandler _keyboardInputHandler;

        public MainWindow()
        {
            InitializeComponent(); 

            MainViewModel viewModel = new MainViewModel();
            this.DataContext = viewModel;
            _keyboardInputHandler = new KeyboardInputHandler(viewModel);

            this.KeyDown += _keyboardInputHandler.Window_KeyDown;
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Calculator v1.0\nRealizat de: Petre Flaviu-Mihai\nGrupa: 10LF332", "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}