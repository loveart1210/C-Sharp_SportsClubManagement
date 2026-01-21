using System.Windows.Controls;

namespace SportsClubManagement.Views
{
    public partial class TeamDetailView : UserControl
    {
        public TeamDetailView()
        {
            InitializeComponent();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Ensure we only handle the TabControl's selection change, not nested controls
            if (e.Source is TabControl && DataContext is ViewModels.TeamDetailViewModel vm)
            {
                 vm.RefreshCurrentTab();
            }
        }
    }
}
