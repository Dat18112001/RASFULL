using System;
using System.Collections.Generic;

namespace HakunaMatata.Models.DataModels
{
    public partial class SocialLogin
    {
        public string ProviderKey { get; set; }
        public int UserId { get; set; }
        public string Provider { get; set; }

        public virtual Agent User { get; set; }
    }
}
