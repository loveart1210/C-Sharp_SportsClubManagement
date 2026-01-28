using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SportsClubManagement.Models;
using SportsClubManagement.Data;
using SportsClubManagement.Helpers;

namespace SportsClubManagement.Services
{
    public class DataService
    {
        private static readonly string LOG_FILE = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "debug.log");
        private static readonly string FILE_NAME = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SportsClubManagement",
            "data.json"
        );
        
        private static DataService? _instance;
        public static DataService Instance => _instance ??= new DataService();

        public List<User> Users { get; set; } = new List<User>();
        public List<Team> Teams { get; set; } = new List<Team>();
        public List<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
        public List<Subject> Subjects { get; set; } = new List<Subject>();
        public List<Session> Sessions { get; set; } = new List<Session>();
        public List<Attendance> Attendances { get; set; } = new List<Attendance>();
        public List<FundTransaction> Transactions { get; set; } = new List<FundTransaction>();
        public List<Notification> Notifications { get; set; } = new List<Notification>();

        public User? CurrentUser { get; set; }

        private bool _useDatabase = false;
        private const string CONNECTION_STRING = "Server=localhost;Database=sports_club_management;User=root;Password=;";

        public DataService()
        {
            InitializeDatabase();
            if (!_useDatabase)
            {
                Load();
                if (!Users.Any()) SeedData();
                else { EnsureDemoUsers(); EnsureJoinCodes(); }
                MigratePasswords();
            }
            else
            {
                EnsureJoinCodes();
                MigratePasswords(); // Migrate DB users too if they are loaded
            }
        }

        public void InitializeDatabase()
        {
            try
            {
                var optionsBuilder = new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<SportsClubDbContext>();
                optionsBuilder.UseMySql(CONNECTION_STRING, Microsoft.EntityFrameworkCore.ServerVersion.AutoDetect(CONNECTION_STRING));

                using (var context = new SportsClubDbContext(optionsBuilder.Options))
                {
                    Log("Attempting to connect to MySQL...");
                    if (context.Database.CanConnect())
                    {
                        Log("MySQL connection successful. Ensuring database and tables exist...");
                        context.Database.EnsureCreated();
                        _useDatabase = true;
                        
                        // Load from DB
                        LoadFromDb(context);
                        
                        // If DB was empty but JSON has data, migrate JSON to DB
                        if (!Users.Any())
                        {
                            Log("Database is empty. Checking JSON for data to migrate...");
                            Load(); // Load from JSON
                            if (Users.Any())
                            {
                                Log($"Found {Users.Count} users in JSON. Migrating to DB...");
                                Save(); // Save to DB
                            }
                        }
                    }
                    else
                    {
                        Log("MySQL connection failed: CanConnect returned false.");
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"MySQL initialization error: {ex.Message}\n{ex.StackTrace}");
                _useDatabase = false;
            }
        }

        private void Log(string message)
        {
            try
            {
                File.AppendAllText(LOG_FILE, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}\n");
                System.Diagnostics.Debug.WriteLine(message);
            }
            catch { }
        }

        private void LoadFromDb(SportsClubDbContext context)
        {
            Users = context.Users.ToList();
            Teams = context.Teams.ToList();
            TeamMembers = context.TeamMembers.ToList();
            Subjects = context.Subjects.ToList();
            Sessions = context.Sessions.ToList();
            Attendances = context.Attendances.ToList();
            Transactions = context.Transactions.ToList();
            Notifications = context.Notifications.ToList();

            if (!Users.Any())
            {
                SeedData();
                Save();
            }
        }

        public void Load()
        {
            if (_useDatabase) return;

            try
            {
                var directory = Path.GetDirectoryName(FILE_NAME);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory!);

                if (File.Exists(FILE_NAME))
                {
                    var json = File.ReadAllText(FILE_NAME);
                    var data = JsonSerializer.Deserialize<DataWrapper>(json);
                    if (data != null)
                    {
                        Users = data.Users ?? new List<User>();
                        Teams = data.Teams ?? new List<Team>();
                        TeamMembers = data.TeamMembers ?? new List<TeamMember>();
                        Subjects = data.Subjects ?? new List<Subject>();
                        Sessions = data.Sessions ?? new List<Session>();
                        Attendances = data.Attendances ?? new List<Attendance>();
                        Transactions = data.Transactions ?? new List<FundTransaction>();
                        Notifications = data.Notifications ?? new List<Notification>();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading data: {ex.Message}");
                throw;
            }
        }

        public void Save()
        {
            if (_useDatabase)
            {
                try
                {
                    var optionsBuilder = new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<SportsClubDbContext>();
                    optionsBuilder.UseMySql(CONNECTION_STRING, Microsoft.EntityFrameworkCore.ServerVersion.AutoDetect(CONNECTION_STRING));

                    using (var context = new SportsClubDbContext(optionsBuilder.Options))
                    {
                        Log($"Synchronizing {Users.Count} users, {Teams.Count} teams to DB...");
                        // Simple sync logic for demo purposes
                        // For Users
                        foreach (var user in Users)
                        {
                            var existing = context.Users.Find(user.Id);
                            if (existing == null) context.Users.Add(user);
                            else context.Entry(existing).CurrentValues.SetValues(user);
                        }
                        // Stop here for brevity in chunk but implement others too

                        // For Teams
                        foreach (var team in Teams)
                        {
                            var existing = context.Teams.Find(team.Id);
                            if (existing == null) context.Teams.Add(team);
                            else context.Entry(existing).CurrentValues.SetValues(team);
                        }

                        // For TeamMembers
                        foreach (var member in TeamMembers)
                        {
                            var existing = context.TeamMembers.Find(member.Id);
                            if (existing == null) context.TeamMembers.Add(member);
                            else context.Entry(existing).CurrentValues.SetValues(member);
                        }

                        // For Subjects
                        foreach (var subject in Subjects)
                        {
                            var existing = context.Subjects.Find(subject.Id);
                            if (existing == null) context.Subjects.Add(subject);
                            else context.Entry(existing).CurrentValues.SetValues(subject);
                        }

                        // For Sessions
                        foreach (var session in Sessions)
                        {
                            var existing = context.Sessions.Find(session.Id);
                            if (existing == null) context.Sessions.Add(session);
                            else context.Entry(existing).CurrentValues.SetValues(session);
                        }

                        // For Attendances
                        foreach (var att in Attendances)
                        {
                            var existing = context.Attendances.Find(att.Id);
                            if (existing == null) context.Attendances.Add(att);
                            else context.Entry(existing).CurrentValues.SetValues(att);
                        }

                        // For Transactions
                        foreach (var trans in Transactions)
                        {
                            var existing = context.Transactions.Find(trans.Id);
                            if (existing == null) context.Transactions.Add(trans);
                            else context.Entry(existing).CurrentValues.SetValues(trans);
                        }

                        // For Notifications
                        foreach (var note in Notifications)
                        {
                            var existing = context.Notifications.Find(note.Id);
                            if (existing == null) context.Notifications.Add(note);
                            else context.Entry(existing).CurrentValues.SetValues(note);
                        }

                        context.SaveChanges();
                        Log("DB sync successful.");
                        SaveToJson(); // Also update JSON for backup
                    }
                }
                catch (Exception ex)
                {
                    Log($"Error saving to DB: {ex.Message}\n{ex.StackTrace}");
                    SaveToJson();
                }
            }
            else
            {
                SaveToJson();
            }
        }

        private void SaveToJson()
        {
            try
            {
                var directory = Path.GetDirectoryName(FILE_NAME);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory!);

                var data = new DataWrapper
                {
                    Users = Users,
                    Teams = Teams,
                    TeamMembers = TeamMembers,
                    Subjects = Subjects,
                    Sessions = Sessions,
                    Attendances = Attendances,
                    Transactions = Transactions,
                    Notifications = Notifications
                };
                var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(FILE_NAME, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving data: {ex.Message}");
                throw;
            }
        }

        private void SeedData()
        {
            var admin = new User
            {
                Username = "admin",
                Password = PasswordHasher.Hash("admin123"),
                FullName = "Admin System",
                Role = "Admin",
                Email = "admin@sports.club",
                BirthDate = new DateTime(1990, 1, 1)
            };
            Users.Add(admin);

            var user1 = new User
            {
                Username = "user1",
                Password = PasswordHasher.Hash("user123"),
                FullName = "Nguyễn Văn A",
                Role = "User",
                Email = "user1@sports.club",
                BirthDate = new DateTime(2000, 5, 15)
            };
            Users.Add(user1);

            var user2 = new User
            {
                Username = "user2",
                Password = PasswordHasher.Hash("user123"),
                FullName = "Trần Văn B",
                Role = "User",
                Email = "user2@sports.club",
                BirthDate = new DateTime(2001, 3, 20)
            };
            Users.Add(user2);

            var team1 = new Team
            {
                Name = "Bóng Chuyền A",
                Description = "Đội bóng chuyền chính",
                Balance = 5000000,
                JoinCode = GenerateJoinCode()
            };
            Teams.Add(team1);

            TeamMembers.Add(new TeamMember
            {
                TeamId = team1.Id,
                UserId = user1.Id,
                Role = "Founder",
                JoinDate = DateTime.Now
            });

            TeamMembers.Add(new TeamMember
            {
                TeamId = team1.Id,
                UserId = user2.Id,
                Role = "Coach",
                JoinDate = DateTime.Now.AddDays(-10)
            });

            var subject1 = new Subject
            {
                TeamId = team1.Id,
                Name = "Kỹ thuật cơ bản",
                Description = "Học các kỹ thuật chuyền, phát bóng",
                ParticipantCount = 2
            };
            Subjects.Add(subject1);

            var sessionDate = DateTime.Now.AddDays(2);
            var session1 = new Session
            {
                TeamId = team1.Id,
                SubjectId = subject1.Id,
                Name = "Buổi tập chiều",
                StartTime = new DateTime(sessionDate.Year, sessionDate.Month, sessionDate.Day, 15, 0, 0),
                EndTime = new DateTime(sessionDate.Year, sessionDate.Month, sessionDate.Day, 17, 0, 0),
                Note = "Mang theo nước uống"
            };
            Sessions.Add(session1);

            var notif = new Notification
            {
                TeamId = team1.Id,
                ByUserId = user1.Id,
                Title = "Thông báo tạo team",
                Content = "Team Bóng Chuyền A vừa được tạo",
                IsSystemNotification = true
            };
            Notifications.Add(notif);

            Save();
        }

        private void EnsureDemoUsers()
        {
            bool changed = false;
            
            if (!Users.Any(u => u.Username == "admin"))
            {
                Users.Add(new User
                {
                    Username = "admin",
                    Password = PasswordHasher.Hash("admin123"),
                    FullName = "Admin System",
                    Role = "Admin",
                    Email = "admin@sports.club",
                    BirthDate = new DateTime(1990, 1, 1)
                });
                changed = true;
            }

            if (!Users.Any(u => u.Username == "user1"))
            {
                Users.Add(new User
                {
                    Username = "user1",
                    Password = PasswordHasher.Hash("user123"),
                    FullName = "Nguyễn Văn A",
                    Role = "User",
                    Email = "user1@sports.club",
                    BirthDate = new DateTime(2000, 5, 15)
                });
                changed = true;
            }

            if (!Users.Any(u => u.Username == "user2"))
            {
                Users.Add(new User
                {
                    Username = "user2",
                    Password = PasswordHasher.Hash("user123"),
                    FullName = "Trần Văn B",
                    Role = "User",
                    Email = "user2@sports.club",
                    BirthDate = new DateTime(2001, 3, 20)
                });
                changed = true;
            }

            if (changed) Save();
        }

        public bool CanManageTeam(User user, Team team)
        {
            if (user.Role == "Admin") return true;
            var membership = TeamMembers.FirstOrDefault(tm => tm.UserId == user.Id && tm.TeamId == team.Id);
            return membership?.Role == "Founder" || membership?.Role == "Admin";
        }

        public bool CanManageAttendance(User user, Team team)
        {
            if (user.Role == "Admin") return true;
            var membership = TeamMembers.FirstOrDefault(tm => tm.UserId == user.Id && tm.TeamId == team.Id);
            return membership?.Role == "Founder" || membership?.Role == "Admin" || membership?.Role == "Coach";
        }

        public string GenerateJoinCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            string code;
            do
            {
                code = new string(Enumerable.Repeat(chars, 8)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
            } while (Teams.Any(t => t.JoinCode == code));
            
            return code;
        }

        public void EnsureJoinCodes()
        {
            bool changed = false;
            foreach (var team in Teams)
            {
                if (string.IsNullOrWhiteSpace(team.JoinCode))
                {
                    team.JoinCode = GenerateJoinCode();
                    changed = true;
                }
            }
            if (changed) Save();
        }

        private void MigratePasswords()
        {
            bool changed = false;
            foreach (var user in Users)
            {
                // SHA-256 hex hash is exactly 64 characters
                if (user.Password.Length != 64)
                {
                    user.Password = PasswordHasher.Hash(user.Password);
                    changed = true;
                }
            }
            if (changed) Save();
        }
    }

    public class DataWrapper
    {
        public List<User> Users { get; set; } = new List<User>();
        public List<Team> Teams { get; set; } = new List<Team>();
        public List<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
        public List<Subject> Subjects { get; set; } = new List<Subject>();
        public List<Session> Sessions { get; set; } = new List<Session>();
        public List<Attendance> Attendances { get; set; } = new List<Attendance>();
        public List<FundTransaction> Transactions { get; set; } = new List<FundTransaction>();
        public List<Notification> Notifications { get; set; } = new List<Notification>();
    }
}

