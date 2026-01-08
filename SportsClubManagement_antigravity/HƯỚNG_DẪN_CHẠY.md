# Hướng Dẫn Chạy Ứng dụng Quản lý Câu lạc bộ Thể thao trên Visual Studio Code

## 1. Yêu cầu hệ thống

- **OS**: Windows 10 hoặc cao hơn
- **.NET**: .NET 10.0 SDK trở lên
- **Visual Studio Code**: Phiên bản mới nhất
- **Extensions cần thiết**:
  - C# Dev Kit (Microsoft)
  - .NET Install Tool (Microsoft)

## 2. Cài đặt môi trường

### 2.1 Kiểm tra .NET SDK
Mở PowerShell hoặc Command Prompt:
```powershell
dotnet --version
```

Nếu chưa cài đặt, tải từ: https://dotnet.microsoft.com/download

### 2.2 Cài đặt Extensions trên VS Code
1. Mở VS Code
2. Nhấn `Ctrl+Shift+X` để mở Extensions Marketplace
3. Tìm kiếm và cài đặt:
   - `C# Dev Kit`
   - `.NET Install Tool`

## 3. Mở dự án trên VS Code

1. Mở VS Code
2. Nhấn `Ctrl+K Ctrl+O` hoặc `File → Open Folder`
3. Điều hướng đến thư mục:
   ```
   e:\Personal\Group Assignments\Lap_trinh_Windows\Antigravity\SportsClubManagement_antigravity\SportsClubManagement
   ```
4. Nhấn `Select Folder`

## 4. Chạy ứng dụng

### Phương pháp 1: Sử dụng Terminal (Khuyến nghị)

1. Mở Terminal trong VS Code (`Ctrl + ~` hoặc `View → Terminal`)
2. Chạy lệnh:
   ```powershell
   dotnet run
   ```
3. Ứng dụng sẽ khởi động và hiển thị LoginView

### Phương pháp 2: Sử dụng Task Runner

1. Nhấn `Ctrl+Shift+D` để mở Run and Debug panel
2. Chọn `.NET 5+ Console` từ dropdown
3. Nhấn nút `Run` (hoặc `F5`)

### Phương pháp 3: Build rồi chạy executable

1. Mở Terminal và chạy:
   ```powershell
   dotnet build --configuration Release
   ```
2. Executable sẽ nằm ở:
   ```
   bin\Release\net10.0-windows\SportsClubManagement.exe
   ```
3. Double-click vào file `.exe` để chạy

## 5. Thông tin đăng nhập (Demo)

Ứng dụng đã được seed với dữ liệu demo. Sử dụng tài khoản sau để đăng nhập:

### Tài khoản Admin:
- **Tên đăng nhập**: `admin`
- **Mật khẩu**: `admin123`
- **Quyền**: Quản trị hệ thống, quản lý người dùng

### Tài khoản Người dùng:
- **Username**: `user1` | **Password**: `user123`
- **Username**: `user2` | **Password**: `user123`

## 6. Cấu trúc dữ liệu

### Data Persistence (JSON)
- Dữ liệu được lưu trữ trong file `data.json` tại thư mục chạy ứng dụng
- File được tự động tạo khi ứng dụng chạy lần đầu tiên
- Tất cả thay đổi được tự động lưu

### Dữ liệu Demo được tạo bao gồm:
- **3 Users**: admin, user1, user2
- **1 Team**: "Bóng đá Thanh niên"
- **1 Subject**: "Bóng đá 5 người"
- **1 Session**: "Tập luyện tuần 1"
- **1 Notification**: Thông báo hệ thống

## 7. Chức năng chính

### Màn hình Đăng nhập
- Nhập Username/Password
- Tích vào "Remember me" để lưu trạng thái
- Gợi ý demo credentials

### Dashboard (Trang chủ)
- Thống kê tổng số đội
- Số buổi tập sắp tới
- Tổng số thành viên
- Quick actions

### Quản lý Hồ sơ
- Cập nhật thông tin cá nhân
- Tải ảnh đại diện
- Cập nhật thông tin liên hệ

### Quản lý Đội
- Xem danh sách các đội của bạn
- Chi tiết đội gồm:
  - **Thành viên**: Danh sách, vai trò, ngày tham gia
  - **Môn thi đấu**: Thêm/xóa các môn
  - **Buổi tập**: Lịch tập, chi tiết buổi tập
  - **Điểm danh**: Ghi nhận chuyên cần
  - **Quỹ đội**: Quản lý tài chính, ghi có/nợ
  - **Thông báo**: Tin tức đội

### Quản lý Người dùng (Admin only)
- Xem toàn bộ người dùng hệ thống
- Reset mật khẩu người dùng
- Quản lý vai trò

## 8. Tính năng nâng cao

### MVVM Architecture
- Model-View-ViewModel pattern
- Two-way data binding
- Reactive UI updates

### Role-Based Access Control
- Admin: Truy cập toàn bộ chức năng
- User: Các chức năng cơ bản

### Data Persistence
- JSON-based offline storage
- Tự động lưu các thay đổi
- Không cần kết nối internet

## 9. Troubleshooting

### Lỗi: ".NET 10.0 SDK not found"
**Giải pháp**: 
```powershell
dotnet sdk check
dotnet --list-runtimes
```
Tải .NET SDK từ: https://dotnet.microsoft.com/download

### Lỗi: "Port already in use"
Nếu sử dụng phương pháp web:
```powershell
netstat -ano | findstr :5000
taskkill /PID <PID> /F
```

### Ứng dụng không khởi động LoginView
1. Xóa file `data.json` nếu tồn tại
2. Chạy lại: `dotnet run`

### Dữ liệu bị mất
1. Kiểm tra file `data.json` trong thư mục chạy
2. Restart ứng dụng để seed dữ liệu demo lại

## 10. Phát triển thêm

### Thêm Views mới
1. Tạo file `YourView.xaml` trong `Views/`
2. Tạo `YourViewModel.cs` trong `ViewModels/`
3. Thêm DataTemplate trong `MainWindow.xaml`:
   ```xaml
   <DataTemplate DataType="{x:Type vm:YourViewModel}">
       <v:YourView />
   </DataTemplate>
   ```

### Thêm Models mới
1. Khai báo class trong `Models/DomainModels.cs` hoặc file riêng
2. Thêm ObservableCollection<YourModel> vào `DataService.cs`
3. Thêm serialize/deserialize logic

### Build Debug vs Release
```powershell
# Debug (với debugging symbols)
dotnet run

# Release (optimized)
dotnet run --configuration Release
```

## 11. Liên hệ & Hỗ trợ

- **Phiên bản**: 1.0
- **Framework**: WPF (.NET 10.0-windows)
- **Architecture**: MVVM
- **Data Storage**: JSON (offline-first)

---

**Chúc bạn sử dụng ứng dụng vui vẻ! 🎉**
