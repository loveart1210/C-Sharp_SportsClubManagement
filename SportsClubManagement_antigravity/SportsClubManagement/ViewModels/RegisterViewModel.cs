using System;
using System.Windows;
using System.Windows.Input;
using SportsClubManagement.Helpers;
using SportsClubManagement.Models;
using SportsClubManagement.Services;
using SportsClubManagement.Views;

namespace SportsClubManagement.ViewModels
{
    public class RegisterViewModel : ViewModelBase
    {
        private string _fullName = string.Empty;
        private DateTime _birthDate = DateTime.Now.AddYears(-18); // Default to 18 years ago
        private string _email = string.Empty;
        private string _username = string.Empty;
        private string _password = string.Empty; // In a real app, use SecureString
        private string _errorMessage = string.Empty;

        public string FullName
        {
            get => _fullName;
            set => SetProperty(ref _fullName, value);
        }

        public DateTime BirthDate
        {
            get => _birthDate;
            set => SetProperty(ref _birthDate, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

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

        public ICommand RegisterCommand { get; }
        public ICommand NavigateToLoginCommand { get; }

        public RegisterViewModel()
        {
            RegisterCommand = new RelayCommand(ExecuteRegister);
            NavigateToLoginCommand = new RelayCommand(ExecuteNavigateToLogin);
        }

        private void ExecuteRegister(object? parameter)
        {
            // Update password from parameter (PasswordBox binding workarounds usually involve this)
            // But for simplicity in MVVM without extra behaviors, we might bind to a normal TextBox or use code-behind to update ViewModel
            // For this demo, let's assume the View passes the password via CommandParameter or we use a simple binding (less secure but works for demo)
            if (parameter is System.Windows.Controls.PasswordBox passwordBox)
            {
                Password = passwordBox.Password;
            }

            if (!ValidateInput())
                return;

            try
            {
                var newUser = new User
                {
                    FullName = FullName,
                    BirthDate = BirthDate,
                    Email = Email,
                    Username = Username,
                    Password = Password,
                    Role = "User", // Default role
                    CreatedDate = DateTime.Now
                };

                // Check for duplicate username
                if (DataService.Instance.Users.Exists(u => u.Username.Equals(Username, StringComparison.OrdinalIgnoreCase)))
                {
                    ErrorMessage = "Tên đăng nhập đã tồn tại. Vui lòng chọn tên khác.";
                    return;
                }

                DataService.Instance.Users.Add(newUser);
                DataService.Instance.Save();
                DataService.Instance.CurrentUser = newUser;

                // Proceed to MainWindow
                var mainWindow = new MainWindow();
                mainWindow.Show();

                // Close the specific window associated with this view model (RegisterView)
                // We need to find the window.
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.DataContext == this)
                    {
                        window.Close();
                        break;
                    }
                }
                Application.Current.MainWindow = mainWindow;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Đăng ký thất bại: {ex.Message}";
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(FullName) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Vui lòng điền đầy đủ tất cả các trường.";
                return false;
            }

            if (!Email.Contains("@") || !Email.Contains("."))
            {
                ErrorMessage = "Email không hợp lệ.";
                return false;
            }

            return true;
        }

        private void ExecuteNavigateToLogin(object? parameter)
        {
            var loginView = new LoginView();
            loginView.Show();

            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                    break;
                }
            }
            Application.Current.MainWindow = loginView;
        }
    }
}
