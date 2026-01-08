using System;
using System.Configuration;
using System.Data;
using System.Windows;
using SportsClubManagement.Services;
using SportsClubManagement.Views;

namespace SportsClubManagement;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        try
        {
            base.OnStartup(e);
            
            // Initialize DataService first
            var dataService = DataService.Instance;
            
            // Set LoginView as the main window
            LoginView loginView = new LoginView();
            this.MainWindow = loginView;
            loginView.Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khởi động ứng dụng:\n\n{ex.GetType().Name}: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}", 
                           "Lỗi Khởi Động", 
                           MessageBoxButton.OK, 
                           MessageBoxImage.Error);
            this.Shutdown(1);
        }
    }

    private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        MessageBox.Show($"Lỗi không xử lý được:\n\n{e.Exception.GetType().Name}: {e.Exception.Message}\n\nStack Trace:\n{e.Exception.StackTrace}", 
                       "Lỗi Ứng dụng", 
                       MessageBoxButton.OK, 
                       MessageBoxImage.Error);
        e.Handled = true;
    }
}
