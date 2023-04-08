using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Wpf.Ui.Controls;
using Button = Wpf.Ui.Controls.Button;
using MessageBox = Wpf.Ui.Controls.MessageBox;

namespace Finance_Manager;

public partial class Categories : UiWindow
{
    private readonly MainWindow Parent;

    public Categories(MainWindow parent)
    {
        InitializeComponent();
        Parent = parent;
        Refresh();
    }

    private void Create(object sender, RoutedEventArgs e)
    {
        try
        {
            Parent._controller.Add_Category(Tx_New_Category.Text);
        }
        catch (Exception exception)
        {
            var msg = new MessageBox();
            msg.Content = exception.Message;
            msg.Title = "Error";
            msg.ShowFooter = false;
            msg.ShowDialog();
        }
        Parent.Refresh_Data();
        Refresh();
    }

    private void Refresh()
    {
        StP_Categories.Children.Clear();
        var dt = Parent._controller.Categories;
        for (var index = 2; index < dt.Rows.Count; index++)
        {
            var row = dt.Rows[index];
            var categoryName = row["Category Name"].ToString();
            CreateCategoryControls(categoryName);
        }

        StP_Categories.UpdateLayout();
    }

    private void CreateCategoryControls(string categoryName)
    {
        var grid = new Grid();
        grid.Margin = new Thickness(5);
        var col1 = new ColumnDefinition();
        col1.Width = new GridLength(1, GridUnitType.Star);
        var col2 = new ColumnDefinition();
        col2.Width = new GridLength(50);
        grid.ColumnDefinitions.Add(col1);
        grid.ColumnDefinitions.Add(col2);
        var tb = new TextBlock();
        tb.Text = categoryName;
        tb.Margin = new Thickness(5);
        var btn = new Button();
        var img = new Image();
        img.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/Remove.png"));
        btn.Content = img;
        btn.HorizontalAlignment = HorizontalAlignment.Stretch;
        btn.VerticalAlignment = VerticalAlignment.Stretch;
        btn.Click += Btn_Remove_Category;
        Grid.SetColumn(tb, 0);
        Grid.SetColumn(btn, 1);
        grid.Children.Add(tb);
        grid.Children.Add(btn);
        StP_Categories.Children.Add(grid);
    }

    private void Btn_Remove_Category(object sender, RoutedEventArgs e)
    {
        var btn = sender as Button;
        var sp = VisualTreeHelper.GetParent(btn) as Grid;
        var tb = sp.Children[0] as TextBlock;
        var categoryName = tb.Text;
        Parent._controller.Remove_Category(categoryName);
        Parent.Refresh_Data();
        Refresh();
    }
}