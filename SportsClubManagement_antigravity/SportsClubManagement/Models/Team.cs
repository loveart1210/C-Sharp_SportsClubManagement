using System;
using System.Collections.ObjectModel;

namespace SportsClubManagement.Models
{
    public class Team
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Description { get; set; }
        public string AvatarPath { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public decimal Balance { get; set; } = 0m;
    }
}
