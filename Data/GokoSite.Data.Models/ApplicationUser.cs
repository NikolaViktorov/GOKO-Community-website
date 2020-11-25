// ReSharper disable VirtualMemberCallInConstructor
namespace GokoSite.Data.Models
{
    using System;
    using System.Collections.Generic;

    using GokoSite.Data.Common.Models;
    using GokoSite.Data.Models.LoL;
    using GokoSite.Data.Models.News;
    using Microsoft.AspNetCore.Identity;

    public class ApplicationUser : IdentityUser, IAuditInfo, IDeletableEntity
    {
        public ApplicationUser()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Roles = new HashSet<IdentityUserRole<string>>();
            this.Claims = new HashSet<IdentityUserClaim<string>>();
            this.Logins = new HashSet<IdentityUserLogin<string>>();
            this.UserGames = new HashSet<UserGames>();
            this.News = new HashSet<New>();
        }

        // Audit info
        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        // Deletable entity
        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public virtual ICollection<IdentityUserRole<string>> Roles { get; set; }

        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }

        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }

        public virtual ICollection<UserGames> UserGames { get; set; }

        public virtual ICollection<New> News { get; set; }
    }
}
