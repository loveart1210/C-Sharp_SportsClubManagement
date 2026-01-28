using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using SportsClubManagement.Helpers;
using SportsClubManagement.Models;
using SportsClubManagement.Services;

namespace SportsClubManagement.ViewModels
{
    public class UserManagementViewModel : ViewModelBase
    {
        private ObservableCollection<User> _users = new ObservableCollection<User>();
        private string _searchText = string.Empty;
        private string _filterRole = "All";
        private ObservableCollection<User> _allUsers = new ObservableCollection<User>();
        private string _tempPassword = string.Empty;
        private bool _showTempPassword = false;

        public ObservableCollection<User> Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                ApplyFilters();
            }
        }

        public string FilterRole
        {
            get => _filterRole;
            set
            {
                SetProperty(ref _filterRole, value);
                ApplyFilters();
            }
        }

        public string TempPassword
        {
            get => _tempPassword;
            set => SetProperty(ref _tempPassword, value);
        }

        public bool ShowTempPassword
        {
            get => _showTempPassword;
            set => SetProperty(ref _showTempPassword, value);
        }

        public ICommand ResetPasswordCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand CloseTempPasswordCommand { get; }

        public UserManagementViewModel()
        {
            _filterRole = "All";
            LoadUsers();
            ResetPasswordCommand = new RelayCommand(ResetPassword);
            DeleteUserCommand = new RelayCommand(DeleteUser);
            CloseTempPasswordCommand = new RelayCommand(_ => ShowTempPassword = false);
        }

        private void LoadUsers()
        {
            _allUsers = new ObservableCollection<User>(DataService.Instance.Users.ToList());
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var filtered = _allUsers.AsEnumerable();
            
            if (!string.IsNullOrWhiteSpace(SearchText))
                filtered = filtered.Where(u => u.FullName.ToLower().Contains(SearchText.ToLower()) || 
                                             u.Username.ToLower().Contains(SearchText.ToLower()) ||
                                             u.Email.ToLower().Contains(SearchText.ToLower()));
            
            if (FilterRole != "All")
                filtered = filtered.Where(u => u.Role == FilterRole);
            
            Users = new ObservableCollection<User>(filtered.ToList());
        }

        private void ResetPassword(object? obj)
        {
            if (obj is User user)
            {
                var result = MessageBox.Show($"Bạn có chắc muốn reset mật khẩu cho {user.FullName} ({user.Username})?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    // Generate a 6-digit random password
                    string plainPassword = new Random().Next(100000, 999999).ToString();
                    
                    // Hash and save
                    user.Password = PasswordHasher.Hash(plainPassword);
                    user.MustChangePassword = true;
                    DataService.Instance.Save();
                    
                    // Display to admin
                    TempPassword = plainPassword;
                    ShowTempPassword = true;
                }
            }
        }

        private void DeleteUser(object? obj)
        {
            if (obj is User deleteUser && deleteUser.Id != DataService.Instance.CurrentUser?.Id)
            {
                var result = MessageBox.Show($"Bạn có chắc muốn xóa người dùng {deleteUser.FullName}?", "Xác nhận Xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    DataService.Instance.Users.Remove(deleteUser);
                    DataService.Instance.Save();
                    LoadUsers();
                }
            }
            else if (obj is User currentUser && currentUser.Id == DataService.Instance.CurrentUser?.Id)
            {
                MessageBox.Show("Không thể tự xóa tài khoản của chính mình!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
