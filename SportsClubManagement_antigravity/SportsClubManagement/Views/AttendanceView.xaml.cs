using System.Windows.Controls;
using SportsClubManagement.ViewModels;

namespace SportsClubManagement.Views
{
    public partial class AttendanceView : UserControl
    {
        public AttendanceView()
        {
            InitializeComponent();
            // DataContext = new AttendanceViewModel(); // Moved to Loaded event or handled by XAML
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is ViewModels.AttendanceViewModel vm)
            {
                // vm.RefreshData(); // Try auto-refresh on load
                System.Diagnostics.Debug.WriteLine($"AttendanceView Loaded. VM Sessions: {vm.Sessions?.Count}");
            }
            else
            {
                // If DataContext is not set in XAML, set it here
                if (DataContext == null)
                {
                    DataContext = new AttendanceViewModel();
                    vm = DataContext as ViewModels.AttendanceViewModel;
                    if (vm != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"AttendanceView Loaded. DataContext initialized. VM Sessions: {vm.Sessions?.Count}");
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("AttendanceView DataContext is NOT AttendanceViewModel!");
                }
            }
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is ViewModels.AttendanceViewModel vm)
            {
                vm.RefreshData();
                System.Windows.MessageBox.Show($"Manual Refresh. Sessions: {vm.Sessions?.Count}");
            }
        }
    }
}
