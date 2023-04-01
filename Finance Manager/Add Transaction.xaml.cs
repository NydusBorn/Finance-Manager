﻿using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Wpf.Ui.Controls;
using MessageBox = Wpf.Ui.Controls.MessageBox;

namespace Finance_Manager;

public partial class Add_Transaction : UiWindow
{
    private MainWindow Parent;
    private Brush std;
    public Add_Transaction(MainWindow parent)
    {
        InitializeComponent();
        Parent = parent;
        foreach (DataRow row in Parent._controller.Users.Rows)
        {
            Cb_Transaction_User.Items.Add(row[1]);
        }

        std = Tx_Transaction_Description.Background;
    }
    
    public void Create(object sender, RoutedEventArgs e)
    {
        bool success = true;
        if (!Check_Date())
        {
            success = false;
        }
        if (!Check_Price())
        {
            success = false;
        }

        if (!success) return;
        Parent._controller.Add_Transaction(int.Parse((string)Parent._controller.Users.Rows[Cb_Transaction_User.SelectedIndex][0]),Tx_Transaction_Description.Text,(int)double.Parse(Tx_Transaction_Price.Text), Parent._controller.Time(Dp_Transaction_Date.SelectedDate.Value));
        Parent.Refresh_Data();
        Signal_Update();
    }

    

    private void Cancel(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
    
    async void Signal_Update()
    {
        Tb_Status.Text = "Создано";
        await Task.Delay(1000);
        Tb_Status.Text = "Ожидание";
    }

    private void Dp_Transaction_Date_OnSelectedDateChanged(object? sender, SelectionChangedEventArgs e)
    {
        Check_Date();
    }

    private bool Check_Date()
    {
        if (Dp_Transaction_Date.SelectedDate == null)
        {
            Dp_Transaction_Date.Background = Brushes.Red;
            Dp_Transaction_Date.ToolTip = "Требуется задать дату транзакции.";
            return false;
        }
        else
        {
            Dp_Transaction_Date.Background = std;
            Dp_Transaction_Date.ToolTip = null;
            return true;
        }
    }
    private void Tx_Transaction_Price_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        Check_Price();
    }
    private bool Check_Price()
    {
        if (!double.TryParse(Tx_Transaction_Price.Text,out _))
        {
            Tx_Transaction_Price.Background = Brushes.Red;
            Tx_Transaction_Price.ToolTip = "Должно быть числом.";
            return false;
        }
        else
        {
            Tx_Transaction_Price.Background = std;
            Tx_Transaction_Price.ToolTip = null;
            return true;
        }
    }
}