# Sports Club Management - Ứng dụng Quản lý Câu lạc bộ Thể thao

![.NET Version](https://img.shields.io/badge/.NET-10.0-blue)
![Platform](https://img.shields.io/badge/Platform-Windows-blue)
![Architecture](https://img.shields.io/badge/Architecture-MVVM-green)
![License](https://img.shields.io/badge/License-MIT-green)

## 📋 Mô tả

Ứng dụng desktop WPF dành cho quản lý các câu lạc bộ thể thao, cung cấp các chức năng toàn diện từ quản lý thành viên, lịch tập, quỹ đội cho đến thông báo hệ thống.

**Tính năng chính:**
- ✅ Quản lý thành viên đội
- ✅ Lịch tập và buổi tập
- ✅ Quản lý tài chính (Quỹ đội)
- ✅ Điểm danh tự động
- ✅ Thông báo đội
- ✅ Quản lý người dùng (Admin)
- ✅ Hồ sơ cá nhân
- ✅ Lưu trữ dữ liệu JSON (Offline-first)

## 🚀 Quick Start

### Yêu cầu
- Windows 10 hoặc cao hơn
- .NET 10.0 SDK trở lên
- Visual Studio Code (khuyến nghị)

### Chạy ứng dụng
```bash
cd SportsClubManagement
dotnet run
```

### Tài khoản Demo
- **Admin**: `admin` / `admin123`
- **User**: `user1` / `user123` hoặc `user2` / `user123`

## 📁 Cấu trúc Dự án

```
SportsClubManagement/
├── Models/
│   ├── DomainModels.cs       # User, Team, Subject, Session, etc.
│   └── Team.cs               # Team model
├── Services/
│   └── DataService.cs        # Singleton data access layer
├── ViewModels/
│   ├── LoginViewModel.cs     # Authentication
│   ├── MainViewModel.cs      # Master shell & navigation
│   ├── DashboardViewModel.cs # Welcome & statistics
│   ├── ProfileViewModel.cs   # User profile CRUD
│   ├── TeamsViewModel.cs     # Teams list
│   ├── TeamDetailViewModel.cs# Team master detail
│   ├── TeamMembersViewModel.cs
│   ├── TeamSubjectsViewModel.cs
│   ├── TeamSessionsViewModel.cs
│   ├── TeamFundsViewModel.cs
│   └── UserManagementViewModel.cs # Admin panel
├── Views/
│   ├── LoginView.xaml        # Login screen
│   ├── MainWindow.xaml       # Application shell
│   ├── DashboardView.xaml    # Dashboard
│   ├── ProfileView.xaml      # User profile
│   ├── TeamsView.xaml        # Teams list
│   ├── TeamDetailView.xaml   # Team details (6 tabs)
│   └── UserManagementView.xaml
├── Helpers/
│   ├── ViewModelBase.cs      # MVVM base with INotifyPropertyChanged
│   └── RelayCommand.cs       # ICommand implementation
├── App.xaml & App.xaml.cs    # Application entry point
├── MainWindow.xaml           # Master window
└── data.json                 # Data persistence (auto-created)
```

## 🏗️ Architecture

### MVVM Pattern
```
View (XAML) ←→ ViewModel ←→ Model (Business Logic)
```

### Data Binding
- Two-way binding for form controls
- Command binding for button actions
- PropertyChanged notifications

### Singleton DataService
- Centralized data access
- JSON persistence
- Automatic seed data on first run

## 🎨 UI Components

### Layouts
- **Sidebar Navigation**: Quick access to main views
- **Master Detail**: Team management with 6 tabs
- **Responsive Grid**: Adaptive layouts

### Controls Used
- DataGrid: Team members, subjects, sessions, transactions
- TabControl: Team detail organization
- DatePicker: Session scheduling
- ComboBox: Filtering and selection
- PasswordBox: Secure password input
- Image: User avatars (circular styling)

## 💾 Data Models

### Core Entities
1. **User** - Application users with roles
   - Id, Username, Password, FullName, Email, Role, AvatarPath, BirthDate

2. **Team** - Sports clubs
   - Id, Name, Description, AvatarPath, Balance, CreatedDate

3. **TeamMember** - Member association
   - Id, UserId, TeamId, Role, JoinDate

4. **Subject** - Sports disciplines/events
   - Id, TeamId, Name, Description, ParticipantCount

5. **Session** - Training sessions
   - Id, TeamId, SubjectId, Name, StartTime, EndTime, Note

6. **Attendance** - Training participation
   - Id, SessionId, UserId, IsPresent, Note

7. **FundTransaction** - Financial records
   - Id, TeamId, Date, Amount, Description, ByUserId, Type

8. **Notification** - System notifications
   - Id, TeamId, ByUserId, Content, CreatedDate, IsSystemNotification

## 🔐 Security Features

- Role-based access control (Admin/User)
- Password-based authentication (demo only - should be hashed in production)
- Admin panel with password reset capability

## 📊 Key Features

### Dashboard
- Welcome message with user's full name
- Total teams count
- Upcoming sessions count
- Total team members count
- Quick action buttons

### Profile Management
- View/edit user information
- Avatar upload support
- Birth date picker
- Email validation
- Real-time save

### Team Management
- View user's teams
- Member roles display
- Master detail view with 6 tabs:
  1. **Members**: Team roster with search/filter
  2. **Subjects**: Sports disciplines management
  3. **Sessions**: Training schedule
  4. **Attendance**: Participation tracking
  5. **Notifications**: Team announcements
  6. **Funds**: Financial management (deposits/withdrawals)

### Admin Features
- User management dashboard
- Password reset functionality
- User list with roles and contact info

## 🔄 Data Persistence

### JSON Storage
- Offline-first approach
- Auto-save on modifications
- Human-readable format
- Located in application directory

### Seed Data
- 3 demo users (admin, user1, user2)
- 1 sample team
- 1 subject
- 1 session
- 1 notification

## 🛠️ Technologies Used

- **Language**: C# (.NET 10.0)
- **UI Framework**: WPF (Windows Presentation Foundation)
- **Architecture**: MVVM (Model-View-ViewModel)
- **Data Format**: JSON (System.Text.Json)
- **Binding**: Two-way data binding with INotifyPropertyChanged
- **Commands**: RelayCommand pattern

## 📝 Configuration

### App.xaml.cs
- DataService initialization
- Seed data creation
- LoginView as startup view

### XAML Resources
- Color schemes (Blue: #1976D2, Light Gray: #F5F5F5)
- Button styles
- Border styling
- Font definitions

## 🚧 Future Enhancements

- [ ] Database integration (SQL Server/SQLite)
- [ ] Authentication service
- [ ] Image upload to cloud storage
- [ ] Export to CSV/PDF
- [ ] Email notifications
- [ ] Mobile companion app
- [ ] Real-time sync
- [ ] Backup/restore functionality

## ⚠️ Known Limitations

1. **Password Security**: Passwords stored in plain text (for demo only)
2. **Data Validation**: Basic validation only
3. **Offline Only**: Requires local JSON file
4. **Single User Mode**: One user per session
5. **No Concurrent Access**: Not designed for multi-user simultaneous access

## 🐛 Troubleshooting

### Application won't start
```bash
# Delete data file and restart
rm data.json
dotnet run
```

### Port conflicts (if using web version in future)
```powershell
netstat -ano | findstr :5000
taskkill /PID <PID> /F
```

### Missing .NET SDK
```bash
dotnet sdk check
# Install from: https://dotnet.microsoft.com/download
```

## 📚 Developer Notes

### Adding New Views
1. Create `YourView.xaml` in Views folder
2. Create `YourViewModel.cs` in ViewModels folder
3. Add DataTemplate to MainWindow.xaml
4. Add navigation command to MainViewModel

### Modifying Data Models
1. Update entity class in Models folder
2. Update DataService collections
3. Update Load/Save methods
4. Recreate data.json for schema changes

## 📄 License

MIT License - Free to use and modify

## 👥 Contributing

Contributions welcome! Please:
1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## 📞 Support

For issues, questions, or suggestions:
- Check the HƯỚNG_DẪN_CHẠY.md file
- Review the code documentation
- Examine the demo data structure

---

**Version**: 1.0.0  
**Last Updated**: January 2026  
**Author**: Group Assignment
