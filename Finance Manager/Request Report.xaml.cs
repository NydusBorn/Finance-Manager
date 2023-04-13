using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Wpf.Ui.Controls;

namespace Finance_Manager;

public partial class Request_Report : UiWindow {
    
    [Flags]
    public enum ReportType {
        None = 0b_0000_0000,
        User = 0b_0000_0001,
        Date = 0b_0000_0010,
        Category = 0b_0000_0100
    }

    private readonly MainWindow Parent;
    private readonly Brush std;

    private readonly ReportType Type;

    public Request_Report(MainWindow parent, ReportType type) {
        InitializeComponent();
        Type = type;
        std = Cb_User.Background;
        Parent = parent;
        foreach (DataRow row in Parent._controller.Users.Rows) Cb_User.Items.Add(row[1]);
        foreach (DataRow row in Parent._controller.Categories.Rows) Cb_Category.Items.Add(row[1]);
        if ((type & ReportType.User) != 0)
        {
            RDef_User.Height = new(50);
            this.MinHeight += 50;
            this.MaxHeight += 50;
        }
        if ((type & ReportType.Date) != 0)
        {
            RDef_Date.Height = new(70);
            this.MinHeight += 70;
            this.MaxHeight += 70;
        }
        if ((type & ReportType.Category) != 0)
        {
            RDef_Category.Height = new(50);
            this.MinHeight += 50;
            this.MaxHeight += 50;
        }
    }

    private void Create(object sender, RoutedEventArgs e) {
        var success = true;
        if ((Type & ReportType.User) != 0) {
            if (Cb_User.SelectedIndex == -1) {
                success = false;
                Cb_User.Background = Brushes.Red;
                Cb_User.ToolTip = "Нужно выбрать пользователя.";
            } else {
                Cb_User.Background = std;
                Cb_User.ToolTip = null;
            }
        }

        if ((Type & ReportType.Category) != 0) {
            if (Cb_Category.SelectedIndex == -1) {
                success = false;
                Cb_Category.Background = Brushes.Red;
                Cb_Category.ToolTip = "Нужно выбрать категорию.";
            } else {
                Cb_Category.Background = std;
                Cb_Category.ToolTip = null;
            }
        }
        
        if ((Type & ReportType.Date) != 0) {
            if (!Check_Date(DP_From)) success = false;
            if (!Check_Date(DP_To)) success = false;
        }

        if (!success) return;
        switch (Type) {
            case ReportType.None:
                throw new ArgumentOutOfRangeException();
                break;
            case ReportType.User:
                Parent.Data_Grid_Rep.ItemsSource =
                    Parent._controller.GetUser(
                        int.Parse((string)Parent._controller.Users.Rows[Cb_User.SelectedIndex][0])).DefaultView;
                break;
            case ReportType.Date:
                Parent.Data_Grid_Rep.ItemsSource =
                    Parent._controller.GetTransactionsPerPeriod(DP_From.SelectedDate.Value - new TimeSpan(0,0,1),DP_To.SelectedDate.Value + new TimeSpan(0,0,1)).DefaultView;
                break;
            case ReportType.Category:
                Parent.Data_Grid_Rep.ItemsSource =
                    Parent._controller.GetTransactionPerCategory(
                        int.Parse((string)Parent._controller.Categories.Rows[Cb_Category.SelectedIndex][0])).DefaultView;
                break;
            case ReportType.User | ReportType.Date:
                Parent.Data_Grid_Rep.ItemsSource =
                    Parent._controller.GetTransactionsPerPeriodAndUser(
                        DP_From.SelectedDate.Value - new TimeSpan(0,0,1),DP_To.SelectedDate.Value + new TimeSpan(0,0,1),int.Parse((string)Parent._controller.Users.Rows[Cb_User.SelectedIndex][0])).DefaultView;
                break;
            case ReportType.Category | ReportType.Date:
                Parent.Data_Grid_Rep.ItemsSource =
                    Parent._controller.GetTransactionPerCategoryAndDate(
                        DP_From.SelectedDate.Value - new TimeSpan(0,0,1),DP_To.SelectedDate.Value + new TimeSpan(0,0,1), int.Parse((string)Parent._controller.Categories.Rows[Cb_Category.SelectedIndex][0])).DefaultView;
                break;
            case ReportType.User | ReportType.Category:
                Parent.Data_Grid_Rep.ItemsSource =
                    Parent._controller.GetTransactionPerCategoryAndUser(
                        int.Parse((string)Parent._controller.Users.Rows[Cb_User.SelectedIndex][0]),int.Parse((string)Parent._controller.Categories.Rows[Cb_Category.SelectedIndex][0])).DefaultView;
                break;
            case ReportType.Category | ReportType.Date | ReportType.User:
                Parent.Data_Grid_Rep.ItemsSource =
                    Parent._controller.GetTransactionPerCategoryAndDateAndUser(
                        DP_From.SelectedDate.Value - new TimeSpan(0,0,1),DP_To.SelectedDate.Value + new TimeSpan(0,0,1), int.Parse((string)Parent._controller.Categories.Rows[Cb_Category.SelectedIndex][0]),int.Parse((string)Parent._controller.Users.Rows[Cb_User.SelectedIndex][0])).DefaultView;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private bool Check_Date(DatePicker check) {
        if (check.SelectedDate == null) {
            check.Background = Brushes.Red;
            check.ToolTip = "Требуется задать дату транзакции.";
            return false;
        }

        check.Background = std;
        check.ToolTip = null;
        return true;
    }
}