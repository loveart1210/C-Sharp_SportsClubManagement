using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using SportsClubManagement.Helpers;
using SportsClubManagement.Models;
using SportsClubManagement.Services;

namespace SportsClubManagement.ViewModels
{
    public class TeamSessionsViewModel : ViewModelBase
    {
        private Team _team;
        private ObservableCollection<Session> _sessions;
        private DateTime _selectedDate;
        private ObservableCollection<Subject> _availableSubjects;
        private string _newSessionName = string.Empty;
        private Subject _selectedSubject;
        private DateTime _newSessionStart;
        private DateTime _newSessionEnd;
        private string _newSessionNote = string.Empty;
        
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

        public ObservableCollection<Subject> AvailableSubjects
        {
            get => _availableSubjects;
            set => SetProperty(ref _availableSubjects, value);
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

        public ICommand AddSessionCommand { get; }
        public ICommand RemoveSessionCommand { get; }
        public ICommand ExportScheduleCommand { get; }

        public TeamSessionsViewModel(Team team = null)
        {
            _team = team;
            _selectedDate = DateTime.Today;
            _newSessionStart = DateTime.Now;
            _newSessionEnd = DateTime.Now.AddHours(2);

            if (_team != null)
            {
                LoadSubjects();
                LoadSessions();
            }

            AddSessionCommand = new RelayCommand(AddSession, CanAddSession);
            RemoveSessionCommand = new RelayCommand(RemoveSession);
            ExportScheduleCommand = new RelayCommand(ExportSchedule);
        }

        private void LoadSubjects()
        {
            var subs = DataService.Instance.Subjects.Where(s => s.TeamId == _team.Id).ToList();
            AvailableSubjects = new ObservableCollection<Subject>(subs);
            SelectedSubject = AvailableSubjects.FirstOrDefault();
        }

        private void LoadSessions()
        {
            if (_team == null) return;
            
            var list = DataService.Instance.Sessions
                .Where(s => s.TeamId == _team.Id && s.StartTime.Date == SelectedDate.Date)
                .OrderBy(s => s.StartTime)
                .ToList();
            Sessions = new ObservableCollection<Session>(list);
        }

        private bool CanAddSession(object obj)
        {
            return !string.IsNullOrWhiteSpace(NewSessionName) && SelectedSubject != null;
        }

        private void AddSession(object obj)
        {
            var sess = new Session
            {
                TeamId = _team.Id,
                Name = NewSessionName,
                SubjectId = SelectedSubject.Id,
                StartTime = SelectedDate.Date + NewSessionStart.TimeOfDay,
                EndTime = SelectedDate.Date + NewSessionEnd.TimeOfDay,
                Note = NewSessionNote
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
            if (_team != null)
            {
                var filePath = ExportService.ExportScheduleToCSV(_team);
                System.Windows.MessageBox.Show($"Lịch tập đã được xuất tại:\n{filePath}", "Xuất thành công", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
        }
    }
}
