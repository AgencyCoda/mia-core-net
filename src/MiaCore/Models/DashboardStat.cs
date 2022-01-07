using System;

namespace MiaCore.Models
{
    public class DashboardStat
    {
        public decimal TotalTokens { get; set; }
        public long TotalAccounts { get; set; }
        public long TotalNews { get; set; }
        public decimal TotalRewards { get; set; }
        public long TotalTransfers { get; set; }
        public long TotalPlans { get; set; }
    }
}