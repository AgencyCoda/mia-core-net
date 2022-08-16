using System;
using System.Collections.Generic;
using MiaCore.Models;

namespace MiaCore.Features
{
    public class MiaUserDto
    {
        public long Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Photo { get; set; }
        public string Phone { get; set; }
        public string FacebookId { get; set; }
        public int Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int Status { get; set; }
        public bool IsNotification { get; set; }
        public string Caption { get; set; }
        public int VerifiedStatus { get; set; }
        public int IdentificationType { get; set; }
        public decimal CredibilityPoints { get; set; }
        public string Language { get; set; }
        public bool OtpEnabled { get; set; }
        public List<MiaUserCategory> Categories { get; set; }

        public List<RequestChange> RequestChanges { get; set; }
    }

    public class MiauserDetailedDto : MiaUserDto
    {
        public string IdentificationFrontUrl { get; set; }
        public string IdentificationBackUrl { get; set; }
    }
}