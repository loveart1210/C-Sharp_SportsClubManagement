# Phân Tích Chức Năng - So Sánh Yêu Cầu vs Hiện Tại

## 1. CHỨC NĂNG CHUNG (Cả Admin và User)

### 1.1 Quản lý trang cá nhân
- ✅ CRUD Avatar - **Có** (ProfileViewModel)
- ✅ CRUD Họ tên - **Có** (ProfileViewModel)
- ✅ CRUD Ngày sinh - **Có** (ProfileViewModel - BirthDate)
- ✅ CRUD Email - **Có** (ProfileViewModel)

### 1.2 Quản lý môn tập, lịch tập cá nhân
- ⚠️ Quản lý môn tập cá nhân - **Một phần** (Có Subject model nhưng chưa implement toàn bộ)
- ⚠️ Quản lý lịch tập cá nhân - **Một phần** (Có Session model + TeamSessionsViewModel)

---

## 2. CHỨC NĂNG ADMIN HỆ THỐNG

### 2.1 Quản lý User
- ✅ Xem thông tin cá nhân user - **Có** (UserManagementViewModel)
- ✅ Xem avatar user - **Có**
- ✅ Xem họ tên user - **Có**
- ✅ Xem email user - **Có**
- ✅ Quản lý tài khoản user - **Có**
- ✅ Reset password user - **Có** (RelayCommand trong UserManagementViewModel)

### 2.2 Quản lý Team
#### 2.2.1 Quản lý toàn bộ team
- ✅ CRUD thành viên - **Có** (TeamMembersViewModel)
- ✅ Sửa role thành viên - **Có** (TeamMember.Role)
- ✅ CRUD danh sách môn tập - **Có** (TeamSubjectsViewModel)
- ✅ Quản lý buổi tập (CRUD) - **Có** (TeamSessionsViewModel)
- ✅ CRUD avatar team - **Có** (Team.AvatarPath)
- ✅ CRUD tên team - **Có** (Team.Name)
- ⚠️ Quản lý ngày thành lập - **Có model** (Team.CreatedDate nhưng UI chưa allow edit)
- ⚠️ Tìm kiếm thành viên - **Chưa có** (Filter by role, subject, session)
- ⚠️ Tìm kiếm môn tập - **Chưa có** (Filter by member, session)
- ⚠️ Quản lý điểm danh - **Có model** (Attendance) nhưng UI chưa implement
- ✅ CRUD phần hiển thị thông báo - **Có** (Notification model)
- ❌ Xuất lịch tập - **Chưa có**

---

## 3. CHỨC NĂNG USER

### 3.1 Quản lý tài khoản cá nhân
- ⚠️ CRUD tên đăng nhập - **Chưa có** (Username edit chưa implement)
- ⚠️ Reset password cá nhân - **Chưa có** (Chỉ có admin mới reset được)

### 3.2 Quản lý team do mình thành lập
- ✅ CRUD thành viên - **Có**
- ✅ Sửa role thành viên - **Có**
- ✅ CRUD danh sách môn tập - **Có**
- ✅ Quản lý buổi tập - **Có**
- ✅ CRUD avatar team - **Có**
- ✅ CRUD tên team - **Có**
- ⚠️ Tìm kiếm thành viên - **Chưa có**
- ⚠️ Tìm kiếm môn tập - **Chưa có**
- ⚠️ Quản lý điểm danh - **Có model nhưng chưa implement UI**
- ✅ CRUD thông báo/bài viết - **Có**
- ✅ Xuất lịch tập - **Chưa có**
- ✅ Quản lý quỹ nhóm - **Có** (TeamFundsViewModel + ExportCommand)

### 3.3 Quản lý team do mình tham gia
- Tùy role (Member, Coach)
- ✅ Xem thông tin cơ bản - **Có**
- ⚠️ Quản lý điểm danh (Coach) - **Chưa implement UI**
- ✅ CRUD bài viết (Coach) - **Có**

---

## TÓML TẮT TÌNH TRẠNG

### ✅ Đã Implement Tốt (Core Features)
1. CRUD Profile (Avatar, Họ tên, Email, Ngày sinh)
2. CRUD Team Members
3. Sửa Role Thành viên
4. CRUD Subjects
5. CRUD Sessions
6. Quản lý Quỹ Team (Deposit/Withdraw)
7. Reset Password (Admin)
8. CRUD Notifications

### ⚠️ Đã Có Model Nhưng Cần Hoàn Thiện UI
1. Quản lý điểm danh (Attendance model có, UI cần)
2. Chỉnh sửa ngày thành lập team
3. Username edit cho user cá nhân
4. Reset password cho user cá nhân

### ❌ Cần Implement Mới
1. Tìm kiếm/Filter Members (by role, subject, session)
2. Tìm kiếm/Filter Subjects (by member, session)
3. Xuất lịch tập (CSV/PDF)
4. Xuất báo cáo tài chính khi xuất quỹ (auto-notification)
5. Phân quyền chi tiết theo role (Founder, Admin Team, Coach, Member)

---

## KẾ HOẠCH CẢI THIỆN

### Phase 1: Core Features (Ưu tiên Cao)
- [ ] Implement Attendance Management UI
- [ ] Add Filter/Search cho Members và Subjects
- [ ] Xuất lịch tập (CSV format)
- [ ] User password reset cho chính mình

### Phase 2: Enhancement (Ưu tiên Trung)
- [ ] Phân quyền chi tiết theo role
- [ ] Tự động tạo notification khi xuất báo cáo tài chính
- [ ] Chỉnh sửa ngày thành lập team (Admin only)

### Phase 3: Polish (Ưu tiên Thấp)
- [ ] Validation logic được tăng cường
- [ ] Error handling được cải thiện
