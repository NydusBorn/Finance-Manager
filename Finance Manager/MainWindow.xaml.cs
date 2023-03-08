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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Controls;

namespace Finance_Manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : UiWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Tab_Users_Select(object sender, RoutedEventArgs e)
        {
            if (!IsInitialized) return;
            Tc_Tabs.SelectedIndex = 0;
            Tg_Transaction_Sel.IsChecked = false;
        }
        private void Tab_Transactions_Select(object sender, RoutedEventArgs e)
        {
            if (!IsInitialized) return;
            Tc_Tabs.SelectedIndex = 1;
            Tg_Users_Sel.IsChecked = false;
        }
    }
}