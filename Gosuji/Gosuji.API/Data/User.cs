﻿using Gosuji.Client.Data;
using Gosuji.Client.Data.Attributes;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Gosuji.API.Data
{
    public class User : IdentityUser, IDbModel
    {
        public long? CurrentSubscriptionId { get; set; }
        public Subscription? CurrentSubscription { get; set; }

        [CustomPersonalData]
        public bool IsBanned { get; set; }
        [CustomPersonalData]
        public DateTimeOffset? EmailConfirmedDate { get; set; }

        [Required]
        public string BackupCode { get; set; }

        [Required]
        [CustomPersonalData]
        public DateTimeOffset CreateDate { get; set; }
        [Required]
        [CustomPersonalData]
        public DateTimeOffset ModifyDate { get; set; }

        public User()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            CreateDate = now;
            ModifyDate = now;
        }
    }
}
