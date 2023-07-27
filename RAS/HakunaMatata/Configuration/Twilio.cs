using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HakunaMatata.Configuration
{
    public class Twilio
    {
        public string AccountSid { get; set; } = "MY_ACOUNT_SID";
        public string AuthToken { get; set; } = "MY_AUTH_TOKEN";
        public string VerificationSid { get; set; } = "VERIFICATION_SID";
    }
}
