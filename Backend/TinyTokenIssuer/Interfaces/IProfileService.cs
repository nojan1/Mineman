using System;
using System.Collections.Generic;
using System.Text;
using TinyTokenIssuer.Models;

namespace TinyTokenIssuer.Interfaces
{
    public interface IProfileService<TProfile> where TProfile: BaseTokenProperties
    {
        TProfile PasswordSignIn(string username, string password);
    }
}
