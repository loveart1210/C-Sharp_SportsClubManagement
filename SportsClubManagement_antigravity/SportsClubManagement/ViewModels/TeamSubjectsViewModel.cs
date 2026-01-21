using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using SportsClubManagement.Helpers;
using SportsClubManagement.Models;
using SportsClubManagement.Services;

namespace SportsClubManagement.ViewModels
{
    public class TeamSubjectsViewModel : ViewModelBase
    {
        private Team? _team;
        private ObservableCollection<Subject> _subjects = new ObservableCollection<Subject>();
        private Subject? _selectedSubject;
        private string _newSubjectName = string.Empty;
        private string _newSubjectDesc = string.Empty;
        private string _searchText = string.Empty;
        private bool _isEditing;
        private Subject? _editingSubject;

        private ObservableCollection<Subject> _allSubjects = new ObservableCollection<Subject>();

        public ObservableCollection<Subject> Subjects
        {
            get => _subjects;
            set => SetProperty(ref _subjects, value);
        }

        public Subject? SelectedSubject
        {
            get => _selectedSubject;
            set => SetProperty(ref _selectedSubject, value);
        }

        public string NewSubjectName
        {
            get => _newSubjectName;
            set => SetProperty(ref _newSubjectName, value);
        }

        public string NewSubjectDesc
        {
            get => _newSubjectDesc;
            set => SetProperty(ref _newSubjectDesc, value);
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

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                SetProperty(ref _isEditing, value);
                OnPropertyChanged(nameof(FormTitle));
                OnPropertyChanged(nameof(ButtonText));
            }
        }

        public string FormTitle => IsEditing ? "Cập nhật môn tập" : "Thêm môn tập mới";
        public string ButtonText => IsEditing ? "Lưu thay đổi" : "Thêm Môn Tập";

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

        public ICommand AddSubjectCommand { get; }
        public ICommand RemoveSubjectCommand { get; }
        public ICommand EditSubjectCommand { get; }
        public ICommand CancelEditCommand { get; }

        public TeamSubjectsViewModel(Team? team = null)
        {
            _team = team;
            if (_team != null)
            {
                LoadSubjects();
            }
            AddSubjectCommand = new RelayCommand(AddSubject, CanAddSubject);
            RemoveSubjectCommand = new RelayCommand(RemoveSubject, CanRemoveSubject);
            EditSubjectCommand = new RelayCommand(EditSubject, CanEditSubject);
            CancelEditCommand = new RelayCommand(CancelEdit);
        }

        public void RefreshData()
        {
            LoadSubjects();
        }

        private void LoadSubjects()
        {
            if (_team == null) return;
            var list = DataService.Instance.Subjects.Where(s => s.TeamId == _team.Id).ToList();
            _allSubjects = new ObservableCollection<Subject>(list);
            
            ApplyFilters();
            OnPropertyChanged(nameof(IsFounderOrCoach));
        }

        private void ApplyFilters()
        {
            if (_allSubjects == null) return;

            var filtered = _allSubjects.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(s => s.Name.ToLower().Contains(SearchText.ToLower()) || 
                                             s.Description?.ToLower().Contains(SearchText.ToLower()) == true);
            }

            Subjects = new ObservableCollection<Subject>(filtered.ToList());
        }

        private bool CanAddSubject(object? obj)
        {
            return IsFounderOrCoach && !string.IsNullOrWhiteSpace(NewSubjectName);
        }

        private void AddSubject(object? obj)
        {
            if (IsEditing && _editingSubject != null)
            {
                // Update
                _editingSubject.Name = NewSubjectName;
                _editingSubject.Description = NewSubjectDesc;
                
                // Also update in DataService (reference is same, so just Save)
                var dsSubject = DataService.Instance.Subjects.FirstOrDefault(s => s.Id == _editingSubject.Id);
                if (dsSubject != null)
                {
                    dsSubject.Name = NewSubjectName;
                    dsSubject.Description = NewSubjectDesc;
                }
                
                DataService.Instance.Save();
                
                IsEditing = false;
                _editingSubject = null;
            }
            else
            {
                var currentUser = DataService.Instance.CurrentUser;
                if (currentUser != null && _team != null)
                {
                    // Add New
                    var newSub = new Subject
                    {
                        TeamId = _team.Id,
                        Name = NewSubjectName,
                        Description = NewSubjectDesc,
                        UserId = currentUser.Id,
                        CreatedDate = DateTime.Now
                    };
                    DataService.Instance.Subjects.Add(newSub);
                    DataService.Instance.Save();
                }
            }
            
            NewSubjectName = string.Empty;
            NewSubjectDesc = string.Empty;
            LoadSubjects();
        }

        private bool CanEditSubject(object? obj) => IsFounderOrCoach;

        private void EditSubject(object? obj)
        {
            if (obj is Subject s)
            {
                _editingSubject = s;
                NewSubjectName = s.Name;
                NewSubjectDesc = s.Description;
                IsEditing = true;
            }
        }

        private void CancelEdit(object? obj)
        {
            IsEditing = false;
            _editingSubject = null;
            NewSubjectName = string.Empty;
            NewSubjectDesc = string.Empty;
        }

        private bool CanRemoveSubject(object? obj) => IsFounderOrCoach;

        private void RemoveSubject(object? parameter)
        {
            if (parameter is Subject sub)
            {
                var result = System.Windows.MessageBox.Show($"Bạn có chắc chắn muốn xóa môn tập '{sub.Name}'?", "Xác nhận", 
                    System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);
                
                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    DataService.Instance.Subjects.Remove(sub);
                    DataService.Instance.Save();
                    LoadSubjects();
                }
            }
        }
    }
}
