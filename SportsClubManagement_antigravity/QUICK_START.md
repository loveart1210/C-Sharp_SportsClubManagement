# ⚡ Quick Start Guide - 5 Phút để Chạy Ứng dụng

## Step 1: Mở Terminal (30 giây)
- VS Code: Nhấn `Ctrl + ~` hoặc `View → Terminal`
- Hoặc Command Prompt/PowerShell

## Step 2: Điều hướng đến Project (30 giây)
```powershell
cd "e:\Personal\Group Assignments\Lap_trinh_Windows\Antigravity\SportsClubManagement_antigravity\SportsClubManagement"
```

## Step 3: Chạy Ứng dụng (1 phút)
```powershell
dotnet run
```

✅ **Ứng dụng sẽ khởi động tự động!**

## Step 4: Đăng Nhập (1 phút)

### Tài khoản Admin (Quản trị)
```
Username: admin
Password: admin123
```

### Tài khoản Người dùng
```
Username: user1
Password: user123
```

hoặc

```
Username: user2
Password: user123
```

## ✨ Giao Diện sau khi Đăng Nhập

```
┌──────────────────────────────────────────┐
│  🎉 Ứng dụng Quản lý Câu lạc bộ Thể thao │
│                                          │
│  Xin chào, [Tên người dùng]!             │
│                                          │
│  📊 Dashboard          👥 Quản lý Đội     │
│  👤 Hồ sơ            ⚙️ Quản lý (Admin) │
│                                          │
│  [👤 John Doe] [Logout]                 │
└──────────────────────────────────────────┘
```

## 🎮 Các Chức Năng Chính

### 📊 Dashboard
- Xem thống kê
- Quick stats (Số đội, buổi tập, thành viên)

### 👤 Hồ sơ
- Cập nhật thông tin cá nhân
- Thay đổi ảnh đại diện
- Cập nhật email, ngày sinh

### 👥 Quản lý Đội
- Xem danh sách đội của bạn
- Click "View Detail" để xem chi tiết đội:
  - **Thành viên**: Danh sách + search
  - **Môn thi đấu**: Thêm/xóa môn
  - **Buổi tập**: Xem lịch tập
  - **Điểm danh**: Ghi nhận chuyên cần
  - **Quỹ đội**: Quản lý tài chính
  - **Thông báo**: Tin tức đội

### ⚙️ Quản lý (Admin only)
- Xem tất cả người dùng
- Reset mật khẩu người dùng

## 📁 Cấu Trúc Thư Mục Quan Trọng

```
SportsClubManagement/
├── Views/              👈 Giao diện (XAML)
├── ViewModels/         👈 Logic xử lý
├── Models/             👈 Dữ liệu
├── Services/           👈 DataService
├── data.json           👈 Dữ liệu (auto-tạo)
└── bin/Debug/          👈 Executable
    └── net10.0-windows/
        └── SportsClubManagement.exe
```

## 🔄 Vòng đời Dữ liệu

```
1. Ứng dụng khởi động
   ↓
2. DataService load data từ data.json
   ↓
3. Nếu data.json không tồn tại → Tạo dữ liệu demo
   ↓
4. Hiển thị LoginView
   ↓
5. Người dùng đăng nhập
   ↓
6. Hiển thị MainWindow (với Dashboard, Profile, Teams, etc.)
   ↓
7. Khi thay đổi dữ liệu → DataService.Save() lưu vào data.json
```

## 🆘 Nếu Có Lỗi

### Lỗi: ".NET is not installed"
```powershell
# Kiểm tra phiên bản
dotnet --version

# Nếu chưa cài, tải từ:
# https://dotnet.microsoft.com/download
```

### Lỗi: "Project not found"
```powershell
# Kiểm tra đường dẫn
cd SportsClubManagement
ls  # Xem thư mục

# Phải có file: SportsClubManagement.csproj
```

### Ứng dụng khởi động nhưng không hiển thị
1. Đóng tất cả cửa sổ VS Code
2. Xóa folder `bin` và `obj`
3. Chạy lại: `dotnet run`

## 🎯 Tips & Tricks

| Phím tắt | Chức năng |
|---------|----------|
| `Ctrl + ~` | Mở/Đóng Terminal trong VS Code |
| `Ctrl + K Ctrl + C` | Comment dòng |
| `Ctrl + K Ctrl + U` | Uncomment dòng |
| `F5` | Debug (nếu có launch config) |
| `Ctrl + B` | Toggle Sidebar |

## 📊 Dữ Liệu Demo Mặc Định

### Users (3)
- admin / admin123 (Role: Admin)
- user1 / user123 (Role: User)
- user2 / user123 (Role: User)

### Teams (1)
- "Bóng đá Thanh niên"
- Admin: admin user
- Members: user1, user2

### Data Files
- **data.json**: Lưu dữ liệu toàn bộ ứng dụng

## ✅ Checklist - Sẵn Sàng Chạy

- [ ] .NET 10.0 SDK cài đặt (chạy `dotnet --version`)
- [ ] Visual Studio Code cài đặt
- [ ] C# Dev Kit extension cài đặt
- [ ] Mở folder `SportsClubManagement` trong VS Code
- [ ] Mở Terminal (`Ctrl + ~`)
- [ ] Chạy `dotnet run`
- [ ] Đăng nhập với `admin / admin123` hoặc `user1 / user123`

## 📝 Thư Mục Quan Trọng Sau Chạy Lần Đầu

```
SportsClubManagement/
├── bin/
│   └── Debug/net10.0-windows/
│       ├── SportsClubManagement.exe    👈 Executable
│       └── [DLL files]
├── obj/                                👈 Build artifacts
├── data.json                           👈 Dữ liệu (auto-tạo)
└── [Source files]
```

## 🚀 Chạy Executable Trực Tiếp (Không cần Terminal)

Sau khi build thành công:

1. Mở Windows Explorer
2. Điều hướng đến: `bin\Debug\net10.0-windows\`
3. Double-click vào `SportsClubManagement.exe`

**Lưu ý**: Phải có `.NET 10.0 Runtime` cài đặt để chạy!

---

**🎉 Bây giờ bạn đã sẵn sàng sử dụng ứng dụng!**

Nếu có vấn đề, xem file `HƯỚNG_DẪN_CHẠY.md` để có thêm chi tiết.
