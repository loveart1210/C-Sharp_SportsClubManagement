using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using SportsClubManagement.Helpers;
using SportsClubManagement.Models;
using SportsClubManagement.Services;
using SportsClubManagement.Views; // Added for MainViewModel navigation

namespace SportsClubManagement.ViewModels
{
    public class TeamDetailViewModel : ViewModelBase
    {
        private Team? _team;
        private ObservableCollection<TeamMember> _members = new ObservableCollection<TeamMember>();
        private ObservableCollection<Notification> _notifications = new ObservableCollection<Notification>();
        private int _selectedTabIndex;

        // Child ViewModels
        public TeamSubjectsViewModel? SubjectsVM { get; private set; }
        public TeamSessionsViewModel? SessionsVM { get; private set; }
        public AttendanceViewModel? AttendanceVM { get; private set; }

        public Team? Team
        {
            get => _team;
            set
            {
                SetProperty(ref _team, value);
                LoadTeamData();
                InitializeChildViewModels();
            }
        }

        public ObservableCollection<TeamMember> Members
        {
            get => _members;
            set => SetProperty(ref _members, value);
        }

        public ObservableCollection<Notification> Notifications
        {
            get => _notifications;
            set => SetProperty(ref _notifications, value);
        }

        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set
            {
                if (SetProperty(ref _selectedTabIndex, value))
                {
                    RefreshCurrentTab();
                }
            }
        }

        public ICommand BackCommand { get; }

        public event EventHandler? OnBack;

        public void RefreshCurrentTab()
        {
            switch (SelectedTabIndex)
            {
                case 1: 
                    SubjectsVM?.RefreshData(); 
                    break;
                case 2: 
                    SessionsVM?.RefreshData(); 
                    break;
                case 3: 
                    // Sync date from Sessions tab to Attendance tab for better UX
                    if (SessionsVM != null && AttendanceVM != null)
                    {
                        AttendanceVM.SelectedDate = SessionsVM.SelectedDate;
                    }
                    AttendanceVM?.RefreshData(); 
                    break;
            }
        }

        public TeamDetailViewModel(Team? team = null)
        {
            _team = team;
            BackCommand = new RelayCommand(GoBack);
            if (_team != null)
            {
                LoadTeamData();
                InitializeChildViewModels();
            }
        }

        private void InitializeChildViewModels()
        {
            if (_team != null)
            {
                SubjectsVM = new TeamSubjectsViewModel(_team);
                SessionsVM = new TeamSessionsViewModel(_team);
                AttendanceVM = new AttendanceViewModel(_team);
                
                OnPropertyChanged(nameof(SubjectsVM));
                OnPropertyChanged(nameof(SessionsVM));
                OnPropertyChanged(nameof(AttendanceVM));
            }
        }

        private void LoadTeamData()
        {
            if (_team == null) return;

            // Load Members
            var members = DataService.Instance.TeamMembers.Where(tm => tm.TeamId == _team.Id).ToList();
            Members = new ObservableCollection<TeamMember>(members);

            // Load Notifications
            var notifs = DataService.Instance.Notifications
                .Where(n => n.TeamId == _team.Id)
                .OrderByDescending(n => n.CreatedDate)
                .ToList();
            Notifications = new ObservableCollection<Notification>(notifs);
        }

        private void GoBack(object? obj)
        {
            OnBack?.Invoke(this, EventArgs.Empty);
        }
    }
}
