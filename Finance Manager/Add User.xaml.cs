using System.Threading.Tasks;
using System.Windows;
using Wpf.Ui.Controls;

namespace Finance_Manager;

public partial class Add_User : UiWindow
{
    private MainWindow Parent;
    public Add_User(MainWindow parent)
    {
        InitializeComponent();
        Parent = parent;
    }

    public void Create(object sender, RoutedEventArgs e)
    {
        Parent._controller.Add_User(Tx_New_User.Text);
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
}