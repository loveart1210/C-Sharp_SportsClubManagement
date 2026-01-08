using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SportsClubManagement.Models;

namespace SportsClubManagement.Services
{
    public class ExportService
    {
        public static string ExportScheduleToCSV(Team team, DateTime? startDate = null, DateTime? endDate = null)
        {
            var filePath = GetExportFilePath($"LichTap_{team.Name}_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
            var sessions = DataService.Instance.Sessions
                .Where(s => s.TeamId == team.Id)
                .ToList();

            if (startDate.HasValue)
                sessions = sessions.Where(s => s.StartTime >= startDate.Value).ToList();
            if (endDate.HasValue)
                sessions = sessions.Where(s => s.StartTime <= endDate.Value).ToList();

            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                // Header
                writer.WriteLine($"LỊCH TẬP - {team.Name}");
                writer.WriteLine($"Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
                writer.WriteLine();
                writer.WriteLine("Buổi tập,Môn tập,Thời gian bắt đầu,Thời gian kết thúc,Ghi chú");

                // Data
                foreach (var session in sessions.OrderBy(s => s.StartTime))
                {
                    var subject = DataService.Instance.Subjects.FirstOrDefault(s => s.Id == session.SubjectId);
                    var subjectName = subject?.Name ?? "N/A";
                    
                    writer.WriteLine($"\"{session.Name}\",\"{subjectName}\",{session.StartTime:dd/MM/yyyy HH:mm},{session.EndTime:dd/MM/yyyy HH:mm},\"{session.Note ?? ""}\"");
                }
            }

            return filePath;
        }

        public static string ExportFinancialReportToCSV(Team team)
        {
            var filePath = GetExportFilePath($"BaoCaoTaiChinh_{team.Name}_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
            var transactions = DataService.Instance.Transactions
                .Where(t => t.TeamId == team.Id)
                .OrderByDescending(t => t.Date)
                .ToList();

            decimal runningBalance = team.Balance;
            var depositTotal = transactions.Where(t => t.Type == "Deposit").Sum(t => t.Amount);
            var withdrawTotal = transactions.Where(t => t.Type == "Withdraw").Sum(t => t.Amount);

            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                // Header
                writer.WriteLine($"BÁO CÁO TÀI CHÍNH - {team.Name}");
                writer.WriteLine($"Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
                writer.WriteLine();
                writer.WriteLine($"Số dư hiện tại: {runningBalance:N0} VNĐ");
                writer.WriteLine($"Tổng nạp quỹ: {depositTotal:N0} VNĐ");
                writer.WriteLine($"Tổng rút quỹ: {withdrawTotal:N0} VNĐ");
                writer.WriteLine();
                writer.WriteLine("Ngày,Loại,Số tiền,Mô tả,Người thực hiện");

                // Data
                foreach (var transaction in transactions)
                {
                    var user = DataService.Instance.Users.FirstOrDefault(u => u.Id == transaction.ByUserId);
                    var userName = user?.FullName ?? "N/A";
                    var amount = transaction.Type == "Withdraw" ? -transaction.Amount : transaction.Amount;
                    
                    writer.WriteLine($"{transaction.Date:dd/MM/yyyy HH:mm},{transaction.Type},\"{Math.Abs(amount):N0}\",\"{transaction.Description}\",\"{userName}\"");
                }
            }

            return filePath;
        }

        public static string ExportMembersToCSV(Team team)
        {
            var filePath = GetExportFilePath($"DanhSachThanhVien_{team.Name}_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
            var members = DataService.Instance.TeamMembers
                .Where(tm => tm.TeamId == team.Id)
                .OrderBy(tm => tm.Role)
                .ToList();

            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                // Header
                writer.WriteLine($"DANH SÁCH THÀNH VIÊN - {team.Name}");
                writer.WriteLine($"Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
                writer.WriteLine();
                writer.WriteLine("Tên,Email,Vai trò,Ngày tham gia");

                // Data
                foreach (var member in members)
                {
                    var user = DataService.Instance.Users.FirstOrDefault(u => u.Id == member.UserId);
                    if (user != null)
                    {
                        writer.WriteLine($"\"{user.FullName}\",{user.Email},{member.Role},{member.JoinDate:dd/MM/yyyy}");
                    }
                }
            }

            return filePath;
        }

        public static string ExportPersonalScheduleToCSV(User user, DateTime? startDate = null, DateTime? endDate = null)
        {
            var filePath = GetExportFilePath($"LichTapCaNhan_{user.Username}_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
            var sessions = DataService.Instance.Sessions
                .Where(s => s.UserId == user.Id)
                .ToList();

            if (startDate.HasValue)
                sessions = sessions.Where(s => s.StartTime >= startDate.Value).ToList();
            if (endDate.HasValue)
                sessions = sessions.Where(s => s.StartTime <= endDate.Value).ToList();

            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                // Header
                writer.WriteLine($"LỊCH TẬP CÁ NHÂN - {user.FullName}");
                writer.WriteLine($"Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
                writer.WriteLine();
                writer.WriteLine("Buổi tập,Môn tập,Thời gian bắt đầu,Thời gian kết thúc,Ghi chú");

                // Data
                foreach (var session in sessions.OrderBy(s => s.StartTime))
                {
                    var subject = DataService.Instance.Subjects.FirstOrDefault(s => s.Id == session.SubjectId);
                    var subjectName = subject?.Name ?? "N/A";
                    
                    writer.WriteLine($"\"{session.Name}\",\"{subjectName}\",{session.StartTime:dd/MM/yyyy HH:mm},{session.EndTime:dd/MM/yyyy HH:mm},\"{session.Note ?? ""}\"");
                }
            }

            return filePath;
        }

        private static string GetExportFilePath(string fileName)
        {
            var exportDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "SportsClubManagement_Exports"
            );

            if (!Directory.Exists(exportDir))
                Directory.CreateDirectory(exportDir);

            return Path.Combine(exportDir, fileName);
        }
    }
}
