using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using SportsClubManagement.Helpers;
using SportsClubManagement.Models;
using SportsClubManagement.Services;

namespace SportsClubManagement.ViewModels
{
    public class TeamsViewModel : ViewModelBase
    {
        private ObservableCollection<TeamDisplayModel> _teams = new ObservableCollection<TeamDisplayModel>();
        private ObservableCollection<TeamDisplayModel> _allTeams = new ObservableCollection<TeamDisplayModel>();
        private TeamDisplayModel? _selectedTeam;
        private string _searchText = string.Empty;
        private string _newTeamName = string.Empty;
        private string _newTeamDescription = string.Empty;

        public ObservableCollection<TeamDisplayModel> Teams
        {
            get => _teams;
            set => SetProperty(ref _teams, value);
        }

        public TeamDisplayModel? SelectedTeam
        {
            get => _selectedTeam;
            set => SetProperty(ref _selectedTeam, value);
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

        public string NewTeamName
        {
            get => _newTeamName;
            set => SetProperty(ref _newTeamName, value);
        }

        public string NewTeamDescription
        {
            get => _newTeamDescription;
            set => SetProperty(ref _newTeamDescription, value);
        }

        public ICommand ViewTeamCommand { get; }
        public ICommand CreateTeamCommand { get; }
        public ICommand JoinTeamCommand { get; }

        private string _joinTeamCode = string.Empty;
        public string JoinTeamCode
        {
            get => _joinTeamCode;
            set => SetProperty(ref _joinTeamCode, value);
        }

        public TeamsViewModel()
        {
            ViewTeamCommand = new RelayCommand(ViewTeam, CanViewTeam);
            CreateTeamCommand = new RelayCommand(CreateTeam, CanCreateTeam);
            JoinTeamCommand = new RelayCommand(JoinTeam, CanJoinTeam);
            LoadTeamData();
        }

        private bool CanJoinTeam(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(JoinTeamCode);
        }

        private void JoinTeam(object? parameter)
        {
            var currentUser = DataService.Instance.CurrentUser;
            if (currentUser == null) return;

            // Check if team exists
            var team = DataService.Instance.Teams.FirstOrDefault(t => t.Id == JoinTeamCode || t.Id.StartsWith(JoinTeamCode));
            
            if (team == null)
            {
                System.Windows.MessageBox.Show("Không tìm thấy Team với mã này.", "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }

            // Check if already a member
            if (DataService.Instance.TeamMembers.Any(tm => tm.TeamId == team.Id && tm.UserId == currentUser.Id))
            {
                System.Windows.MessageBox.Show("Bạn đã tham gia Team này rồi.", "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                return;
            }

            // Join team
            var newMember = new TeamMember
            {
                TeamId = team.Id,
                UserId = currentUser.Id,
                Role = "Member",
                JoinDate = DateTime.Now
            };

            DataService.Instance.TeamMembers.Add(newMember);
            DataService.Instance.Save();

            System.Windows.MessageBox.Show($"Bạn đã tham gia Team '{team.Name}' thành công!", "Thành công", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            
            JoinTeamCode = string.Empty;
            LoadTeamData();
        }

        private void LoadTeamData()
        {
            var currentUser = DataService.Instance.CurrentUser;
            if (currentUser == null) return;

            var userTeamIds = DataService.Instance.TeamMembers
                .Where(tm => tm.UserId == currentUser.Id)
                .Select(tm => tm.TeamId)
                .ToList();

            _allTeams = new ObservableCollection<TeamDisplayModel>();

            foreach (var teamId in userTeamIds)
            {
                var team = DataService.Instance.Teams.FirstOrDefault(t => t.Id == teamId);
                var memberRole = DataService.Instance.TeamMembers
                    .FirstOrDefault(tm => tm.UserId == currentUser.Id && tm.TeamId == teamId)?.Role ?? "Member";

                var memberCount = DataService.Instance.TeamMembers
                    .Count(tm => tm.TeamId == teamId);

                if (team != null)
                {
                    _allTeams.Add(new TeamDisplayModel
                    {
                        Team = team,
                        MemberRole = memberRole,
                        MemberCount = memberCount
                    });
                }
            }

            ApplyFilters();
        }

        private void ApplyFilters()
        {
            if (_allTeams == null) return;
            
            var filtered = _allTeams.AsEnumerable();
            
            if (!string.IsNullOrWhiteSpace(SearchText))
                filtered = filtered.Where(t => t.Team.Name.ToLower().Contains(SearchText.ToLower()) ||
                                             t.Team.Description?.ToLower().Contains(SearchText.ToLower()) == true);
            
            Teams = new ObservableCollection<TeamDisplayModel>(filtered.ToList());
        }

        private bool CanViewTeam(object? parameter)
        {
            return parameter is TeamDisplayModel || SelectedTeam != null;
        }

        private void ViewTeam(object? parameter)
        {
            var teamModel = parameter as TeamDisplayModel ?? SelectedTeam;
            if (teamModel?.Team != null)
            {
                // This would normally navigate; for now we can raise an event
                OnTeamSelected?.Invoke(this, teamModel.Team);
            }
        }

        private bool CanCreateTeam(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(NewTeamName);
        }

        private void CreateTeam(object? parameter)
        {
            var currentUser = DataService.Instance.CurrentUser;
            if (currentUser == null) return;

            // Create new team
            var newTeam = new Team
            {
                Name = NewTeamName,
                Description = NewTeamDescription,
                CreatedDate = DateTime.Now,
                Balance = 0
            };
            DataService.Instance.Teams.Add(newTeam);

            // Add current user as Founder
            var teamMember = new TeamMember
            {
                UserId = currentUser.Id,
                TeamId = newTeam.Id,
                Role = "Founder",
                JoinDate = DateTime.Now
            };
            DataService.Instance.TeamMembers.Add(teamMember);

            // Save changes
            DataService.Instance.Save();

            // Clear form
            NewTeamName = string.Empty;
            NewTeamDescription = string.Empty;

            // Reload teams
            LoadTeamData();

            System.Windows.MessageBox.Show($"Team '{newTeam.Name}' đã được tạo thành công!", "Thành công",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        public event EventHandler<Team>? OnTeamSelected;
    }

    public class TeamDisplayModel
    {
        public Team Team { get; set; } = new Team();
        public string MemberRole { get; set; } = "Member";
        public int MemberCount { get; set; }
    }
}
