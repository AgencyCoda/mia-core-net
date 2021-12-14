using System;

namespace MiaCore.Models
{
    public class MiaRecovery : BaseEntity
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Token { get; set; }
        public int Status { get; set; }
    }
}