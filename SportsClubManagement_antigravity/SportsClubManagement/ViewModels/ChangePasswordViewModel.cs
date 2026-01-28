using System;
using System.Windows.Input;
using SportsClubManagement.Helpers;
using SportsClubManagement.Models;
using SportsClubManagement.Services;

namespace SportsClubManagement.ViewModels
{
    public class ChangePasswordViewModel : ViewModelBase
    {
        private string _newPassword = string.Empty;
        private string _confirmPassword = string.Empty;
        private string _errorMessage = string.Empty;
        private readonly User _user;

        public string NewPassword
        {
            get => _newPassword;
            set => SetProperty(ref _newPassword, value);
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand SaveCommand { get; }

        public event EventHandler? OnSuccess;

        public ChangePasswordViewModel(User user)
        {
            _user = user;
            SaveCommand = new RelayCommand(_ => Save(), _ => CanSave());
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(NewPassword) && 
                   NewPassword.Length >= 6 && 
                   NewPassword == ConfirmPassword;
        }

        private void Save()
        {
            if (NewPassword != ConfirmPassword)
            {
                ErrorMessage = "Mật khẩu xác nhận không khớp.";
                return;
            }

            if (NewPassword.Length < 6)
            {
                ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.";
                return;
            }

            try
            {
                _user.Password = PasswordHasher.Hash(NewPassword);
                _user.MustChangePassword = false;
                DataService.Instance.Save();
                OnSuccess?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi khi lưu mật khẩu: {ex.Message}";
            }
        }
    }
}
