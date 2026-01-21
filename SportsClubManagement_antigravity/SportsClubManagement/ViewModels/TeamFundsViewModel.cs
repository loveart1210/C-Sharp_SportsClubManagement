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
        private Team? _team;
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
            set => SetProperty(ref _isDeposit, value);
        }

        public ICommand DepositCommand { get; }
        public ICommand WithdrawCommand { get; }
        public ICommand ExportCommand { get; }

        public TeamFundsViewModel(Team? team = null)
        {
            _team = team;
            if (_team != null)
            {
                LoadFundData();
            }
            DepositCommand = new RelayCommand(Deposit, CanTransaction);
            WithdrawCommand = new RelayCommand(Withdraw, CanTransaction);
            ExportCommand = new RelayCommand(Export);
        }

        private void LoadFundData()
        {
            if (_team == null) return;
            var transactions = DataService.Instance.Transactions
                .Where(t => t.TeamId == _team.Id)
                .ToList();
            
            Transactions = new ObservableCollection<FundTransaction>(transactions);
            TotalBalance = _team.Balance;
        }

        private bool CanTransaction(object? parameter)
        {
            return Amount > 0 && !string.IsNullOrWhiteSpace(Description);
        }

        private void Deposit(object? obj)
        {
            if (CanTransaction(obj) && _team != null)
            {
                var transaction = new FundTransaction
                {
                    TeamId = _team.Id,
                    Amount = Amount,
                    Description = Description,
                    Type = "Deposit",
                    ByUserId = DataService.Instance.CurrentUser?.Id ?? ""
                };
                
                DataService.Instance.Transactions.Add(transaction);
                _team.Balance += Amount;
                DataService.Instance.Save();
                
                Transactions.Add(transaction);
                TotalBalance = _team.Balance;
                Amount = 0;
                Description = "";
            }
        }

        private void Withdraw(object? obj)
        {
            if (CanTransaction(obj) && _team != null)
            {
                if (_team.Balance >= Amount)
                {
                    var transaction = new FundTransaction
                    {
                        TeamId = _team.Id,
                        Amount = -Amount,
                        Description = Description,
                        Type = "Withdraw",
                        ByUserId = DataService.Instance.CurrentUser?.Id ?? ""
                    };
                    
                    DataService.Instance.Transactions.Add(transaction);
                    _team.Balance -= Amount;
                    DataService.Instance.Save();
                    
                    Transactions.Add(transaction);
                    TotalBalance = _team.Balance;
                    Amount = 0;
                    Description = "";
                }
            }
        }

        private void Export(object? obj)
        {
            if (_team != null)
            {
                var filePath = ExportService.ExportFinancialReportToCSV(_team);
                
                // Create notification
                var notification = new Notification
                {
                    TeamId = _team.Id,
                    ByUserId = DataService.Instance.CurrentUser?.Id ?? "",
                    Title = "Xuất báo cáo tài chính",
                    Content = $"Báo cáo tài chính đã được xuất tại: {filePath}"
                };
                DataService.Instance.Notifications.Add(notification);
                DataService.Instance.Save();
                
                System.Windows.MessageBox.Show($"Báo cáo đã được xuất tại:\n{filePath}", "Xuất thành công", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
        }
    }
}
