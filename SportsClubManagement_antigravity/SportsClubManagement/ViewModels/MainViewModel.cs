using System;
using System.Windows;
using System.Windows.Input;
using SportsClubManagement.Helpers;
using SportsClubManagement.Services;
using SportsClubManagement.Views;

namespace SportsClubManagement.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private object _currentView = new object();
        private string _userName = string.Empty;
        private string _userRole = string.Empty;
        private string _userAvatar = string.Empty;

        public object CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        public string UserRole
        {
            get => _userRole;
            set => SetProperty(ref _userRole, value);
        }

        public string UserAvatar
        {
            get => _userAvatar;
            set => SetProperty(ref _userAvatar, value);
        }

        public ICommand NavigateToDashboardCommand { get; }
        public ICommand NavigateToProfileCommand { get; }
        public ICommand NavigateToTeamsCommand { get; }
        public ICommand NavigateToPersonalScheduleCommand { get; }
        public ICommand NavigateToAdminCommand { get; }
        public ICommand LogoutCommand { get; }

        public MainViewModel()
        {
            var currentUser = DataService.Instance.CurrentUser;
            if (currentUser != null)
            {
                UserName = currentUser.FullName;
                UserRole = currentUser.Role;
                UserAvatar = currentUser.AvatarPath ?? "https://via.placeholder.com/40";
            }

            NavigateToDashboardCommand = new RelayCommand(o => CurrentView = new DashboardViewModel());
            NavigateToTeamsCommand = new RelayCommand(o => ShowTeamsView());
            NavigateToPersonalScheduleCommand = new RelayCommand(o => {
                var vm = new PersonalScheduleViewModel();
                if (o != null && int.TryParse(o.ToString(), out int index))
                {
                    vm.SelectedTabIndex = index;
                }
                CurrentView = vm;
            });
            NavigateToProfileCommand = new RelayCommand(o => {
                var profileWindow = new ProfileWindow(DataService.Instance.CurrentUser?.Id ?? "");
                profileWindow.Owner = Application.Current.MainWindow;
                profileWindow.ShowDialog();
            });
            NavigateToAdminCommand = new RelayCommand(o => CurrentView = new AdminPanelViewModel());
            LogoutCommand = new RelayCommand(ExecuteLogout);

            // Default View
            CurrentView = new DashboardViewModel();
        }

        private void ExecuteLogout(object? obj)
        {
            DataService.Instance.CurrentUser = null;
            
            // Return to login screen
            LoginView loginView = new LoginView();
            loginView.Show();
            
            // Close current window safely
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                    break;
                }
            }

            // Set the new LoginView as the main window
            Application.Current.MainWindow = loginView;
        }

        private void ShowTeamsView()
        {
            var teamsVM = new TeamsViewModel();
            teamsVM.OnTeamSelected += (s, team) =>
            {
                var detailVM = new TeamDetailViewModel(team);
                detailVM.OnBack += (s2, e2) => ShowTeamsView();
                detailVM.OnRequestProfile += (s2, userId) => {
                    var profileWindow = new ProfileWindow(userId);
                    profileWindow.Owner = Application.Current.MainWindow;
                    profileWindow.ShowDialog();
                };
                CurrentView = detailVM;
            };
            CurrentView = teamsVM;
        }
    }
}
