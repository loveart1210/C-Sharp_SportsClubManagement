using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using SportsClubManagement.Helpers;
using SportsClubManagement.Models;
using SportsClubManagement.Services;

namespace SportsClubManagement.ViewModels
{
    public class PersonalScheduleViewModel : ViewModelBase
    {
        private User _currentUser;
        private ObservableCollection<Subject> _subjects;
        private ObservableCollection<Session> _sessions;
        private DateTime _selectedDate;
        
        // Subject creation
        private string _newSubjectName = string.Empty;
        private string _newSubjectDesc = string.Empty;

        // Session creation
        private string _newSessionName = string.Empty;
        private Subject _selectedSubject;
        private DateTime _newSessionStart;
        private DateTime _newSessionEnd;
        private string _newSessionNote = string.Empty;

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

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                SetProperty(ref _selectedDate, value);
                LoadSessions();
            }
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

        public string NewSessionName
        {
            get => _newSessionName;
            set => SetProperty(ref _newSessionName, value);
        }

        public Subject SelectedSubject
        {
            get => _selectedSubject;
            set => SetProperty(ref _selectedSubject, value);
        }

        public DateTime NewSessionStart
        {
            get => _newSessionStart;
            set => SetProperty(ref _newSessionStart, value);
        }

        public DateTime NewSessionEnd
        {
            get => _newSessionEnd;
            set => SetProperty(ref _newSessionEnd, value);
        }

        public string NewSessionNote
        {
            get => _newSessionNote;
            set => SetProperty(ref _newSessionNote, value);
        }

        public ICommand AddSubjectCommand { get; }
        public ICommand RemoveSubjectCommand { get; }
        public ICommand AddSessionCommand { get; }
        public ICommand RemoveSessionCommand { get; }
        public ICommand ExportScheduleCommand { get; }

        public PersonalScheduleViewModel()
        {
            _currentUser = DataService.Instance.CurrentUser;
            _selectedDate = DateTime.Today;
            _newSessionStart = DateTime.Now;
            _newSessionEnd = DateTime.Now.AddHours(1);

            if (_currentUser != null)
            {
                LoadSubjects();
                LoadSessions();
            }

            AddSubjectCommand = new RelayCommand(AddSubject, CanAddSubject);
            RemoveSubjectCommand = new RelayCommand(RemoveSubject);
            AddSessionCommand = new RelayCommand(AddSession, CanAddSession);
            RemoveSessionCommand = new RelayCommand(RemoveSession);
            ExportScheduleCommand = new RelayCommand(ExportSchedule);
        }

        private void LoadSubjects()
        {
            var list = DataService.Instance.Subjects
                .Where(s => s.UserId == _currentUser.Id) // Filter by User
                .ToList();
            Subjects = new ObservableCollection<Subject>(list);
            
            // Should also include subjects from teams the user is in? 
            // Requirement says "Quản lý môn tập cá nhân", implying items created by user.
            // But usually "Personal Schedule" implies seeing everything.
            // For "Quản lý" (Management) specifically, it usually means CRUD own items.
            // Reading functions.txt: "Quản lý môn tập cá nhân (CRUD dạng lịch)"
            // So I will stick to items where UserId == CurrentUser.Id
            
            if (SelectedSubject == null) 
                SelectedSubject = Subjects.FirstOrDefault();
        }

        private void LoadSessions()
        {
            var list = DataService.Instance.Sessions
                .Where(s => s.UserId == _currentUser.Id && s.StartTime.Date == SelectedDate.Date)
                .OrderBy(s => s.StartTime)
                .ToList();
            Sessions = new ObservableCollection<Session>(list);
        }

        private bool CanAddSubject(object obj)
        {
            return !string.IsNullOrWhiteSpace(NewSubjectName);
        }

        private void AddSubject(object obj)
        {
            var sub = new Subject
            {
                UserId = _currentUser.Id,
                Name = NewSubjectName,
                Description = NewSubjectDesc,
                TeamId = null
            };
            DataService.Instance.Subjects.Add(sub);
            DataService.Instance.Save();
            
            NewSubjectName = "";
            NewSubjectDesc = "";
            LoadSubjects();
        }

        private void RemoveSubject(object obj)
        {
            if (obj is Subject s)
            {
                DataService.Instance.Subjects.Remove(s);
                DataService.Instance.Save();
                LoadSubjects();
            }
        }

        private bool CanAddSession(object obj)
        {
            return !string.IsNullOrWhiteSpace(NewSessionName) && SelectedSubject != null;
        }

        private void AddSession(object obj)
        {
            var sess = new Session
            {
                UserId = _currentUser.Id,
                Name = NewSessionName,
                SubjectId = SelectedSubject.Id,
                StartTime = SelectedDate.Date + NewSessionStart.TimeOfDay,
                EndTime = SelectedDate.Date + NewSessionEnd.TimeOfDay,
                Note = NewSessionNote,
                TeamId = null
            };
            DataService.Instance.Sessions.Add(sess);
            DataService.Instance.Save();
            LoadSessions();
        }

        private void RemoveSession(object obj)
        {
            if (obj is Session s)
            {
                DataService.Instance.Sessions.Remove(s);
                DataService.Instance.Save();
                LoadSessions();
            }
        }

        private void ExportSchedule(object obj)
        {
            if (_currentUser != null)
            {
                var filePath = ExportService.ExportPersonalScheduleToCSV(_currentUser);
                System.Windows.MessageBox.Show($"Lịch tập cá nhân đã được xuất tại:\n{filePath}", "Xuất thành công", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
        }
    }
}
