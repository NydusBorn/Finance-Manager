using System.Threading.Tasks;
using System.Windows;
using Wpf.Ui.Controls;

namespace Finance_Manager;

public partial class Add_Transaction : UiWindow
{
    private MainWindow Parent;
    public Add_Transaction(MainWindow parent)
    {
        InitializeComponent();
        Parent = parent;
    }
    
    public void Create(object sender, RoutedEventArgs e)
    {
        //todo: implement this
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