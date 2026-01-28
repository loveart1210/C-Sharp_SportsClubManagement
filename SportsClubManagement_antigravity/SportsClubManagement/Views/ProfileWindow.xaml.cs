using System.Windows;
using SportsClubManagement.ViewModels;

namespace SportsClubManagement.Views
{
    public partial class ProfileWindow : Window
    {
        public ProfileWindow()
        {
            InitializeComponent();
        }

        public ProfileWindow(string userId) : this()
        {
            DataContext = new ProfileViewModel(userId);
        }
    }
}
