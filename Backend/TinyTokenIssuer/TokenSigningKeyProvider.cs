using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;
using TinyTokenIssuer.Models;

namespace TinyTokenIssuer
{
    public class TokenSigningKeyProvider
    {
        public SigningCredentials Credentials { get; private set; }

        public TokenSigningKeyProvider(TinyTokenIssuerConfig config)
        {
            if(config.SigningKey == null)
                config.SigningKey = new SymmetricSecurityKey(Guid.NewGuid().ToByteArray());

            Credentials = new SigningCredentials(config.SigningKey, SecurityAlgorithms.HmacSha256);
        }

    }
}
