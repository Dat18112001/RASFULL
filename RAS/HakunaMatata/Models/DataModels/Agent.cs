using System;
using System.Collections.Generic;

namespace HakunaMatata.Models.DataModels
{
    public partial class Agent
    {
        public Agent()
        {
            RealEstate = new HashSet<RealEstate>();
            SocialLogin = new HashSet<SocialLogin>();
        }

        public int Id { get; set; }
        public string AgentName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Facebook { get; set; }
        public string Zalo { get; set; }
        public string Avatar { get; set; }
        public int? Coin { get; set; }
        public int? Package { get; set; }
        public string LoginName { get; set; }
        public string Password { get; set; }
        public string ActiveKey { get; set; }
        public string ResetPasswordKey { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool IsActive { get; set; }
        public int LevelId { get; set; }
        public bool ConfirmPhoneNumber { get; set; }

        public virtual Level Level { get; set; }

        public virtual ICollection<RealEstate> RealEstate { get; set; }
        public virtual ICollection<SocialLogin> SocialLogin { get; set; }
        public virtual ICollection<Forum> Forum { get; set; }
        public virtual ICollection<Wishlist> Wishlist { get; set; }

        public virtual ICollection<News> News { get; set; }

        public virtual ICollection<Like> Like { get; set; }

        public virtual ICollection<Comment> Comment { get; set; }
        public virtual ICollection<HistoryPayment> HistoryPayment { get; set; }
    }
}
