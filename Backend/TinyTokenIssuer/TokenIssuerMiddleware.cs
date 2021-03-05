using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TinyTokenIssuer.Interfaces;
using TinyTokenIssuer.Models;

namespace TinyTokenIssuer
{
    public class TokenIssuerMiddleware<TProfile> where TProfile : BaseTokenProperties
    {
        private readonly RequestDelegate _next;
        private readonly TokenSigningKeyProvider _keyProvider;
        private readonly TinyTokenIssuerConfig _config;

        public TokenIssuerMiddleware(RequestDelegate next, TokenSigningKeyProvider keyProvider, TinyTokenIssuerConfig config)
        {
            _next = next;
            _keyProvider = keyProvider;
            _config = config;
        }

        public async Task InvokeAsync(HttpContext context, IProfileService<TProfile> profileService)
        {
            if (context.Request.Path == "/token" && context.Request.Method == "POST")
            {
                //TODO: Support additional login types
                var username = context.Request.Form["username"];
                var password = context.Request.Form["password"];

                var user = profileService.PasswordSignIn(username, password);
                if (user == null)
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Bad login");
                    return;
                }

                var claims = user.ToClaims();

                claims.Add(new Claim(JwtRegisteredClaimNames.Aud, _config.Audience));
                claims.Add(new Claim(JwtRegisteredClaimNames.Iss, _config.Issuer));

                var token = new JwtSecurityToken(
                    _config.Issuer,
                    _config.Audience,
                    user.ToClaims(),
                    null,
                    DateTime.Now.Add(_config.ExpiresIn),
                    _keyProvider.Credentials
                );

                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(token);
                await context.Response.WriteAsync(encodedJwt);
                return;
            }

            await _next(context);
        }
    }
}
