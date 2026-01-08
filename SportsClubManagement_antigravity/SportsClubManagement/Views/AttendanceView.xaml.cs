using System.Windows.Controls;
using SportsClubManagement.ViewModels;

namespace SportsClubManagement.Views
{
    public partial class AttendanceView : UserControl
    {
        public AttendanceView()
        {
            InitializeComponent();
            DataContext = new AttendanceViewModel();
        }
    }
}
