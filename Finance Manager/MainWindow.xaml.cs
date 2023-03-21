using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using Interface;
using Wpf.Ui.Controls;
using System.Data;

namespace Finance_Manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : UiWindow
    {
        private Controller _controller;
        public MainWindow()
        {
            _controller = new Controller("Data.db");
            InitializeComponent();
        }

        private void Tab_Users_Select(object sender, RoutedEventArgs e)
        {
            if (!IsInitialized) return;
            Tc_Tabs.SelectedIndex = 0;
            Tg_Transaction_Sel.IsChecked = false;
            Tg_Reports_Sel.IsChecked = false;
            Data_Grid.ItemsSource = _controller.GetUsers().DefaultView;
            Data_Grid.DataContext = _controller.GetUsers().DefaultView;
        }
        private void Tab_Transactions_Select(object sender, RoutedEventArgs e)
        {
            if (!IsInitialized) return;
            Tc_Tabs.SelectedIndex = 1;
            Tg_Users_Sel.IsChecked = false;
            Tg_Reports_Sel.IsChecked = false;
        }

        private void Tab_Reports_Select(object sender, RoutedEventArgs e)
        {
            if (!IsInitialized) return;
            Tc_Tabs.SelectedIndex = 2;
            Tg_Users_Sel.IsChecked = false;
            Tg_Transaction_Sel.IsChecked = false;
        }

        private void MainWindow_OnClosing(object? sender, CancelEventArgs e)
        {
            _controller.Cause_Close();
            Process.GetCurrentProcess().Kill();
        }
    }
}