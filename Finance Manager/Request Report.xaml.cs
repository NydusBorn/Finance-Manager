using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Wpf.Ui.Controls;

namespace Finance_Manager;

public partial class Request_Report : UiWindow {
    public enum ReportType {
        User,
        Date,
        UserAndDate,
        Category,
        GroupCategory,
        DateAndCategory,
        UserAndCategory,
        UserAndDateAndCategory
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
    }

    private void Create(object sender, RoutedEventArgs e) {
        var success = true;
        if (Type is ReportType.User or ReportType.UserAndDate) {
            if (Cb_User.SelectedIndex == -1) {
                success = false;
                Cb_User.Background = Brushes.Red;
                Cb_User.ToolTip = "Нужно выбрать пользователя.";
            } else {
                Cb_User.Background = std;
                Cb_User.ToolTip = null;
            }
        }

        if (Type is ReportType.Date or ReportType.UserAndDate) {
            if (!Check_Date(DP_From)) success = false;
            if (!Check_Date(DP_To)) success = false;
        }

        if (!success) return;
        switch (Type) {
            case ReportType.User: {
                    Parent.Data_Grid_Rep.ItemsSource = Parent._controller
                        .GetUser(int.Parse((string)Parent._controller.Users.Rows[Cb_User.SelectedIndex][0])).DefaultView;
                }
                break;
            case ReportType.Date: {
                    Parent.Data_Grid_Rep.ItemsSource = Parent._controller
                        .GetTransactionsPerPeriod(DP_From.SelectedDate.Value - new TimeSpan(0, 0, 1),
                            DP_To.SelectedDate.Value + new TimeSpan(1, 0, 0, 0))
                        .DefaultView;
                }
                break;
            case ReportType.UserAndDate: {
                    Parent.Data_Grid_Rep.ItemsSource = Parent._controller
                        .GetTransactionsPerPeriodAndUser(DP_From.SelectedDate.Value - new TimeSpan(0, 0, 1),
                            DP_To.SelectedDate.Value + new TimeSpan(1, 0, 0, 0),
                            int.Parse((string)Parent._controller.Users.Rows[Cb_User.SelectedIndex][0])).DefaultView;
                    break;
                }
            case ReportType.Category: {
                    Parent.Data_Grid_Rep.ItemsSource = Parent._controller
                        .GetTransactionPerCategory(int.Parse((string)Parent._controller.Categories.Rows[Cb_User.SelectedIndex][0])).DefaultView;
                    break;
                }
            case ReportType.GroupCategory: {
                    Parent.Data_Grid_Rep.ItemsSource = Parent._controller
                        .GetTransactionGroupByCategory().DefaultView;
                    break;
                }
            case ReportType.DateAndCategory: {
                    Parent.Data_Grid_Rep.ItemsSource = Parent._controller
                        .GetTransactionPerCategoryAndDate(DP_From.SelectedDate.Value - new TimeSpan(0, 0, 1),
                            DP_To.SelectedDate.Value + new TimeSpan(1, 0, 0, 0), int.Parse((string)Parent._controller.Categories.Rows[Cb_User.SelectedIndex][0])).DefaultView;
                    break;
                }
            case ReportType.UserAndCategory: {
                    Parent.Data_Grid_Rep.ItemsSource = Parent._controller
                        .GetTransactionPerCategoryAndUser(int.Parse((string)Parent._controller.Categories.Rows[Cb_User.SelectedIndex][0]), int.Parse((string)Parent._controller.Categories.Rows[Cb_User.SelectedIndex][0])).DefaultView;
                    break;
                }
            case ReportType.UserAndDateAndCategory: {
                    Parent.Data_Grid_Rep.ItemsSource = Parent._controller
                        .GetTransactionPerCategoryAndDateAndUser(DP_From.SelectedDate.Value - new TimeSpan(0, 0, 1),
                            DP_To.SelectedDate.Value + new TimeSpan(1, 0, 0, 0), int.Parse((string)Parent._controller.Categories.Rows[Cb_User.SelectedIndex][0]), int.Parse((string)Parent._controller.Categories.Rows[Cb_User.SelectedIndex][0])).DefaultView;
                    break;
                }
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