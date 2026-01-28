using System.Windows;
using System.Windows.Controls;
using SportsClubManagement.ViewModels;
using SportsClubManagement.Models;

namespace SportsClubManagement.Views
{
    public partial class ChangePasswordWindow : Window
    {
        private readonly ChangePasswordViewModel _viewModel;

        public ChangePasswordWindow(User user)
        {
            InitializeComponent();
            _viewModel = new ChangePasswordViewModel(user);
            DataContext = _viewModel;

            _viewModel.OnSuccess += (s, e) => 
            {
                DialogResult = true;
                Close();
            };

            // Bind PasswordBoxes
            NewPasswordBox.PasswordChanged += (s, e) => 
            {
                _viewModel.NewPassword = NewPasswordBox.Password;
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            };
            ConfirmPasswordBox.PasswordChanged += (s, e) => 
            {
                _viewModel.ConfirmPassword = ConfirmPasswordBox.Password;
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            };
        }
    }
}
