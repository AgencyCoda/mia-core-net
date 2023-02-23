using System;
using MiaCore.Models.Enums;

namespace MiaCore.Models
{
    public class MiaPlanSchedule : BaseEntity
    {
        public int Id { get; set; }
        public int PlanId { get; set; }
        [Relation]
        public MiaPlan Plan { get; set; }
        public decimal PriceMonth { get; set; }
        public decimal PriceMonthUsd { get; set; }
        public DateTime ChangePriceDate { get; set; }
        public MiaPlanScheduleStatus Status { get; set; }
    }
}