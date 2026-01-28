using System;
using System.Collections.Generic;

namespace SportsClubManagement.Models
{
    public class User
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // For demo only - should be hashed in production
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "User"; // "Admin", "User"
        public string AvatarPath { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; } = DateTime.Now;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool MustChangePassword { get; set; } = false;
    }

    public class TeamMember
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = string.Empty;
        public string TeamId { get; set; } = string.Empty;
        public string Role { get; set; } = "Member"; // "Founder", "Admin", "Coach", "Member"
        public DateTime JoinDate { get; set; } = DateTime.Now;
    }

    public class Subject
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? TeamId { get; set; }
        public string? UserId { get; set; } // Nullable, if set, it's a personal subject
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ParticipantCount { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }

    public class Session
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? TeamId { get; set; }
        public string? UserId { get; set; } // Nullable, if set, it's a personal session
        public string SubjectId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Note { get; set; } = string.Empty;
        public bool IsAttended { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }

    public class Attendance
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string SessionId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public bool IsPresent { get; set; }
        public string Note { get; set; } = string.Empty;
        public DateTime RecordedDate { get; set; } = DateTime.Now;
    }

    public class FundTransaction
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string TeamId { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Now;
        public decimal Amount { get; set; } // Positive = Deposit, Negative = Withdraw
        public string Description { get; set; } = string.Empty;
        public string ByUserId { get; set; } = string.Empty;
        public string Type { get; set; } = "Deposit"; // "Deposit", "Withdraw"
    }

    public class Notification
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string TeamId { get; set; } = string.Empty;
        public string ByUserId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsSystemNotification { get; set; } = false;
        public string Title { get; set; } = string.Empty;
    }
}
