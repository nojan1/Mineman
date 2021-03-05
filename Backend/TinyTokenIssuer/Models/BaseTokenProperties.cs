using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TinyTokenIssuer.Models
{
    public class BaseTokenProperties
    {
        public string Subject { get; set; }
        public string[] Roles { get; set; } = new string[0];
        public string[] Scopes { get; set; } = new string[0];

        public virtual IEnumerable<Claim> GetAdditionalClaims() => new List<Claim>();

        internal List<Claim> ToClaims()
        {
            var claimList = new List<Claim>();

            claimList.Add(new Claim(JwtRegisteredClaimNames.Sub, Subject));
            claimList.AddRange(GetAdditionalClaims());

            return claimList;
        }
    }
}
