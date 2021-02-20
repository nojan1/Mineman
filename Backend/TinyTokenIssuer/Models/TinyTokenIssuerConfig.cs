using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace TinyTokenIssuer.Models
{
    public class TinyTokenIssuerConfig
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public TimeSpan ExpiresIn { get; set; } = TimeSpan.FromMinutes(30);
        public SecurityKey SigningKey{ get; set; }
    }
}
