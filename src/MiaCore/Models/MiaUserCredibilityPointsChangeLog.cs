using System;
using MiaCore.Models.Enums;

namespace MiaCore.Models
{
    public class MiaUserCredibilityPointsChangeLog : BaseEntity
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Reason { get; set; }
        public decimal CredibilityPointsBefore { get; set; }
        public decimal CredibilityPointsAfter { get; set; }
        public decimal CredibilityPointsCheckerBefore { get; set; }
        public decimal CredibilityPointsCheckerAfter { get; set; }
        public decimal CredibilityPointsCreatorBefore { get; set; }
        public decimal CredibilityPointsCreatorAfter { get; set; }
    }
}