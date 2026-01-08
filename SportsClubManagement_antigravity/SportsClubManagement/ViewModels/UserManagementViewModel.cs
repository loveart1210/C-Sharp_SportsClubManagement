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
        private ObservableCollection<User> _users;
        private string _searchText;
        private string _filterRole;
        private ObservableCollection<User> _allUsers;

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

        public ICommand ResetPasswordCommand { get; }
        public ICommand DeleteUserCommand { get; }

        public UserManagementViewModel()
        {
            _filterRole = "All";
            LoadUsers();
            ResetPasswordCommand = new RelayCommand(ResetPassword);
            DeleteUserCommand = new RelayCommand(DeleteUser);
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
                                             u.Email.ToLower().Contains(SearchText.ToLower()));
            
            if (FilterRole != "All")
                filtered = filtered.Where(u => u.Role == FilterRole);
            
            Users = new ObservableCollection<User>(filtered.ToList());
        }

        private void ResetPassword(object obj)
        {
            if (obj is User user)
            {
                var result = MessageBox.Show($"Bạn có chắc muốn reset mật khẩu cho {user.FullName}?", "Xác nhận", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    user.Password = "123"; // Default password
                    DataService.Instance.Save();
                    MessageBox.Show("Mật khẩu đã được reset thành công!");
                }
            }
        }

        private void DeleteUser(object obj)
        {
            if (obj is User deleteUser && deleteUser.Id != DataService.Instance.CurrentUser?.Id)
            {
                var result = MessageBox.Show($"Bạn có chắc muốn xóa người dùng {deleteUser.FullName}?", "Xác nhận", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    DataService.Instance.Users.Remove(deleteUser);
                    DataService.Instance.Save();
                    LoadUsers();
                    MessageBox.Show("Người dùng đã được xóa!");
                }
            }
            else if (obj is User currentUser && currentUser.Id == DataService.Instance.CurrentUser?.Id)
            {
                MessageBox.Show("Không thể xóa tài khoản của chính bạn!");
            }
        }
    }
}
