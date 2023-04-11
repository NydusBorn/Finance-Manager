using System;
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
    private bool Categorised;
    private readonly MainWindow Parent;
    private readonly Brush std;

    public Add_Transaction(MainWindow parent)
    {
        InitializeComponent();
        Parent = parent;
        foreach (DataRow row in Parent._controller.Users.Rows) Cb_Transaction_User.Items.Add(row[1]);
        for (var index = 1; index < Parent._controller.Categories.Rows.Count; index++)
        {
            var row = Parent._controller.Categories.Rows[index];
            CB_Category.Items.Add(row[1]);
        }

        CB_Category.SelectedIndex = 0;
        std = Tx_Transaction_Description.Background;
    }

    public void Create(object sender, RoutedEventArgs e)
    {
        var success = true;
        if (Cb_Transaction_User.SelectedIndex == -1)
        {
            success = false;
            Cb_Transaction_User.Background = Brushes.Red;
            Cb_Transaction_User.ToolTip = "Нужно выбрать пользователя.";
        }
        else
        {
            Cb_Transaction_User.Background = std;
            Cb_Transaction_User.ToolTip = null;
        }

        if (!Check_Date()) success = false;
        if (!Check_Price()) success = false;

        if (!success) return;
        string Tr_description;
        Tr_description = Categorised ? CB_Category.SelectedItem.ToString() : Tx_Transaction_Description.Text;
        try
        {
            Parent._controller.Add_Transaction(
            int.Parse((string)Parent._controller.Users.Rows[Cb_Transaction_User.SelectedIndex][0]),
            int.Parse((string)Parent._controller.Categories.Rows[CB_Category.SelectedIndex + 1][0]), Tr_description,
            (int)double.Parse(Tx_Transaction_Price.Text),
            Parent._controller.Time(Dp_Transaction_Date.SelectedDate.Value));
        }
        catch (Exception exception)
        {
            var msg = new MessageBox();
            msg.Content = exception.Message;
            msg.ShowFooter = false;
            msg.Title = "Error";
            msg.ShowDialog();
        }

        
        Parent.Refresh_Data();
        Signal_Update();
    }


    private void Cancel(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private async void Signal_Update()
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

        Dp_Transaction_Date.Background = std;
        Dp_Transaction_Date.ToolTip = null;
        return true;
    }

    private void Tx_Transaction_Price_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        Check_Price();
    }

    private bool Check_Price()
    {
        if (!double.TryParse(Tx_Transaction_Price.Text, out _))
        {
            Tx_Transaction_Price.Background = Brushes.Red;
            Tx_Transaction_Price.ToolTip = "Должно быть числом.";
            return false;
        }

        Tx_Transaction_Price.Background = std;
        Tx_Transaction_Price.ToolTip = null;
        return true;
    }

    private void CB_Category_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CB_Category.SelectedIndex == 0)
        {
            ColDef_Description.Width = new GridLength(2, GridUnitType.Star);
            Categorised = false;
        }
        else
        {
            ColDef_Description.Width = new GridLength(0);
            Categorised = true;
        }
    }
}