using System;
using System.Data;
using System.Drawing;
using System.IO.Packaging;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Wpf.Ui.Controls;
using Button = Wpf.Ui.Controls.Button;
using Image = System.Windows.Controls.Image;

namespace Finance_Manager;

public partial class Categories : UiWindow
{
    private MainWindow Parent;
    public Categories(MainWindow parent)
    {
        InitializeComponent();
        Parent = parent;
        Refresh();
    }

    private void Create(object sender, RoutedEventArgs e)
    {
        Parent._controller.Add_Category(Tx_New_Category.Text);
        Parent.Refresh_Data();
        Refresh();
    }

    private void Refresh()
    {
        StP_Categories.Children.Clear();
        DataTable dt = Parent._controller.Categories;
        foreach (DataRow row in dt.Rows)
        {
            string categoryName = row["Category Name"].ToString();
            CreateCategoryControls(categoryName);
        }
        
        StP_Categories.UpdateLayout();
    }
    private void CreateCategoryControls(string categoryName)
    {
        Grid grid = new Grid();
        grid.Margin = new Thickness(5);
        ColumnDefinition col1 = new ColumnDefinition();
        col1.Width = new GridLength(1, GridUnitType.Star);
        ColumnDefinition col2 = new ColumnDefinition();
        col2.Width = new GridLength(50);
        grid.ColumnDefinitions.Add(col1);
        grid.ColumnDefinitions.Add(col2);
        TextBlock tb = new TextBlock();
        tb.Text = categoryName;
        tb.Margin = new Thickness(5);
        Button btn = new Button();
        Image img = new Image();
        img.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/Remove.png"));
        btn.Content = img;
        btn.HorizontalAlignment = HorizontalAlignment.Stretch;
        btn.VerticalAlignment = VerticalAlignment.Stretch;
        btn.Click += new RoutedEventHandler(btn_Click);
        Grid.SetColumn(tb, 0);
        Grid.SetColumn(btn, 1);
        grid.Children.Add(tb);
        grid.Children.Add(btn);
        StP_Categories.Children.Add(grid);
    }
    private void btn_Click(object sender, RoutedEventArgs e)
    {
        Button btn = sender as Button;
        StackPanel sp = VisualTreeHelper.GetParent(btn) as StackPanel;
        TextBlock tb = sp.Children[0] as TextBlock;
        string categoryName = tb.Text;
        Parent._controller.Remove_Category(categoryName);
        Parent.Refresh_Data();
        Refresh();
    }
}