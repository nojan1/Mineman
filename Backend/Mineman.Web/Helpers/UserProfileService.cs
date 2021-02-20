using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinyTokenIssuer.Interfaces;
using TinyTokenIssuer.Models;

namespace Mineman.Web.Helpers
{
    public class TokenUser : BaseTokenProperties
    {

    }

    public class UserProfileService : IProfileService<TokenUser>
    {
        public UserProfileService()
        {

        }

        public TokenUser PasswordSignIn(string username, string password)
        {
            return new TokenUser
            {
                Subject = "ldfjkhgldfkjgkldfjg"
            };
        }
    }
}
