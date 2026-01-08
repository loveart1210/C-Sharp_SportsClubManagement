using Microsoft.EntityFrameworkCore;
using SportsClubManagement.Models;

namespace SportsClubManagement.Data
{
    public class SportsClubDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<FundTransaction> Transactions { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        public SportsClubDbContext(DbContextOptions<SportsClubDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);
            modelBuilder.Entity<User>()
                .Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(50);
            modelBuilder.Entity<User>()
                .Property(u => u.Password)
                .IsRequired();
            modelBuilder.Entity<User>()
                .Property(u => u.FullName)
                .IsRequired()
                .HasMaxLength(100);
            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100);
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .IsRequired()
                .HasDefaultValue("User");

            // Configure Team
            modelBuilder.Entity<Team>()
                .HasKey(t => t.Id);
            modelBuilder.Entity<Team>()
                .Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);
            modelBuilder.Entity<Team>()
                .Property(t => t.Balance)
                .HasPrecision(18, 2)
                .HasDefaultValue(0);

            // Configure TeamMember
            modelBuilder.Entity<TeamMember>()
                .HasKey(tm => tm.Id);
            modelBuilder.Entity<TeamMember>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(tm => tm.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<TeamMember>()
                .HasOne<Team>()
                .WithMany()
                .HasForeignKey(tm => tm.TeamId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<TeamMember>()
                .Property(tm => tm.Role)
                .IsRequired()
                .HasDefaultValue("Member");

            // Configure Subject
            modelBuilder.Entity<Subject>()
                .HasKey(s => s.Id);
            modelBuilder.Entity<Subject>()
                .Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(100);
            modelBuilder.Entity<Subject>()
                .HasOne<Team>()
                .WithMany()
                .HasForeignKey(s => s.TeamId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Session
            modelBuilder.Entity<Session>()
                .HasKey(s => s.Id);
            modelBuilder.Entity<Session>()
                .Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(100);
            modelBuilder.Entity<Session>()
                .HasOne<Team>()
                .WithMany()
                .HasForeignKey(s => s.TeamId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Session>()
                .HasOne<Subject>()
                .WithMany()
                .HasForeignKey(s => s.SubjectId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure Attendance
            modelBuilder.Entity<Attendance>()
                .HasKey(a => a.Id);
            modelBuilder.Entity<Attendance>()
                .HasOne<Session>()
                .WithMany()
                .HasForeignKey(a => a.SessionId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Attendance>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Attendance>()
                .Property(a => a.IsPresent)
                .IsRequired()
                .HasDefaultValue(false);

            // Configure FundTransaction
            modelBuilder.Entity<FundTransaction>()
                .HasKey(ft => ft.Id);
            modelBuilder.Entity<FundTransaction>()
                .Property(ft => ft.Amount)
                .HasPrecision(18, 2);
            modelBuilder.Entity<FundTransaction>()
                .HasOne<Team>()
                .WithMany()
                .HasForeignKey(ft => ft.TeamId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<FundTransaction>()
                .Property(ft => ft.Type)
                .IsRequired()
                .HasDefaultValue("Deposit");

            // Configure Notification
            modelBuilder.Entity<Notification>()
                .HasKey(n => n.Id);
            modelBuilder.Entity<Notification>()
                .Property(n => n.Title)
                .IsRequired()
                .HasMaxLength(200);
            modelBuilder.Entity<Notification>()
                .Property(n => n.Content)
                .IsRequired();
            modelBuilder.Entity<Notification>()
                .HasOne<Team>()
                .WithMany()
                .HasForeignKey(n => n.TeamId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes for better query performance
            modelBuilder.Entity<TeamMember>()
                .HasIndex(tm => new { tm.TeamId, tm.UserId })
                .IsUnique();
            modelBuilder.Entity<Session>()
                .HasIndex(s => new { s.TeamId, s.StartTime });
            modelBuilder.Entity<Attendance>()
                .HasIndex(a => new { a.SessionId, a.UserId })
                .IsUnique();
        }
    }
}
