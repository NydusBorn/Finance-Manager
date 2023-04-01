using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Connection_Manager;
using Wpf.Ui.Controls;
using MessageBox = Wpf.Ui.Controls.MessageBox;

namespace Finance_Manager;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : UiWindow
{
    public Controller _controller;
    
    public MainWindow()
    {
        _controller = new Controller("Data.db");
        InitializeComponent();
        Application.Current.Dispatcher.Invoke(Validation);
    }

    private async void Validation()
    {
        await Task.Delay(100);

        if (_controller.State != Connector.Database_State.Correct)
        {
            var ms = new MessageBox();
            ms.Content = "Вы хотите пересоздать базу данных?";
            ms.ButtonLeftName = "Да";
            ms.ButtonRightName = "Нет";
            var result_ok = false;
            ms.ButtonLeftClick += (sender, args) =>
            {
                _controller.Cause_Creation();
                result_ok = true;
                ms.Close();
            };
            ms.ButtonRightClick += (sender, args) => { ms.Close(); };
            ms.Closed += (sender, args) =>
            {
                ms.Close();
                if (!result_ok) MainWindow_OnClosing(sender, new CancelEventArgs());
            };
            if (_controller.State != Connector.Database_State.Incorrect)
                ms.Title = "База данных некорректна.";
            else if (_controller.State != Connector.Database_State.Missing) ms.Title = "База данных несуществует.";
            ms.ShowDialog();
        }
        Refresh_Data();
    }

    public void Refresh_Data()
    {
        _controller.GetUsers();
        _controller.GetTransactions();
        Data_Grid_Users.ItemsSource = _controller.Users.DefaultView;
        Data_Grid_Transactions.ItemsSource = _controller.Transactions.DefaultView;
    }
    
    private void Tab_Users_Select(object sender, RoutedEventArgs e)
    {
        if (!IsInitialized) return;
        Tc_Tabs.SelectedIndex = 0;
        Tg_Transaction_Sel.IsChecked = false;
        Tg_Reports_Sel.IsChecked = false;
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

    private void Show_Report_Click(object sender, RoutedEventArgs e)
    {
        var selectedIdx = Report_ComboBox.SelectedIndex;
        switch (selectedIdx)
        {
            case 0:
            {
                var rp = new Request_Report();
                rp.MinHeight = 170;
                rp.MaxHeight = 170;
                rp.RDef_User.Height = new GridLength(0);
                rp.ShowDialog();
                Data_Grid_Rep.ItemsSource = _controller
                    .GetTransactionsPerPeriod(new DateTime(1970, 1, 5, 0, 0, 0), new DateTime(1970, 1, 19, 0, 0, 0))
                    .DefaultView;
                break;
            }
            case 1:
            {
                var rp = new Request_Report();
                rp.MinHeight = 170;
                rp.MaxHeight = 170;
                rp.RDef_Date.Height = new GridLength(0);
                rp.ShowDialog();
                Data_Grid_Rep.ItemsSource = _controller.GetUser(2).DefaultView;
                break;
            }
            case 2:
            {
                var rp = new Request_Report();
                rp.MinHeight = 220;
                rp.MaxHeight = 220;
                rp.ShowDialog();
                Data_Grid_Rep.ItemsSource = _controller
                    .GetTransactionsPerPeriodAndUser(new DateTime(1970, 1, 5, 0, 0, 0),
                        new DateTime(1970, 1, 17, 0, 0, 0), 1).DefaultView;
                break;
            }
        }
    }

    private void User_Add(object sender, RoutedEventArgs e)
    {
        var us = new Add_User(this);
        us.ShowDialog();
    }

    private void User_Remove(object sender, RoutedEventArgs e)
    {
        List<int> users = new();
        foreach (DataRowView row in Data_Grid_Users.SelectedItems)
        {
            users.Add(int.Parse((string)row.Row[0]));
        }
        _controller.Remove_Users(users);
        Refresh_Data();
    }

    private void Transaction_Add(object sender, RoutedEventArgs e)
    {
        var tr = new Add_Transaction(this);
        tr.ShowDialog();
    }

    private void Transaction_Remove(object sender, RoutedEventArgs e)
    {
        List<int> transactions = new();
        foreach (DataRowView row in Data_Grid_Transactions.SelectedItems)
        {
            transactions.Add(int.Parse((string)row.Row[0]));
        }
        _controller.Remove_Transactions(transactions);
        Refresh_Data();
    }

}