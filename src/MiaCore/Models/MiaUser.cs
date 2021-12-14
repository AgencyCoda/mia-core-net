using System;

namespace MiaCore.Models
{
    public class MiaUser : BaseEntity
    {
        public long Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Photo { get; set; }
        public string Phone { get; set; }
        public string FacebookId { get; set; }
        public int Role { get; set; }
        public int Status { get; set; }
        public bool IsNotification { get; set; }
        public string Caption { get; set; }
    }
}