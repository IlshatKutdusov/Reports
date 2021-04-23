using System;

namespace Reports.API.Models
{
    public class BaseEntity : IBaseEntity
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime DateUpdated { get; set; }
        public bool isActive { get; set; } = true;
    }
}
