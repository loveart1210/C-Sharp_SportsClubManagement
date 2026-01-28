using System.Windows.Controls;
using SportsClubManagement.ViewModels;

namespace SportsClubManagement.Views
{
    public partial class TeamSettingsView : UserControl
    {
        public TeamSettingsView()
        {
            InitializeComponent();
        }

        private void CopyCode_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is TeamSettingsViewModel vm && !string.IsNullOrEmpty(vm.JoinCode))
            {
                System.Windows.Clipboard.SetText(vm.JoinCode);
                System.Windows.MessageBox.Show("Đã copy mã mời vào clipboard!", "Thông báo");
            }
        }
    }
}
