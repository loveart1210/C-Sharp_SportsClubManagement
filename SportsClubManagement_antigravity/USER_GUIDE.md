# Sports Club Management - Feature Usage Guide

## 🎯 User Features Guide

### **1. Login**
```
Username: admin
Password: 123456
Role: Admin (can manage all teams and users)

OR

Username: user1
Password: 123
Role: User (member of teams)
```

---

### **2. Team Management**
**View Teams**
- Dashboard shows all teams you're a member of
- Search teams by name or description
- View member count and your role in each team

**Join/Create Team** (Admin only)
- Create new team with name, description
- Set initial balance for team fund
- Invite members by email

---

### **3. Attendance Tracking** (NEW)
**Location**: Team Detail → Attendance Tab

1. **Select Date**: Pick training date from calendar
2. **Choose Session**: Dropdown shows all sessions for that date
3. **Mark Attendance**:
   - Click checkbox next to member name to mark present
   - Add notes in the Notes column if needed
4. **Bulk Operations**:
   - "Mark All Present" button
   - "Mark All Absent" button
5. **Save**: Click Save Attendance button to persist

**Database Storage**: Saves to Attendances table with SessionId + UserId unique constraint

---

### **4. Search & Filter Users**
**Location**: User Management Tab (Admin only)

```
Search by:
- Full name (text contains)
- Email address

Filter by:
- Role: Admin, User, All

Result: Live-filtered user list
```

**Example**: Search "john" + Role="User" → Shows all Users named john

---

### **5. Search & Filter Team Members**
**Location**: Team Detail → Members Tab

```
Search by:
- Member name
- Email

Filter by:
- Team role: Founder, Admin, Coach, Member
- (Subject filter ready for enhancement)

Result: Filtered member list
```

---

### **6. Export Functionality** (NEW)

#### **Export Schedule to CSV**
**Location**: Team Detail → Sessions Tab → "Export Schedule" button

**Output File**: Desktop/SportsClubManagement_Exports/LichTap_{TeamName}_{Timestamp}.csv

**Contains**:
```
Buổi tập,Môn tập,Thời gian bắt đầu,Thời gian kết thúc,Ghi chú
"Buổi 1","Bóng Đá",22/12/2024 08:00,22/12/2024 10:00,"Buổi khởi động"
"Buổi 2","Bóng Đá",23/12/2024 08:00,23/12/2024 10:00,""
```

#### **Export Financial Report to CSV**
**Location**: Team Detail → Funds Tab → "Export" button

**Output File**: Desktop/SportsClubManagement_Exports/BaoCaoTaiChinh_{TeamName}_{Timestamp}.csv

**Contains**:
```
Ngày,Loại,Số tiền,Mô tả,Người thực hiện
23/12/2024 10:00,Deposit,"500,000 VNĐ","Quỹ tập luyện","Nguyễn Văn A"
24/12/2024 14:30,Withdraw,"200,000 VNĐ","Mua bóng","Trần Thị B"
```

**Auto-Notification**: System creates notification entry after export

#### **Export Members List to CSV**
**Method**: ExportService.ExportMembersToCSV(team)

**Contains**:
```
Tên,Email,Vai trò,Ngày tham gia
"Nguyễn Văn A",nguyenA@email.com,Founder,22/12/2024
"Trần Thị B",tranB@email.com,Coach,23/12/2024
```

---

### **7. User Profile & Password Management** (NEW)
**Location**: User Menu → Profile

**Edit Profile**:
- Change full name
- Change email address
- View profile avatar
- See teams you're in

**Change Password**:
1. Click "Change Password" button
2. Enter current password
3. Enter new password (minimum 6 characters)
4. Confirm password
5. Click "Change Password"

**Validation**:
- ✓ Current password must be correct
- ✓ New password must match confirmation
- ✓ Minimum 6 characters required

---

### **8. User Management** (Admin Only)
**Location**: Admin Panel → User Management

**Features**:
- Search users by name/email
- Filter by role (Admin, User)
- Reset user password to "123"
- Delete user accounts

**Reset Password**:
1. Select user
2. Click "Reset Password"
3. Confirm action
4. User can now login with password "123"

**Delete User**:
1. Select user
2. Click "Delete"
3. Confirm (cannot delete yourself)

---

### **9. Team Finance Management**
**Location**: Team Detail → Funds Tab

**Deposit Money**:
1. Enter amount
2. Enter description (e.g., "Monthly dues", "Sponsor donation")
3. Click "Deposit"

**Withdraw Money**:
1. Enter amount
2. Enter description
3. Click "Withdraw"
4. (System checks sufficient balance)

**View Balance**:
- Current balance displayed at top
- All transaction history in table
- See who performed each transaction
- Export as CSV anytime

---

### **10. Team Session Management**
**Location**: Team Detail → Sessions Tab

**Create Session**:
1. Select date from calendar
2. Enter session name (e.g., "Buổi luyện tập sáng")
3. Choose subject (subject must exist)
4. Set start and end times
5. Add notes (optional)
6. Click "Add Session"

**View Sessions**:
- Filter by date
- See all sessions for selected date
- View subject name and times

**Delete Session**:
- Click session
- Click "Remove"
- Confirms deletion

**Export Schedule**:
- Click "Export Schedule"
- Creates CSV file on Desktop

---

## 💾 DATA PERSISTENCE

### **Automatic Backup Strategy**
- **Primary**: MySQL database (if available)
- **Fallback**: JSON files in application directory

### **Data Sync**
```csharp
DataService.Instance.Save()
  → If MySQL connected: SaveToDatabase()
  → Else: SaveToJson()
  
DataService.Instance.Load()
  → If MySQL connected: LoadFromDatabase()
  → Else: LoadFromJson()
```

---

## 🔒 Role-Based Access

### **Admin Role**
- ✅ Create teams
- ✅ Manage all users (reset password, delete)
- ✅ View all team data
- ✅ Manage finances
- ✅ Export all data

### **Team Founder**
- ✅ Manage team members
- ✅ Manage team finances
- ✅ Create sessions
- ✅ Track attendance
- ✅ Export team data

### **Coach Role**
- ✅ Create sessions
- ✅ Track attendance
- ✅ View team members

### **Regular Member**
- ✓ View team data
- ✓ See own attendance

---

## 🐛 Troubleshooting

### **Export files not saving**
- Check if Desktop/SportsClubManagement_Exports/ folder exists
- Application auto-creates it on first export
- Verify write permissions to Desktop

### **Attendance not showing members**
- Ensure members are added to team first
- Session must be created for the selected date
- Refresh by re-selecting date

### **Password change fails**
- Verify current password is correct
- New password must be at least 6 characters
- Confirmation must match exactly

### **Database connection failed**
- Application auto-falls back to JSON
- Ensure MySQL is running if using database mode
- Check connection string in DataService

---

## 📚 Architecture Notes

### **MVVM Pattern**
- Views: XAML files bind to ViewModels
- ViewModels: Handle UI logic and commands
- Models: Domain entities and DTOs
- Services: Data access and business logic

### **Data Binding**
```xaml
<TextBox Text="{Binding SearchText, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"/>
<DataGrid ItemsSource="{Binding FilteredItems}"/>
<Button Command="{Binding SaveCommand}"/>
```

### **Command Pattern**
```csharp
public ICommand ExportCommand { get; }
public ProfileViewModel()
{
    ExportCommand = new RelayCommand(Export);
}
private void Export(object obj) { ... }
```

---

**Last Updated**: December 2024
**Version**: 1.0
**Framework**: WPF .NET 10.0
