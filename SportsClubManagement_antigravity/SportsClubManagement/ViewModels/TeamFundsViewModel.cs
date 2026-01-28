using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using SportsClubManagement.Helpers;
using SportsClubManagement.Models;
using SportsClubManagement.Services;

namespace SportsClubManagement.ViewModels
{
    public class TeamFundsViewModel : ViewModelBase
    {
        private readonly Team _team;
        private ObservableCollection<FundTransaction> _transactions = new ObservableCollection<FundTransaction>();
        private decimal _totalBalance;
        private decimal _amount;
        private string _description = string.Empty;
        private bool _isDeposit = true;

        public ObservableCollection<FundTransaction> Transactions
        {
            get => _transactions;
            set => SetProperty(ref _transactions, value);
        }

        public decimal TotalBalance
        {
            get => _totalBalance;
            set => SetProperty(ref _totalBalance, value);
        }

        public decimal Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public bool IsDeposit
        {
            get => _isDeposit;
            set 
            {
                if (SetProperty(ref _isDeposit, value))
                {
                    OnPropertyChanged(nameof(Amount)); // Trigger re-eval of command if needed
                }
            }
        }

        public bool IsFounder => DataService.Instance.CurrentUser != null && 
                               DataService.Instance.TeamMembers.Any(tm => tm.TeamId == _team.Id && 
                                                                        tm.UserId == DataService.Instance.CurrentUser.Id && 
                                                                        tm.Role == "Founder");

        public ICommand DepositCommand { get; }
        public ICommand WithdrawCommand { get; }
        public ICommand ExportCommand { get; }

        public TeamFundsViewModel(Team team)
        {
            _team = team;
            DepositCommand = new RelayCommand(_ => Deposit(), _ => CanTransaction());
            WithdrawCommand = new RelayCommand(_ => Withdraw(), _ => CanTransaction());
            ExportCommand = new RelayCommand(_ => Export(), _ => IsFounder);
            
            RefreshData();
        }

        public void RefreshData()
        {
            var transactions = DataService.Instance.Transactions
                .Where(t => t.TeamId == _team.Id)
                .OrderByDescending(t => t.Date)
                .ToList();
            
            Transactions = new ObservableCollection<FundTransaction>(transactions);
            TotalBalance = _team.Balance;
            OnPropertyChanged(nameof(IsFounder));
        }

        private bool CanTransaction()
        {
            bool basicValidation = IsFounder && Amount > 0 && !string.IsNullOrWhiteSpace(Description);
            if (!basicValidation) return false;

            // If withdrawing, check if balance is sufficient
            if (!IsDeposit && _team.Balance < Amount)
                return false;

            return true;
        }

        private void Deposit()
        {
            var currentUser = DataService.Instance.CurrentUser;
            if (currentUser == null) return;

            var transaction = new FundTransaction
            {
                TeamId = _team.Id,
                Amount = Amount,
                Description = Description,
                Type = "Deposit",
                Date = DateTime.Now,
                ByUserId = currentUser.Id
            };
            
            DataService.Instance.Transactions.Add(transaction);
            _team.Balance += Amount;
            DataService.Instance.Save();
            
            Amount = 0;
            Description = "";
            RefreshData();
        }

        private void Withdraw()
        {
            var currentUser = DataService.Instance.CurrentUser;
            if (currentUser == null) return;

            if (_team.Balance < Amount)
            {
                System.Windows.MessageBox.Show("Số dư không đủ để thực hiện giao dịch này.", "Lỗi", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }

            var transaction = new FundTransaction
            {
                TeamId = _team.Id,
                Amount = -Amount,
                Description = Description,
                Type = "Withdraw",
                Date = DateTime.Now,
                ByUserId = currentUser.Id
            };
            
            DataService.Instance.Transactions.Add(transaction);
            _team.Balance -= Amount;
            DataService.Instance.Save();
            
            Amount = 0;
            Description = "";
            RefreshData();
        }

        private void Export()
        {
            try
            {
                var filePath = ExportService.ExportFinancialReportToCSV(_team);
                
                // Automaticaly post notification
                var notification = new Notification
                {
                    TeamId = _team.Id,
                    ByUserId = DataService.Instance.CurrentUser?.Id ?? "",
                    Title = "Báo cáo tài chính",
                    Content = "Báo cáo tài chính đã được xuất",
                    CreatedDate = DateTime.Now,
                    IsSystemNotification = true
                };
                DataService.Instance.Notifications.Add(notification);
                DataService.Instance.Save();
                
                System.Windows.MessageBox.Show($"Báo cáo tài chính đã được xuất tại:\n{filePath}", "Xuất thành công", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Lỗi khi xuất báo cáo: {ex.Message}", "Lỗi", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
    }
}
