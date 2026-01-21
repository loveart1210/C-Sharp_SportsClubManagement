using System;
using System.Windows;
using System.Windows.Input;
using SportsClubManagement.Helpers;
using SportsClubManagement.Services;
using System.Linq;

namespace SportsClubManagement.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand LoginCommand { get; }

        public ICommand NavigateToRegisterCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);
            NavigateToRegisterCommand = new RelayCommand(ExecuteNavigateToRegister);
        }

        private void ExecuteNavigateToRegister(object? parameter)
        {
            var registerView = new Views.RegisterView();
            registerView.Show();

            // Close current window (LoginView)
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                    break;
                }
            }
            Application.Current.MainWindow = registerView;
        }

        private bool CanExecuteLogin(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
        }

        private void ExecuteLogin(object? parameter)
        {
            ErrorMessage = "";

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Vui lòng nhập tên đăng nhập và mật khẩu";
                return;
            }

            var user = DataService.Instance.Users.FirstOrDefault(u => u.Username == Username && u.Password == Password);

            if (user != null)
            {
                DataService.Instance.CurrentUser = user;
                OnLoginSuccess?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng";
            }
        }

        public event EventHandler? OnLoginSuccess;
    }
}
