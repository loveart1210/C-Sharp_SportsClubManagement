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
    public class TeamSettingsViewModel : ViewModelBase
    {
        private Team _team;
        private ObservableCollection<MemberSettingItem> _members = new ObservableCollection<MemberSettingItem>();
        private bool _canManage;

        public ObservableCollection<MemberSettingItem> Members
        {
            get => _members;
            set => SetProperty(ref _members, value);
        }

        public bool CanManage
        {
            get => _canManage;
            set => SetProperty(ref _canManage, value);
        }

        public string JoinCode
        {
            get => _team.JoinCode;
            private set
            {
                _team.JoinCode = value;
                OnPropertyChanged();
            }
        }

        public ICommand ChangeRoleCommand { get; }
        public ICommand KickMemberCommand { get; }
        public ICommand RegenerateCodeCommand { get; }

        public TeamSettingsViewModel(Team team)
        {
            _team = team;
            ChangeRoleCommand = new RelayCommand(ExecuteChangeRole);
            KickMemberCommand = new RelayCommand(ExecuteKickMember);
            RegenerateCodeCommand = new RelayCommand(ExecuteRegenerateCode);
            LoadMembers();
            CheckPermissions();
        }

        private void CheckPermissions()
        {
            var currentUser = DataService.Instance.CurrentUser;
            if (currentUser == null)
            {
                CanManage = false;
                return;
            }

            var userMember = DataService.Instance.TeamMembers.FirstOrDefault(tm => tm.TeamId == _team.Id && tm.UserId == currentUser.Id);
            CanManage = userMember != null && (userMember.Role == "Founder" || userMember.Role == "Admin");
        }

        public void RefreshData()
        {
            LoadMembers();
            CheckPermissions();
        }

        private void LoadMembers()
        {
            var items = new ObservableCollection<MemberSettingItem>();
            var teamMembers = DataService.Instance.TeamMembers.Where(tm => tm.TeamId == _team.Id).ToList();
            
            foreach (var tm in teamMembers)
            {
                var user = DataService.Instance.Users.FirstOrDefault(u => u.Id == tm.UserId);
                if (user != null)
                {
                    items.Add(new MemberSettingItem
                    {
                        UserId = user.Id,
                        FullName = user.FullName,
                        Email = user.Email,
                        Role = tm.Role,
                        JoinDate = tm.JoinDate,
                        IsCurrentUser = user.Id == DataService.Instance.CurrentUser?.Id,
                        IsFounder = tm.Role == "Founder"
                    });
                }
            }
            Members = items;
        }

        private void ExecuteChangeRole(object? parameter)
        {
            if (parameter is MemberSettingItem item)
            {
                // In a real app, this would show a dialog. 
                // For this demo, let's cycle roles: Member -> Coach -> Admin -> Member
                // (Cannot change Founder's role)
                if (item.Role == "Founder")
                {
                    MessageBox.Show("Không thể thay đổi quyền của Founder.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string newRole = item.Role == "Member" ? "Coach" : (item.Role == "Coach" ? "Admin" : "Member");
                
                var teamMember = DataService.Instance.TeamMembers.FirstOrDefault(tm => tm.TeamId == _team.Id && tm.UserId == item.UserId);
                if (teamMember != null)
                {
                    teamMember.Role = newRole;
                    DataService.Instance.Save();
                    LoadMembers();
                    MessageBox.Show($"Đã thay đổi vai trò của {item.FullName} thành {newRole}.", "Thành công");
                }
            }
        }

        private void ExecuteKickMember(object? parameter)
        {
            if (parameter is MemberSettingItem item)
            {
                if (item.Role == "Founder")
                {
                    MessageBox.Show("Không thể kick Founder khỏi nhóm.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (item.IsCurrentUser)
                {
                    MessageBox.Show("Bạn không thể tự kick chính mình ở đây. Hãy sử dụng chức năng 'Rời nhóm' (nếu có).", "Thông báo");
                    return;
                }

                var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa {item.FullName} khỏi nhóm?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var teamMember = DataService.Instance.TeamMembers.FirstOrDefault(tm => tm.TeamId == _team.Id && tm.UserId == item.UserId);
                    if (teamMember != null)
                    {
                        DataService.Instance.TeamMembers.Remove(teamMember);
                        DataService.Instance.Save();
                        LoadMembers();
                        MessageBox.Show($"Đã xóa {item.FullName} khỏi nhóm.", "Thành công");
                    }
                }
            }
        }

        private void ExecuteRegenerateCode(object? parameter)
        {
            var result = MessageBox.Show("Mã mời cũ sẽ hết hiệu lực. Bạn có chắc chắn muốn tạo mã mới?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                JoinCode = DataService.Instance.GenerateJoinCode();
                DataService.Instance.Save();
                MessageBox.Show($"Mã mời mới của bạn là: {JoinCode}", "Thành công");
            }
        }
    }

    public class MemberSettingItem
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime JoinDate { get; set; }
        public bool IsCurrentUser { get; set; }
        public bool IsFounder { get; set; }
        
        public bool CanBeKicked => !IsFounder && !IsCurrentUser;
        public bool CanChangeRole => !IsFounder;
    }
}
