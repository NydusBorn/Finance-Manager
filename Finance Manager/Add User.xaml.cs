using System;
using System.Threading.Tasks;
using System.Windows;
using Wpf.Ui.Controls;
using MessageBox = Wpf.Ui.Controls.MessageBox;

namespace Finance_Manager;

public partial class Add_User : UiWindow
{
    private readonly MainWindow Parent;

    public Add_User(MainWindow parent)
    {
        InitializeComponent();
        Parent = parent;
    }

    public void Create(object sender, RoutedEventArgs e)
    {
        try
        {
            Parent._controller.Add_User(Tx_New_User.Text);
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
}