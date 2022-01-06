using System;

namespace MiaCore.Models
{
    public class MiaUserPlan : BaseEntity, IEntity
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public int PlanId { get; set; }
        public int Frequency { get; set; }
        public decimal Amount { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int ProviderId { get; set; }
        public string Data { get; set; }
        public int Status { get; set; }
    }
}