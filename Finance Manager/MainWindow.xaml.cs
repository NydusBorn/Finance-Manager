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
using Connection_Manager;
using MessageBox = Wpf.Ui.Controls.MessageBox;

namespace Finance_Manager {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : UiWindow {
        private Controller _controller;
        public MainWindow() {
            _controller = new Controller("Data.db");
            InitializeComponent();
            Application.Current.Dispatcher.Invoke(Validation);
        }
        
        async void Validation()
        {
            await Task.Delay(100);
            
            if (_controller.State != Connector.Database_State.Correct)
            {
                var ms = new MessageBox();
                ms.Content = "Вы хотите пересоздать базу данных?";
                ms.ButtonLeftName = "Да";
                ms.ButtonRightName = "Нет";
                ms.ButtonLeftClick += (sender, args) =>
                {
                    _controller.Cause_Creation();
                    ms.Close();
                };
                ms.ButtonRightClick += (sender, args) =>
                {
                    ms.Close();
                    MainWindow_OnClosing(sender,new CancelEventArgs());
                };
                if (_controller.State != Connector.Database_State.Incorrect)
                {
                    ms.Title = "База данных некорректна.";
                }
                else if (_controller.State != Connector.Database_State.Missing)
                {
                    ms.Title = "База данных несуществует.";
                }
                ms.ShowDialog(); 
            }
            Data_Grid_Users.ItemsSource = _controller.GetUsers().DefaultView;
            Data_Grid_Transactions.ItemsSource = _controller.GetTransactions().DefaultView;
        }

        private void Tab_Users_Select(object sender, RoutedEventArgs e) {
            if (!IsInitialized) return;
            Tc_Tabs.SelectedIndex = 0;
            Tg_Transaction_Sel.IsChecked = false;
            Tg_Reports_Sel.IsChecked = false;
        }
        private void Tab_Transactions_Select(object sender, RoutedEventArgs e) {
            if (!IsInitialized) return;
            Tc_Tabs.SelectedIndex = 1;
            Tg_Users_Sel.IsChecked = false;
            Tg_Reports_Sel.IsChecked = false;
            
        }

        private void Tab_Reports_Select(object sender, RoutedEventArgs e) {
            if (!IsInitialized) return;
            Tc_Tabs.SelectedIndex = 2;
            Tg_Users_Sel.IsChecked = false;
            Tg_Transaction_Sel.IsChecked = false;
        }

        private void MainWindow_OnClosing(object? sender, CancelEventArgs e) {
            _controller.Cause_Close();
            Process.GetCurrentProcess().Kill();
        }

        private void Show_Report_Click(object sender, RoutedEventArgs e) {
            int selectedIdx = Report_ComboBox.SelectedIndex;
            switch (selectedIdx) {
                case 0: {
                        Data_Grid_Rep.ItemsSource = _controller.GetTransactionsPerPeriod(new DateTime(1970,1,5,0,0,0), new DateTime(1970, 1, 19, 0, 0, 0)).DefaultView;
                        break;
                    }
                case 1:
                    {
                        Data_Grid_Rep.ItemsSource = _controller.GetUser(2).DefaultView;
                        break;
                    }
                case 2:
                    {
                        Data_Grid_Rep.ItemsSource = _controller.GetTransactionsPerPeriodAndUser(new DateTime(1970, 1, 5, 0, 0, 0), new DateTime(1970, 1, 17, 0, 0, 0), 1).DefaultView;
                        break;
                    }
            }
        }
    }
}