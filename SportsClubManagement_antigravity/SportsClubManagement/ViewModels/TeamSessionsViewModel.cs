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
        private Team? _team;
        private ObservableCollection<Session>? _sessions;
        private DateTime _selectedDate;
        private ObservableCollection<Subject>? _availableSubjects;
        private string _newSessionName = string.Empty;
        private Subject? _selectedSubject;
        private string _newSessionStartString;
        private string _newSessionEndString;
        private string _newSessionNote = string.Empty;
        
        private bool _isMemberSelectionVisible;
        private ObservableCollection<MemberSelectionItem>? _memberSelectionList;

        public ObservableCollection<Session>? Sessions
        {
            get => _sessions;
            set => SetProperty(ref _sessions, value);
        }

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (SetProperty(ref _selectedDate, value))
                {
                    _hasExplicitlySelectedDate = true;
                    OnPropertyChanged(nameof(ScheduleHeaderText));
                    // Reset overriding Date filters when clicking on a calendar date
                    _filterDay = "Tất cả";
                    OnPropertyChanged(nameof(FilterDay));
                    _filterMonth = "Tất cả";
                    OnPropertyChanged(nameof(FilterMonth));
                    _filterYear = "Tất cả";
                    OnPropertyChanged(nameof(FilterYear));
                    LoadSessions();
                }
            }
        }
        
        public string ScheduleHeaderText => _hasExplicitlySelectedDate 
            ? $"Danh sách buổi tập - {SelectedDate:dd/MM/yyyy}" 
            : "Danh sách buổi tập";

        public ObservableCollection<Subject>? AvailableSubjects
        {
            get => _availableSubjects;
            set => SetProperty(ref _availableSubjects, value);
        }

        public string NewSessionName
        {
            get => _newSessionName;
            set => SetProperty(ref _newSessionName, value);
        }

        public Subject? SelectedSubject
        {
            get => _selectedSubject;
            set => SetProperty(ref _selectedSubject, value);
        }

        public string NewSessionStartString
        {
            get => _newSessionStartString;
            set => SetProperty(ref _newSessionStartString, value);
        }

        public string NewSessionEndString
        {
            get => _newSessionEndString;
            set => SetProperty(ref _newSessionEndString, value);
        }

        public string NewSessionNote
        {
            get => _newSessionNote;
            set => SetProperty(ref _newSessionNote, value);
        }

        public bool IsMemberSelectionVisible
        {
            get => _isMemberSelectionVisible;
            set => SetProperty(ref _isMemberSelectionVisible, value);
        }

        public ObservableCollection<MemberSelectionItem>? MemberSelectionList
        {
            get => _memberSelectionList;
            set => SetProperty(ref _memberSelectionList, value);
        }

        public bool IsFounderOrCoach
        {
            get
            {
                if (_team == null) return false;
                var currentUser = DataService.Instance.CurrentUser;
                if (currentUser == null) return false;
                
                var member = DataService.Instance.TeamMembers
                    .FirstOrDefault(tm => tm.TeamId == _team.Id && tm.UserId == currentUser.Id);
                
                return member != null && (member.Role == "Founder" || member.Role == "Coach");
            }
        }

        // Filtering
        private string _filterStatus = "Tất cả";
        private string _filterDay = "Tất cả";
        private string _filterMonth = "Tất cả";
        private string _filterYear = "Tất cả";
        private string _filterSessionName = "Tất cả";
        private string _filterSubjectName = "Tất cả";
        private string _filterStartTime = "Tất cả";
        private string _filterEndTime = "Tất cả";
        private bool _hasExplicitlySelectedDate;

        private ObservableCollection<string>? _dayOptions;
        private ObservableCollection<string>? _monthOptions;
        private ObservableCollection<string>? _yearOptions;
        private ObservableCollection<string>? _sessionNameOptions;
        private ObservableCollection<string>? _subjectNameOptions;
        private ObservableCollection<string>? _startTimeOptions;
        private ObservableCollection<string>? _endTimeOptions;

        // Filter Properties
        public string FilterStatus { get => _filterStatus; set { if (SetProperty(ref _filterStatus, value)) LoadSessions(); } }
        public string FilterDay { get => _filterDay; set { if (SetProperty(ref _filterDay, value)) { _hasExplicitlySelectedDate = false; OnPropertyChanged(nameof(ScheduleHeaderText)); LoadSessions(); } } }
        public string FilterMonth { get => _filterMonth; set { if (SetProperty(ref _filterMonth, value)) { _hasExplicitlySelectedDate = false; OnPropertyChanged(nameof(ScheduleHeaderText)); LoadSessions(); } } }
        public string FilterYear { get => _filterYear; set { if (SetProperty(ref _filterYear, value)) { _hasExplicitlySelectedDate = false; OnPropertyChanged(nameof(ScheduleHeaderText)); LoadSessions(); } } }
        public string FilterSessionName { get => _filterSessionName; set { if (SetProperty(ref _filterSessionName, value)) LoadSessions(); } }
        public string FilterSubjectName { get => _filterSubjectName; set { if (SetProperty(ref _filterSubjectName, value)) LoadSessions(); } }
        public string FilterStartTime { get => _filterStartTime; set { if (SetProperty(ref _filterStartTime, value)) LoadSessions(); } }
        public string FilterEndTime { get => _filterEndTime; set { if (SetProperty(ref _filterEndTime, value)) LoadSessions(); } }

        public ObservableCollection<string> StatusOptions { get; } = new ObservableCollection<string> { "Tất cả", "Chưa bắt đầu", "Đang diễn ra", "Đã kết thúc" };
        public ObservableCollection<string>? DayOptions { get => _dayOptions; set => SetProperty(ref _dayOptions, value); }
        public ObservableCollection<string>? MonthOptions { get => _monthOptions; set => SetProperty(ref _monthOptions, value); }
        public ObservableCollection<string>? YearOptions { get => _yearOptions; set => SetProperty(ref _yearOptions, value); }
        public ObservableCollection<string>? SessionNameOptions { get => _sessionNameOptions; set => SetProperty(ref _sessionNameOptions, value); }
        public ObservableCollection<string>? SubjectNameOptions { get => _subjectNameOptions; set => SetProperty(ref _subjectNameOptions, value); }
        public ObservableCollection<string>? StartTimeOptions { get => _startTimeOptions; set => SetProperty(ref _startTimeOptions, value); }
        public ObservableCollection<string>? EndTimeOptions { get => _endTimeOptions; set => SetProperty(ref _endTimeOptions, value); }

        public ICommand AddSessionCommand { get; }
        public ICommand RemoveSessionCommand { get; }
        public ICommand ExportScheduleCommand { get; }
        public ICommand OpenMemberSelectionCommand { get; }
        public ICommand CloseMemberSelectionCommand { get; }
        public ICommand ClearFiltersCommand { get; }

        public TeamSessionsViewModel(Team? team = null)
        {
            _team = team;
            _selectedDate = DateTime.Today;
            _hasExplicitlySelectedDate = true; // Default to showing today's sessions
            _newSessionStartString = DateTime.Now.ToString("HH:mm");
            _newSessionEndString = DateTime.Now.AddHours(2).ToString("HH:mm");

            if (_team != null)
            {
                LoadSubjects();
                InitializeFilterOptions();
                LoadSessions();
                LoadMembersForSelection();
            }

            AddSessionCommand = new RelayCommand(AddSession, CanAddSession);
            RemoveSessionCommand = new RelayCommand(RemoveSession, CanRemoveSession);
            ExportScheduleCommand = new RelayCommand(ExportSchedule);
            OpenMemberSelectionCommand = new RelayCommand(o => {
                LoadMembersForSelection();
                IsMemberSelectionVisible = true;
            });
            CloseMemberSelectionCommand = new RelayCommand(o => IsMemberSelectionVisible = false);
            ClearFiltersCommand = new RelayCommand(o => ClearFilters());
        }

        private void InitializeFilterOptions()
        {
            DayOptions = new ObservableCollection<string>(new[] { "Tất cả" }.Concat(Enumerable.Range(1, 31).Select(i => i.ToString())).ToList());
            MonthOptions = new ObservableCollection<string>(new[] { "Tất cả" }.Concat(Enumerable.Range(1, 12).Select(i => i.ToString())).ToList());
            UpdateDynamicFilterOptions();
        }

        private void UpdateDynamicFilterOptions()
        {
            if (_team == null) return;
            var teamSessions = DataService.Instance.Sessions.Where(s => s.TeamId == _team.Id).ToList();

            SessionNameOptions = new ObservableCollection<string>(new[] { "Tất cả" }.Concat(teamSessions.Select(s => s.Name).Distinct()).ToList());
            
            // For subjects, we can use the subjects we already loaded, or filter from sessions if we want only used subjects
            SubjectNameOptions = new ObservableCollection<string>(new[] { "Tất cả" }.Concat((AvailableSubjects ?? new ObservableCollection<Subject>()).Select(s => s.Name)).ToList());
            
            StartTimeOptions = new ObservableCollection<string>(new[] { "Tất cả" }.Concat(teamSessions.Select(s => s.StartTime.ToString("HH:mm")).Distinct().OrderBy(t => t)).ToList());
            EndTimeOptions = new ObservableCollection<string>(new[] { "Tất cả" }.Concat(teamSessions.Select(s => s.EndTime.ToString("HH:mm")).Distinct().OrderBy(t => t)).ToList());

            var years = teamSessions.Select(s => s.StartTime.Year.ToString()).Distinct().OrderBy(y => y).ToList();
            YearOptions = new ObservableCollection<string>(new[] { "Tất cả" }.Concat(years).ToList());
        }

        private void LoadMembersForSelection()
        {
            if (_team == null) return;
            var members = DataService.Instance.TeamMembers.Where(tm => tm.TeamId == _team.Id).ToList();
            var list = new ObservableCollection<MemberSelectionItem>();
            foreach (var tm in members)
            {
                var user = DataService.Instance.Users.FirstOrDefault(u => u.Id == tm.UserId);
                if (user != null)
                {
                    list.Add(new MemberSelectionItem
                    {
                        UserId = user.Id,
                        FullName = user.FullName,
                        Role = tm.Role,
                        IsSelected = true // Default to selected
                    });
                }
            }
            MemberSelectionList = list;
        }

        private void LoadSubjects()
        {
            if (_team == null) return;
            var subs = DataService.Instance.Subjects.Where(s => s.TeamId == _team.Id).ToList();
            AvailableSubjects = new ObservableCollection<Subject>(subs);
            SelectedSubject = AvailableSubjects.FirstOrDefault();
        }

        public void RefreshData()
        {
            LoadSubjects(); // Refresh subjects in case they changed
            UpdateDynamicFilterOptions();
            LoadSessions();
        }

        private void LoadSessions()
        {
            if (_team == null) return;
            var now = DateTime.Now;
            
            var allSessions = DataService.Instance.Sessions
                .Where(s => s.TeamId == _team.Id)
                .ToList();

            var filtered = allSessions.AsQueryable();

            // Apply filters
            if (FilterStatus != "Tất cả")
            {
                 // "Chưa bắt đầu", "Đang diễn ra", "Đã kết thúc"
                 if (FilterStatus == "Chưa bắt đầu") filtered = filtered.Where(s => s.StartTime > now);
                 else if (FilterStatus == "Đang diễn ra") filtered = filtered.Where(s => s.StartTime <= now && s.EndTime >= now);
                 else if (FilterStatus == "Đã kết thúc") filtered = filtered.Where(s => s.EndTime < now);
            }

            if (_hasExplicitlySelectedDate)
            {
                filtered = filtered.Where(s => s.StartTime.Date == SelectedDate.Date);
            }
            else
            {
                if (FilterDay != "Tất cả" && int.TryParse(FilterDay, out int d)) filtered = filtered.Where(s => s.StartTime.Day == d);
                if (FilterMonth != "Tất cả" && int.TryParse(FilterMonth, out int m)) filtered = filtered.Where(s => s.StartTime.Month == m);
                if (FilterYear != "Tất cả" && int.TryParse(FilterYear, out int y)) filtered = filtered.Where(s => s.StartTime.Year == y);
            }

            if (FilterSessionName != "Tất cả")
                filtered = filtered.Where(s => s.Name == FilterSessionName);

            if (FilterSubjectName != "Tất cả")
            {
                var subjectsDict = DataService.Instance.Subjects.ToDictionary(sub => sub.Id, sub => sub.Name);
                filtered = filtered.Where(s => subjectsDict.ContainsKey(s.SubjectId) && subjectsDict[s.SubjectId] == FilterSubjectName);
            }

            if (FilterStartTime != "Tất cả")
                filtered = filtered.Where(s => s.StartTime.ToString("HH:mm") == FilterStartTime);

            if (FilterEndTime != "Tất cả")
                filtered = filtered.Where(s => s.EndTime.ToString("HH:mm") == FilterEndTime);

            var list = filtered.OrderBy(s => s.StartTime).ToList();
            Sessions = new ObservableCollection<Session>(list);
            
            OnPropertyChanged(nameof(IsFounderOrCoach));
        }

        private void ClearFilters()
        {
            _filterStatus = "Tất cả";
            _filterDay = "Tất cả";
            _filterMonth = "Tất cả";
            _filterYear = "Tất cả";
            _filterSessionName = "Tất cả";
            _filterSubjectName = "Tất cả";
            _filterStartTime = "Tất cả";
            _filterEndTime = "Tất cả";
            
            _hasExplicitlySelectedDate = false;
            OnPropertyChanged(nameof(ScheduleHeaderText));

            OnPropertyChanged(nameof(FilterStatus));
            OnPropertyChanged(nameof(FilterDay));
            OnPropertyChanged(nameof(FilterMonth));
            OnPropertyChanged(nameof(FilterYear));
            OnPropertyChanged(nameof(FilterSessionName));
            OnPropertyChanged(nameof(FilterSubjectName));
            OnPropertyChanged(nameof(FilterStartTime));
            OnPropertyChanged(nameof(FilterEndTime));

            LoadSessions();
        }

        private bool CanAddSession(object? obj)
        {
            return IsFounderOrCoach && !string.IsNullOrWhiteSpace(NewSessionName) && SelectedSubject != null;
        }

        private void AddSession(object? obj)
        {
            if (_team == null || SelectedSubject == null) return;

            if (!TimeSpan.TryParse(NewSessionStartString, out TimeSpan startTime) || 
                !TimeSpan.TryParse(NewSessionEndString, out TimeSpan endTime))
            {
                System.Windows.MessageBox.Show("Định dạng thời gian không hợp lệ (HH:mm).", "Lỗi", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }

            var sess = new Session
            {
                TeamId = _team.Id,
                Name = NewSessionName,
                SubjectId = SelectedSubject.Id,
                StartTime = SelectedDate.Date + startTime,
                EndTime = SelectedDate.Date + endTime,
                Note = NewSessionNote
            };
            DataService.Instance.Sessions.Add(sess);

            // Add Attendance records for selected members
            if (MemberSelectionList != null)
            {
                foreach (var item in MemberSelectionList.Where(x => x.IsSelected))
                {
                    var attendance = new Attendance
                    {
                        SessionId = sess.Id,
                        UserId = item.UserId,
                        IsPresent = false, // Not present yet
                        RecordedDate = DateTime.Now
                    };
                    DataService.Instance.Attendances.Add(attendance);
                }
            }

            DataService.Instance.Save();
            UpdateDynamicFilterOptions();
            LoadSessions();
            
            // Allow selecting members for next session again
            // IsMemberSelectionVisible = false; 
            NewSessionName = string.Empty;
            NewSessionNote = string.Empty;
            
            System.Windows.MessageBox.Show("Thêm buổi tập thành công!", "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        private bool CanRemoveSession(object? obj) => IsFounderOrCoach;

        private void RemoveSession(object? obj)
        {
            if (obj is Session s)
            {
                 var result = System.Windows.MessageBox.Show($"Bạn có chắc chắn muốn xóa buổi tập '{s.Name}'?", "Xác nhận", 
                    System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);
                
                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    // Remove session
                    DataService.Instance.Sessions.Remove(s);
                    
                    // Remove associated attendance records
                    var attendances = DataService.Instance.Attendances.Where(a => a.SessionId == s.Id).ToList();
                    foreach(var att in attendances)
                    {
                        DataService.Instance.Attendances.Remove(att);
                    }

                    DataService.Instance.Save();
                    UpdateDynamicFilterOptions();
                    LoadSessions();
                }
            }
        }

        private void ExportSchedule(object? obj)
        {
            if (_team != null)
            {
                var filePath = ExportService.ExportScheduleToCSV(_team);
                System.Windows.MessageBox.Show($"Lịch tập đã được xuất tại:\n{filePath}", "Xuất thành công", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
        }
    }

    public class MemberSelectionItem : ViewModelBase
    {
        private bool _isSelected;
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}
