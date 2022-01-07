using System;

namespace MiaCore.Models
{
    public class BaseEntity : IEntity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}