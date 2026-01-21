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
        private string _newSessionStartString;
        private string _newSessionEndString;
        private string _newSessionNote = string.Empty;
        
        private bool _isMemberSelectionVisible;
        private ObservableCollection<MemberSelectionItem> _memberSelectionList;

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
                OnPropertyChanged(nameof(ScheduleHeaderText));
                LoadSessions();
            }
        }
        
        public string ScheduleHeaderText => $"Danh sách buổi tập - {SelectedDate:dd/MM/yyyy}";

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

        public ObservableCollection<MemberSelectionItem> MemberSelectionList
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

        public ICommand AddSessionCommand { get; }
        public ICommand RemoveSessionCommand { get; }
        public ICommand ExportScheduleCommand { get; }
        public ICommand OpenMemberSelectionCommand { get; }
        public ICommand CloseMemberSelectionCommand { get; }

        public TeamSessionsViewModel(Team team = null)
        {
            _team = team;
            _selectedDate = DateTime.Today;
            _newSessionStartString = DateTime.Now.ToString("HH:mm");
            _newSessionEndString = DateTime.Now.AddHours(2).ToString("HH:mm");

            if (_team != null)
            {
                LoadSubjects();
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
        }

        private void LoadMembersForSelection()
        {
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
            var subs = DataService.Instance.Subjects.Where(s => s.TeamId == _team.Id).ToList();
            AvailableSubjects = new ObservableCollection<Subject>(subs);
            SelectedSubject = AvailableSubjects.FirstOrDefault();
        }

        public void RefreshData()
        {
            LoadSubjects(); // Refresh subjects in case they changed
            LoadSessions();
        }

        private void LoadSessions()
        {
            if (_team == null) return;
            
            var list = DataService.Instance.Sessions
                .Where(s => s.TeamId == _team.Id && s.StartTime.Date == SelectedDate.Date)
                .OrderBy(s => s.StartTime)
                .ToList();
            Sessions = new ObservableCollection<Session>(list);
            
            OnPropertyChanged(nameof(IsFounderOrCoach));
        }

        private bool CanAddSession(object obj)
        {
            return IsFounderOrCoach && !string.IsNullOrWhiteSpace(NewSessionName) && SelectedSubject != null;
        }

        private void AddSession(object obj)
        {
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

            DataService.Instance.Save();
            LoadSessions();
            
            // Allow selecting members for next session again
            // IsMemberSelectionVisible = false; 
            NewSessionName = string.Empty;
            NewSessionNote = string.Empty;
            
            System.Windows.MessageBox.Show("Thêm buổi tập thành công!", "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        private bool CanRemoveSession(object obj) => IsFounderOrCoach;

        private void RemoveSession(object obj)
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
                    LoadSessions();
                }
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

    public class MemberSelectionItem : ViewModelBase
    {
        private bool _isSelected;
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}
