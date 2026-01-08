using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using SportsClubManagement.Helpers;
using SportsClubManagement.Models;
using SportsClubManagement.Services;

namespace SportsClubManagement.ViewModels
{
    public class TeamDetailViewModel : ViewModelBase
    {
        private Team _team;
        private string _selectedTabIndex;
        private ObservableCollection<TeamMember> _members;
        private ObservableCollection<Subject> _subjects;
        private ObservableCollection<Session> _sessions;
        private ObservableCollection<Notification> _notifications;
        
        public Team Team
        {
            get => _team;
            set => SetProperty(ref _team, value);
        }

        public string SelectedTabIndex
        {
            get => _selectedTabIndex;
            set => SetProperty(ref _selectedTabIndex, value);
        }

        public ObservableCollection<TeamMember> Members
        {
            get => _members;
            set => SetProperty(ref _members, value);
        }

        public ObservableCollection<Subject> Subjects
        {
            get => _subjects;
            set => SetProperty(ref _subjects, value);
        }

        public ObservableCollection<Session> Sessions
        {
            get => _sessions;
            set => SetProperty(ref _sessions, value);
        }

        public ObservableCollection<Notification> Notifications
        {
            get => _notifications;
            set => SetProperty(ref _notifications, value);
        }

        public ICommand BackCommand { get; }

        public TeamDetailViewModel(Team team = null)
        {
            Team = team;
            SelectedTabIndex = "0";
            BackCommand = new RelayCommand(ExecuteBack);
            LoadTeamData();
        }

        private void LoadTeamData()
        {
            if (Team == null) return;

            // Load members
            var memberList = DataService.Instance.TeamMembers
                .Where(tm => tm.TeamId == Team.Id)
                .ToList();
            Members = new ObservableCollection<TeamMember>(memberList);

            // Load subjects
            var subjectList = DataService.Instance.Subjects
                .Where(s => s.TeamId == Team.Id)
                .ToList();
            Subjects = new ObservableCollection<Subject>(subjectList);

            // Load sessions
            var sessionList = DataService.Instance.Sessions
                .Where(s => s.TeamId == Team.Id)
                .ToList();
            Sessions = new ObservableCollection<Session>(sessionList);

            // Load notifications
            var notificationList = DataService.Instance.Notifications
                .Where(n => n.TeamId == Team.Id)
                .ToList();
            Notifications = new ObservableCollection<Notification>(notificationList);
        }

        private void ExecuteBack(object parameter)
        {
            OnBack?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler OnBack;
    }
}
