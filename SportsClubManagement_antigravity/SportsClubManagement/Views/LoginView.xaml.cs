using System.Windows;
using System.Windows.Controls;
using SportsClubManagement.ViewModels;

namespace SportsClubManagement.Views
{
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
            var vm = this.DataContext as LoginViewModel;
            if (vm != null)
            {
                vm.OnLoginSuccess += Vm_OnLoginSuccess;
            }
        }

        private void Vm_OnLoginSuccess(object sender, System.EventArgs e)
        {
            // Pass password to ViewModel before opening MainWindow
            var vm = this.DataContext as LoginViewModel;
            if (vm != null && txtPassword != null)
            {
                vm.Password = txtPassword.Password;
            }

            // Open MainWindow
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            // Handle password box value passing
            var vm = this.DataContext as LoginViewModel;
            if (vm != null && txtPassword != null)
            {
                vm.Password = txtPassword.Password;
                vm.LoginCommand.Execute(null);
            }
        }
    }
}
