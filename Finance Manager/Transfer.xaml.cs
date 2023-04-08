using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Wpf.Ui.Controls;
using MessageBox = Wpf.Ui.Controls.MessageBox;

namespace Finance_Manager;

public partial class Transfer : UiWindow
{
    private MainWindow Parent;
    private readonly Brush std;
    public Transfer(MainWindow parent)
    {
        InitializeComponent();
        Parent = parent;
        foreach (DataRow row in Parent._controller.Users.Rows)
        {
            Cb_Sender.Items.Add(row[1]);
            Cb_Recipient.Items.Add(row[1]);
        }
        std = Tx_Amount.Background;
    }

    private void Cancel(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Transfer_Click(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(Tx_Amount.Text, out _) || Cb_Sender.SelectedIndex == -1 || Cb_Recipient.SelectedIndex == -1 || !Check_Date())
        {
            Signal_Update(false);
            return;
        }

        bool half = false;
        try
        {
            Parent._controller.Add_Transaction(
                int.Parse((string)Parent._controller.Users.Rows[Cb_Sender.SelectedIndex][0]),
                int.Parse((string)Parent._controller.Categories.Rows[0][0]), $"Перевод пользователю {Parent._controller.Users.Rows[Cb_Recipient.SelectedIndex][1]}",
                -(int)double.Parse(Tx_Amount.Text),
                Parent._controller.Time(Dp_Transaction_Date.SelectedDate.Value));
            half = true;
            Parent.Refresh_Data();
            Parent._controller.Add_Transaction(
                int.Parse((string)Parent._controller.Users.Rows[Cb_Recipient.SelectedIndex][0]),
                int.Parse((string)Parent._controller.Categories.Rows[0][0]), $"Перевод от пользователя {Parent._controller.Users.Rows[Cb_Sender.SelectedIndex][1]}",
                (int)double.Parse(Tx_Amount.Text),
                Parent._controller.Time(Dp_Transaction_Date.SelectedDate.Value));
        }
        catch (Exception exception)
        {
            if (half)
            {
                Parent.Refresh_Data();
                List<int> transactions = new();
                transactions.Add(int.Parse((string)((DataRowView)Parent.Data_Grid_Transactions.Items[^1]).Row[0]));
                Parent._controller.Remove_Transactions(transactions);
            }
            var msg = new MessageBox();
            msg.Content = exception.Message;
            msg.ShowFooter = false;
            msg.Title = "Error";
            msg.ShowDialog();
        }

        
        Parent.Refresh_Data();
        Signal_Update(true);
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

        Dp_Transaction_Date.Background = std;
        Dp_Transaction_Date.ToolTip = null;
        return true;
    }
    private async void Signal_Update(bool success)
    {
        if (success)
        {
            Tb_Status.Text = "Создано";
        }
        else
        {
            Tb_Status.Text = "Не удалось";
        }
        await Task.Delay(1000);
        Tb_Status.Text = "Ожидание";
    }
}