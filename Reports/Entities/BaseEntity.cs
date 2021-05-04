﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Reports.Entities
{
    public class BaseEntity : IBaseEntity
    {
        [Key]
        public int Id { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public DateTime DateUpdated { get; set; }

        public bool isActive { get; set; } = true;
    }
}