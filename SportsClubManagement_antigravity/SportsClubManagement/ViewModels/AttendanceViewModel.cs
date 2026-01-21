using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using SportsClubManagement.Helpers;
using SportsClubManagement.Models;
using SportsClubManagement.Services;

namespace SportsClubManagement.ViewModels
{
    public class AttendanceViewModel : ViewModelBase
    {
        private Team? _team;
        private Session? _selectedSession;
        private DateTime _selectedDate;
        private ObservableCollection<Session> _sessions = new ObservableCollection<Session>();
        private ObservableCollection<AttendanceRecord> _attendanceRecords = new ObservableCollection<AttendanceRecord>();
        private string _attendanceSummary = string.Empty;

        public Team? Team
        {
            get => _team;
            set => SetProperty(ref _team, value);
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

        public ObservableCollection<Session> Sessions
        {
            get => _sessions;
            set => SetProperty(ref _sessions, value);
        }

        public Session? SelectedSession
        {
            get => _selectedSession;
            set
            {
                SetProperty(ref _selectedSession, value);
                LoadAttendance();
            }
        }

        public ObservableCollection<AttendanceRecord> AttendanceRecords
        {
            get => _attendanceRecords;
            set => SetProperty(ref _attendanceRecords, value);
        }

        public string HeaderText => SelectedSession != null ? $"Điểm danh: {SelectedSession.Name}" : "Vui lòng chọn buổi tập";

        public string AttendanceSummary
        {
            get => _attendanceSummary;
            set => SetProperty(ref _attendanceSummary, value);
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

        public ICommand SaveAttendanceCommand { get; }

        public AttendanceViewModel(Team? team = null)
        {
            _team = team;
            _selectedDate = DateTime.Today;
            
            SaveAttendanceCommand = new RelayCommand(SaveAttendance, CanSaveAttendance);

            if (_team != null)
            {
                LoadSessions();
            }
        }

        private void LoadSessions()
        {
            if (_team == null) return;
            var sessions = DataService.Instance.Sessions
                .Where(s => s.TeamId == _team.Id && s.StartTime.Date == _selectedDate.Date)
                .OrderBy(s => s.StartTime)
                .ToList();
            
            // System.Windows.MessageBox.Show($"Debug LoadSessions:\nDate: {_selectedDate}\nTeamId: {_team.Id}\nFound: {sessions.Count}\nTotal DS Sessions: {DataService.Instance.Sessions.Count}");

            
            Sessions = new ObservableCollection<Session>(sessions);
            SelectedSession = sessions.FirstOrDefault();
        }

        public void RefreshData()
        {
            LoadSessions();
        }

        private void LoadAttendance()
        {
            if (_selectedSession == null || _team == null)
            {
                AttendanceRecords = new ObservableCollection<AttendanceRecord>();
                AttendanceSummary = "";
                OnPropertyChanged(nameof(HeaderText));
                return;
            }

            OnPropertyChanged(nameof(HeaderText));

            // Logic: Load Attendance records for this session.
            // These records delineate WHO is participating.
            var attendances = DataService.Instance.Attendances
                .Where(a => a.SessionId == _selectedSession.Id)
                .ToList();

            var records = new ObservableCollection<AttendanceRecord>();
            int presentCount = 0;

            foreach (var att in attendances)
            {
                var user = DataService.Instance.Users.FirstOrDefault(u => u.Id == att.UserId);
                var memberRole = DataService.Instance.TeamMembers
                         .FirstOrDefault(tm => tm.TeamId == _team.Id && tm.UserId == att.UserId)?.Role ?? "Member";

                if (user != null)
                {
                    records.Add(new AttendanceRecord
                    {
                        AttendanceId = att.Id,
                        SessionId = att.SessionId,
                        UserId = user.Id,
                        UserName = user.FullName,
                        Role = memberRole,
                        IsPresent = att.IsPresent,
                        Note = att.Note
                    });
                    if (att.IsPresent) presentCount++;
                }
            }

            AttendanceRecords = records;
            AttendanceSummary = $"{presentCount}/{records.Count} có mặt";
            OnPropertyChanged(nameof(IsFounderOrCoach));
        }

        private bool CanSaveAttendance(object? obj) => IsFounderOrCoach;

        private void SaveAttendance(object? obj)
        {
            if (_selectedSession == null) return;

            try
            {
                foreach (var record in AttendanceRecords)
                {
                    var existing = DataService.Instance.Attendances
                        .FirstOrDefault(a => a.Id == record.AttendanceId);

                    if (existing != null)
                    {
                        existing.IsPresent = record.IsPresent;
                        existing.Note = record.Note;
                    }
                }

                DataService.Instance.Save();
                System.Windows.MessageBox.Show("Lưu điểm danh thành công!", "Thông báo", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                
                LoadAttendance(); // Update summary
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
    }

    public class AttendanceRecord : ViewModelBase
    {
        public string AttendanceId { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        
        private bool _isPresent;
        public bool IsPresent 
        { 
            get => _isPresent;
            set => SetProperty(ref _isPresent, value);
        }
        
        private string _note = string.Empty;
        public string Note 
        { 
            get => _note; 
            set => SetProperty(ref _note, value);
        }
    }
}
