using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using SportsClubManagement.Helpers;
using SportsClubManagement.Models;
using SportsClubManagement.Services;

namespace SportsClubManagement.ViewModels
{
    public class NotificationDisplayItem
    {
        public Notification Notification { get; set; } = new Notification();
        public string AuthorName { get; set; } = string.Empty;
        public bool IsDeletable { get; set; }
    }

    public class TeamNotificationsViewModel : ViewModelBase
    {
        private readonly Team _team;
        private ObservableCollection<NotificationDisplayItem> _notifications = new ObservableCollection<NotificationDisplayItem>();
        private string _newNotificationTitle = string.Empty;
        private string _newNotificationContent = string.Empty;
        private bool _isAddingNotification;

        public ObservableCollection<NotificationDisplayItem> Notifications
        {
            get => _notifications;
            set => SetProperty(ref _notifications, value);
        }

        public string NewNotificationTitle
        {
            get => _newNotificationTitle;
            set => SetProperty(ref _newNotificationTitle, value);
        }

        public string NewNotificationContent
        {
            get => _newNotificationContent;
            set => SetProperty(ref _newNotificationContent, value);
        }

        public bool IsAddingNotification
        {
            get => _isAddingNotification;
            set => SetProperty(ref _isAddingNotification, value);
        }

        public bool CanManageNotifications => DataService.Instance.CurrentUser != null && 
                                             DataService.Instance.CanManageTeam(DataService.Instance.CurrentUser, _team);

        public ICommand RefreshCommand { get; }
        public ICommand ShowAddFormCommand { get; }
        public ICommand CancelAddCommand { get; }
        public ICommand AddNotificationCommand { get; }
        public ICommand DeleteNotificationCommand { get; }

        public TeamNotificationsViewModel(Team team)
        {
            _team = team;
            RefreshCommand = new RelayCommand(_ => RefreshData());
            ShowAddFormCommand = new RelayCommand(_ => IsAddingNotification = true, _ => CanManageNotifications);
            CancelAddCommand = new RelayCommand(_ => 
            {
                IsAddingNotification = false;
                NewNotificationTitle = string.Empty;
                NewNotificationContent = string.Empty;
            });
            AddNotificationCommand = new RelayCommand(_ => AddNotification(), _ => CanAddNotification());
            DeleteNotificationCommand = new RelayCommand(obj => DeleteNotification(obj as NotificationDisplayItem), _ => CanManageNotifications);

            RefreshData();
        }

        public void RefreshData()
        {
            var currentUser = DataService.Instance.CurrentUser;
            var canManage = CanManageNotifications;

            var notifs = DataService.Instance.Notifications
                .Where(n => n.TeamId == _team.Id)
                .OrderByDescending(n => n.CreatedDate)
                .Select(n => new NotificationDisplayItem
                {
                    Notification = n,
                    AuthorName = DataService.Instance.Users.FirstOrDefault(u => u.Id == n.ByUserId)?.FullName ?? "Hệ thống",
                    IsDeletable = canManage
                })
                .ToList();
            Notifications = new ObservableCollection<NotificationDisplayItem>(notifs);
        }

        private bool CanAddNotification()
        {
            return !string.IsNullOrWhiteSpace(NewNotificationTitle) && 
                   !string.IsNullOrWhiteSpace(NewNotificationContent) &&
                   CanManageNotifications;
        }

        private void AddNotification()
        {
            var currentUser = DataService.Instance.CurrentUser;
            if (currentUser == null) return;

            var notification = new Notification
            {
                TeamId = _team.Id,
                ByUserId = currentUser.Id,
                Title = NewNotificationTitle,
                Content = NewNotificationContent,
                CreatedDate = DateTime.Now,
                IsSystemNotification = false
            };

            DataService.Instance.Notifications.Add(notification);
            DataService.Instance.Save();

            IsAddingNotification = false;
            NewNotificationTitle = string.Empty;
            NewNotificationContent = string.Empty;
            
            RefreshData();
        }

        private void DeleteNotification(NotificationDisplayItem? item)
        {
            if (item == null) return;

            var existing = DataService.Instance.Notifications.FirstOrDefault(n => n.Id == item.Notification.Id);
            if (existing != null)
            {
                DataService.Instance.Notifications.Remove(existing);
                DataService.Instance.Save();
                RefreshData();
            }
        }
    }
}
