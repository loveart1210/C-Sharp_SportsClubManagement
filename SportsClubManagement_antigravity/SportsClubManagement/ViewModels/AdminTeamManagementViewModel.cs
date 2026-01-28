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
    public class AdminTeamManagementViewModel : ViewModelBase
    {
        private ObservableCollection<TeamItemViewModel> _teams = new ObservableCollection<TeamItemViewModel>();
        private string _searchText = string.Empty;
        private ObservableCollection<TeamItemViewModel> _allTeams = new ObservableCollection<TeamItemViewModel>();

        public ObservableCollection<TeamItemViewModel> Teams
        {
            get => _teams;
            set => SetProperty(ref _teams, value);
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

        public ICommand DeleteTeamCommand { get; }

        public AdminTeamManagementViewModel()
        {
            LoadTeams();
            DeleteTeamCommand = new RelayCommand(DeleteTeam);
        }

        private void LoadTeams()
        {
            var teams = DataService.Instance.Teams.Select(t => new TeamItemViewModel
            {
                Team = t,
                MemberCount = DataService.Instance.TeamMembers.Count(tm => tm.TeamId == t.Id),
                FounderName = DataService.Instance.Users.FirstOrDefault(u => u.Id == DataService.Instance.TeamMembers.FirstOrDefault(tm => tm.TeamId == t.Id && tm.Role == "Founder")?.UserId)?.FullName ?? "N/A"
            }).ToList();

            _allTeams = new ObservableCollection<TeamItemViewModel>(teams);
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var filtered = _allTeams.AsEnumerable();
            
            if (!string.IsNullOrWhiteSpace(SearchText))
                filtered = filtered.Where(t => t.Team.Name.ToLower().Contains(SearchText.ToLower()) || 
                                              t.Team.Description.ToLower().Contains(SearchText.ToLower()) ||
                                              t.FounderName.ToLower().Contains(SearchText.ToLower()));
            
            Teams = new ObservableCollection<TeamItemViewModel>(filtered.ToList());
        }

        private void DeleteTeam(object? obj)
        {
            if (obj is TeamItemViewModel teamVM)
            {
                var result = MessageBox.Show($"Bạn có chắc muốn xóa Team '{teamVM.Team.Name}'? Hành động này sẽ xóa toàn bộ dữ liệu liên quan (thành viên, môn tập, buổi tập...).", 
                                           "Xác nhận Xóa Team", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                
                if (result == MessageBoxResult.Yes)
                {
                    var team = teamVM.Team;
                    
                    // Remove all related data
                    DataService.Instance.TeamMembers.RemoveAll(tm => tm.TeamId == team.Id);
                    DataService.Instance.Subjects.RemoveAll(s => s.TeamId == team.Id);
                    DataService.Instance.Sessions.RemoveAll(s => s.TeamId == team.Id);
                    DataService.Instance.Notifications.RemoveAll(n => n.TeamId == team.Id);
                    DataService.Instance.Transactions.RemoveAll(t => t.TeamId == team.Id);
                    // Attendance records are linked via sessions, but if we delete by teamId we need to check if they have it
                    DataService.Instance.Attendances.RemoveAll(a => DataService.Instance.Sessions.Any(s => s.Id == a.SessionId && s.TeamId == team.Id));
                    
                    DataService.Instance.Teams.Remove(team);
                    DataService.Instance.Save();
                    
                    LoadTeams();
                }
            }
        }
    }

    public class TeamItemViewModel
    {
        public Team Team { get; set; } = new Team();
        public int MemberCount { get; set; }
        public string FounderName { get; set; } = string.Empty;
    }
}
