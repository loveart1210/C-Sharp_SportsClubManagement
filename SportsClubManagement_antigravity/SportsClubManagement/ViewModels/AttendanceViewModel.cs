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
        private Team _team;
        private Session _selectedSession;
        private DateTime _selectedDate;
        private ObservableCollection<Session> _sessions;
        private ObservableCollection<AttendanceRecord> _attendanceRecords;
        private string _message;

        public Team Team
        {
            get => _team;
            set => SetProperty(ref _team, value);
        }

        public Session SelectedSession
        {
            get => _selectedSession;
            set
            {
                SetProperty(ref _selectedSession, value);
                LoadAttendance();
            }
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

        public ObservableCollection<AttendanceRecord> AttendanceRecords
        {
            get => _attendanceRecords;
            set => SetProperty(ref _attendanceRecords, value);
        }

        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public ICommand SaveAttendanceCommand { get; }
        public ICommand MarkAllPresentCommand { get; }
        public ICommand MarkAllAbsentCommand { get; }

        public AttendanceViewModel(Team team = null)
        {
            _team = team;
            _selectedDate = DateTime.Now;
            
            SaveAttendanceCommand = new RelayCommand(SaveAttendance);
            MarkAllPresentCommand = new RelayCommand(o => MarkAll(true));
            MarkAllAbsentCommand = new RelayCommand(o => MarkAll(false));

            if (_team != null)
            {
                LoadSessions();
            }
        }

        private void LoadSessions()
        {
            var sessions = DataService.Instance.Sessions
                .Where(s => s.TeamId == _team.Id && s.StartTime.Date == _selectedDate.Date)
                .ToList();
            
            Sessions = new ObservableCollection<Session>(sessions);
            Message = sessions.Count == 0 ? "Không có buổi tập nào trong ngày này" : "";
        }

        private void LoadAttendance()
        {
            if (_selectedSession == null)
            {
                AttendanceRecords = new ObservableCollection<AttendanceRecord>();
                return;
            }

            var records = new ObservableCollection<AttendanceRecord>();
            var teamMembers = DataService.Instance.TeamMembers
                .Where(tm => tm.TeamId == _team.Id)
                .ToList();

            foreach (var member in teamMembers)
            {
                var user = DataService.Instance.Users.FirstOrDefault(u => u.Id == member.UserId);
                if (user != null)
                {
                    var attendance = DataService.Instance.Attendances
                        .FirstOrDefault(a => a.SessionId == _selectedSession.Id && a.UserId == user.Id);

                    var record = new AttendanceRecord
                    {
                        AttendanceId = attendance?.Id ?? Guid.NewGuid().ToString(),
                        SessionId = _selectedSession.Id,
                        UserId = user.Id,
                        UserName = user.FullName,
                        IsPresent = attendance?.IsPresent ?? false,
                        Note = attendance?.Note ?? ""
                    };

                    records.Add(record);
                }
            }

            AttendanceRecords = records;
        }

        private void SaveAttendance(object obj)
        {
            if (_selectedSession == null)
            {
                Message = "Vui lòng chọn buổi tập";
                return;
            }

            try
            {
                foreach (var record in AttendanceRecords)
                {
                    var existing = DataService.Instance.Attendances
                        .FirstOrDefault(a => a.SessionId == record.SessionId && a.UserId == record.UserId);

                    if (existing != null)
                    {
                        existing.IsPresent = record.IsPresent;
                        existing.Note = record.Note;
                    }
                    else
                    {
                        var newAttendance = new Attendance
                        {
                            Id = record.AttendanceId,
                            SessionId = record.SessionId,
                            UserId = record.UserId,
                            IsPresent = record.IsPresent,
                            Note = record.Note,
                            RecordedDate = DateTime.Now
                        };

                        DataService.Instance.Attendances.Add(newAttendance);
                    }
                }

                DataService.Instance.Save();
                Message = "Lưu điểm danh thành công!";
            }
            catch (Exception ex)
            {
                Message = $"Lỗi: {ex.Message}";
            }
        }

        private void MarkAll(bool isPresent)
        {
            if (AttendanceRecords == null || AttendanceRecords.Count == 0)
            {
                Message = "Không có thành viên để điểm danh";
                return;
            }

            foreach (var record in AttendanceRecords)
            {
                record.IsPresent = isPresent;
            }

            Message = isPresent ? "Đã đánh dấu tất cả có mặt" : "Đã đánh dấu tất cả vắng mặt";
        }
    }

    public class AttendanceRecord
    {
        public string AttendanceId { get; set; }
        public string SessionId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public bool IsPresent { get; set; }
        public string Note { get; set; }
    }
}
