using System;
using System.Collections.Generic;
using MiaCore.Models.Enums;

namespace MiaCore.Models
{
    public class MiaUser : BaseEntity, IEntity, IDeletableEntity
    {
        public long Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Photo { get; set; }
        public string Phone { get; set; }
        public string FacebookId { get; set; }
        public int Role { get; set; }
        public MiaUserStatus Status { get; set; }
        public bool IsNotification { get; set; }
        public string Caption { get; set; }
        public MiaUserVerifiedStatus VerifiedStatus { get; set; }
        public UserIdentificationType IdentificationType { get; set; }
        public string IdentificationFrontUrl { get; set; }
        public string IdentificationBackUrl { get; set; }
        public decimal CredibilityPoints { get; set; }
        public decimal CredibilityPointsChecker { get; set; }
        public decimal CredibilityPointsCreator { get; set; }
        public string Language { get; set; }
        public bool OtpEnabled { get; set; }
        public bool WalletConnected { get; set; }
        public bool VerifyEmailSent { get; set; }
        public string StripeCustomerId { get; set; }
        public string PaypalMerchantId { get; set; }
        public bool UseArs { get; set; }

        [Relation]
        public List<MiaUserCategory> Categories { get; set; }

        [Relation]
        public List<RequestChange> RequestChanges { get; set; }
    }
}