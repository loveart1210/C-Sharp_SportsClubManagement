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
        private ObservableCollection<TeamDisplayModel> _teams;
        private ObservableCollection<TeamDisplayModel> _allTeams;
        private TeamDisplayModel _selectedTeam;
        private string _searchText;

        public ObservableCollection<TeamDisplayModel> Teams
        {
            get => _teams;
            set => SetProperty(ref _teams, value);
        }

        public TeamDisplayModel SelectedTeam
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

        public ICommand ViewTeamCommand { get; }

        public TeamsViewModel()
        {
            ViewTeamCommand = new RelayCommand(ViewTeam, CanViewTeam);
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

        private bool CanViewTeam(object parameter)
        {
            return SelectedTeam != null;
        }

        private void ViewTeam(object parameter)
        {
            if (SelectedTeam?.Team != null)
            {
                // This would normally navigate; for now we can raise an event
                OnTeamSelected?.Invoke(this, SelectedTeam.Team);
            }
        }

        public event EventHandler<Team> OnTeamSelected;
    }

    public class TeamDisplayModel
    {
        public Team Team { get; set; }
        public string MemberRole { get; set; }
        public int MemberCount { get; set; }
    }
}
