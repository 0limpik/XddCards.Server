using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.IdentityModel.Tokens;
using XddCards.Server.Auth;
using XddCards.Server.Model;
using XddCards.Server.Model.Auth;

namespace XddCards.Server.Services.Auth
{
    public class AuthService : AuthGrpc.AuthGrpcBase
    {
        private static List<User> people = new List<User>
        {
            new User { Nickname = "Olimpik" },
        };

        public override Task<AuthReply> Auth(AuthRequest request, ServerCallContext context)
        {
            if (people.Any(x => x.Nickname == request.Nickname))
                throw new Exception();

            var identity = GetIdentity(request.Nickname);

            people.Add(new User { Nickname = request.Nickname, Identity = identity });

            var now = DateTime.UtcNow;

            var token = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(AuthOptions.Lifetime),
                signingCredentials: new SigningCredentials(
                    AuthOptions.SecurityKey,
                    SecurityAlgorithms.HmacSha256));

            var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);

            return Task.FromResult(new AuthReply { Token = encodedToken });
        }

        private ClaimsIdentity GetIdentity(string nickname)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, nickname),
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, "Token", 
                ClaimsIdentity.DefaultNameClaimType, 
                ClaimsIdentity.DefaultRoleClaimType);

            return claimsIdentity;
        }
    }
}
