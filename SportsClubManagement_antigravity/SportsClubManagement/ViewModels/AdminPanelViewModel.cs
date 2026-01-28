using System;
using SportsClubManagement.Helpers;

namespace SportsClubManagement.ViewModels
{
    public class AdminPanelViewModel : ViewModelBase
    {
        private UserManagementViewModel _userManagementVM;
        private AdminTeamManagementViewModel _teamManagementVM;
        private int _selectedTabIndex = 0;

        public UserManagementViewModel UserManagementVM
        {
            get => _userManagementVM;
            set => SetProperty(ref _userManagementVM, value);
        }

        public AdminTeamManagementViewModel TeamManagementVM
        {
            get => _teamManagementVM;
            set => SetProperty(ref _teamManagementVM, value);
        }

        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set => SetProperty(ref _selectedTabIndex, value);
        }

        public AdminPanelViewModel()
        {
            _userManagementVM = new UserManagementViewModel();
            _teamManagementVM = new AdminTeamManagementViewModel();
        }
    }
}
