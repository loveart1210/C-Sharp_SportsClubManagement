using System;
using System.Collections.ObjectModel;

namespace SportsClubManagement.Models
{
    public class Team
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string AvatarPath { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public decimal Balance { get; set; } = 0m;
        public string JoinCode { get; set; } = string.Empty;
    }
}
