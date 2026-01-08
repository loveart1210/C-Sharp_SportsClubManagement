using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using SportsClubManagement.Helpers;
using SportsClubManagement.Models;
using SportsClubManagement.Services;

namespace SportsClubManagement.ViewModels
{
    public class TeamSubjectsViewModel : ViewModelBase
    {
        private Team _team;
        private ObservableCollection<Subject> _subjects;
        private string _newSubjectName = string.Empty;
        private string _newSubjectDesc = string.Empty;
        private string _searchText = string.Empty;
        private string _filterMember = "All";
        private string _filterSession = "All";

        private ObservableCollection<MemberDisplay> _availableMembers;
        private ObservableCollection<Session> _availableSessions;
        private ObservableCollection<Subject> _allSubjects;

        public ObservableCollection<MemberDisplay> AvailableMembers
        {
            get => _availableMembers;
            set => SetProperty(ref _availableMembers, value);
        }

        public ObservableCollection<Session> AvailableSessions
        {
            get => _availableSessions;
            set => SetProperty(ref _availableSessions, value);
        }

        public ObservableCollection<Subject> Subjects
        {
            get => _subjects;
            set => SetProperty(ref _subjects, value);
        }

        public string NewSubjectName
        {
            get => _newSubjectName;
            set => SetProperty(ref _newSubjectName, value);
        }

        public string NewSubjectDesc
        {
            get => _newSubjectDesc;
            set => SetProperty(ref _newSubjectDesc, value);
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

        public string FilterMember
        {
            get => _filterMember;
            set
            {
                SetProperty(ref _filterMember, value);
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

        public ICommand AddSubjectCommand { get; }
        public ICommand RemoveSubjectCommand { get; }

        public TeamSubjectsViewModel(Team team = null)
        {
            _team = team;
            if (_team != null)
            {
                LoadSubjects();
            }
            AddSubjectCommand = new RelayCommand(AddSubject, CanAddSubject);
            RemoveSubjectCommand = new RelayCommand(RemoveSubject);
        }

        private void LoadSubjects()
        {
            var list = DataService.Instance.Subjects.Where(s => s.TeamId == _team.Id).ToList();
            _allSubjects = new ObservableCollection<Subject>(list);
            
            LoadFilters();
            ApplyFilters();
        }

        private void LoadFilters()
        {
            var sessions = DataService.Instance.Sessions.Where(s => s.TeamId == _team.Id).ToList();
            sessions.Insert(0, new Session { Id = "All", Name = "All Sessions" });
            AvailableSessions = new ObservableCollection<Session>(sessions);

            var members = new ObservableCollection<MemberDisplay>();
            members.Add(new MemberDisplay { UserId = "All", FullName = "All Members" });

            foreach (var tm in DataService.Instance.TeamMembers.Where(x => x.TeamId == _team.Id))
            {
                var user = DataService.Instance.Users.FirstOrDefault(u => u.Id == tm.UserId);
                if (user != null)
                {
                    members.Add(new MemberDisplay { UserId = user.Id, FullName = user.FullName });
                }
            }
            AvailableMembers = members;
        }

        private void ApplyFilters()
        {
            var filtered = _allSubjects.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(s => s.Name.ToLower().Contains(SearchText.ToLower()) || 
                                             s.Description.ToLower().Contains(SearchText.ToLower()));
            }

            if (FilterSession != "All")
            {
                // Subjects that have this session
                var session = DataService.Instance.Sessions.FirstOrDefault(s => s.Id == FilterSession);
                if (session != null)
                {
                    filtered = filtered.Where(s => s.Id == session.SubjectId);
                }
            }

            if (FilterMember != "All")
            {
                // Subjects where Member attended any session
                var attendedSessionIds = DataService.Instance.Attendances
                    .Where(a => a.UserId == FilterMember)
                    .Select(a => a.SessionId)
                    .ToList();
                
                var subjectIds = DataService.Instance.Sessions
                    .Where(s => attendedSessionIds.Contains(s.Id))
                    .Select(s => s.SubjectId)
                    .Distinct();

                filtered = filtered.Where(s => subjectIds.Contains(s.Id));
            }

            Subjects = new ObservableCollection<Subject>(filtered.ToList());
        }

        private bool CanAddSubject(object obj)
        {
            return !string.IsNullOrWhiteSpace(NewSubjectName);
        }

        private void AddSubject(object obj)
        {
            var newSub = new Subject
            {
                TeamId = _team.Id,
                Name = NewSubjectName,
                Description = NewSubjectDesc
            };
            DataService.Instance.Subjects.Add(newSub);
            DataService.Instance.Save();
            
            NewSubjectName = string.Empty;
            NewSubjectDesc = string.Empty;
            LoadSubjects();
        }

        private void RemoveSubject(object parameter)
        {
            if (parameter is Subject sub)
            {
                DataService.Instance.Subjects.Remove(sub);
                DataService.Instance.Save();
                LoadSubjects();
            }
        }
    }
}
