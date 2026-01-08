using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using SportsClubManagement.Helpers;
using SportsClubManagement.Models;
using SportsClubManagement.Services;

namespace SportsClubManagement.ViewModels
{
    public class TeamMembersViewModel : ViewModelBase
    {
        private Team _team;
        private ObservableCollection<MemberDisplay> _members;
        private ObservableCollection<MemberDisplay> _allMembers;
        private string _searchText = string.Empty;
        private string _filterRole = "All";
        private string _filterSubject = "All";
        private string _filterSession = "All";

        private ObservableCollection<string> _roles = new ObservableCollection<string> { "All", "Founder", "Admin", "Coach", "Member" };
        private ObservableCollection<Subject> _availableSubjects;
        private ObservableCollection<Session> _availableSessions;

        public ObservableCollection<string> Roles => _roles;

        public ObservableCollection<Subject> AvailableSubjects
        {
            get => _availableSubjects;
            set => SetProperty(ref _availableSubjects, value);
        }

        public ObservableCollection<Session> AvailableSessions
        {
            get => _availableSessions;
            set => SetProperty(ref _availableSessions, value);
        }

        public ObservableCollection<MemberDisplay> Members
        {
            get => _members;
            set => SetProperty(ref _members, value);
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

        public string FilterSubject
        {
            get => _filterSubject;
            set
            {
                SetProperty(ref _filterSubject, value);
                ApplyFilters();
            }
        }

        public string FilterSession
        {
            get => _filterSession;
            set
            {
                SetProperty(ref _filterSession, value);
                ApplyFilters();
            }
        }

        public ICommand RemoveMemberCommand { get; }
        public ICommand AddMemberCommand { get; }

        public TeamMembersViewModel(Team team = null)
        {
            _team = team;
            RemoveMemberCommand = new RelayCommand(RemoveMember);
            AddMemberCommand = new RelayCommand(AddMember);
            if (_team != null)
            {
                LoadMembers();
            }
        }

        private void LoadMembers()
        {
            _allMembers = new ObservableCollection<MemberDisplay>();
            foreach (var tm in DataService.Instance.TeamMembers.Where(x => x.TeamId == _team.Id))
            {
                var user = DataService.Instance.Users.FirstOrDefault(u => u.Id == tm.UserId);
                if (user != null)
                {
                    _allMembers.Add(new MemberDisplay
                    {
                        UserId = user.Id,
                        FullName = user.FullName,
                        Email = user.Email,
                        Role = tm.Role,
                        AvatarPath = user.AvatarPath
                    });
                }
            }
            ApplyFilters();
            LoadFilters();
        }

        private void LoadFilters()
        {
            var subjects = DataService.Instance.Subjects.Where(s => s.TeamId == _team.Id).ToList();
            subjects.Insert(0, new Subject { Id = "All", Name = "All Subjects" });
            AvailableSubjects = new ObservableCollection<Subject>(subjects);

            var sessions = DataService.Instance.Sessions.Where(s => s.TeamId == _team.Id).ToList();
            sessions.Insert(0, new Session { Id = "All", Name = "All Sessions" });
            AvailableSessions = new ObservableCollection<Session>(sessions);
        }

        private void ApplyFilters()
        {
            var filtered = _allMembers.AsEnumerable();

            // Filter by search text
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(m => m.FullName.ToLower().Contains(SearchText.ToLower()) ||
                                             m.Email.ToLower().Contains(SearchText.ToLower()));
            }

            // Filter by role
            if (FilterRole != "All")
            {
                filtered = filtered.Where(m => m.Role == FilterRole);
            }

            // Filter by Subject (Member attended any session of this subject)
            if (FilterSubject != "All")
            {
                var sessionIds = DataService.Instance.Sessions
                    .Where(s => s.SubjectId == FilterSubject)
                    .Select(s => s.Id)
                    .ToList();
                
                var userIdsInSubject = DataService.Instance.Attendances
                    .Where(a => sessionIds.Contains(a.SessionId))
                    .Select(a => a.UserId)
                    .Distinct();
                
                filtered = filtered.Where(m => userIdsInSubject.Contains(m.UserId));
            }

            // Filter by Session (Member attended this session)
            if (FilterSession != "All")
            {
                 var userIdsInSession = DataService.Instance.Attendances
                    .Where(a => a.SessionId == FilterSession)
                    .Select(a => a.UserId)
                    .Distinct();

                filtered = filtered.Where(m => userIdsInSession.Contains(m.UserId));
            }

            Members = new ObservableCollection<MemberDisplay>(filtered.ToList());
        }

        private void RemoveMember(object parameter)
        {
            if (parameter is MemberDisplay member)
            {
                var tm = DataService.Instance.TeamMembers.FirstOrDefault(x => x.TeamId == _team.Id && x.UserId == member.UserId);
                if (tm != null)
                {
                    DataService.Instance.TeamMembers.Remove(tm);
                    DataService.Instance.Save();
                    LoadMembers();
                }
            }
        }

        private void AddMember(object obj)
        {
            // Mock - needs dialog in real app
        }
    }

    public class MemberDisplay
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string AvatarPath { get; set; }
    }
}
