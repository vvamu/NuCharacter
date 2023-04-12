using NuCharacter.ViewModels;
using System.Globalization;
using System.Threading;
using System.Windows;

namespace NuCharacter
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = MainWindowViewModel.Instance;
        }

    }
}
