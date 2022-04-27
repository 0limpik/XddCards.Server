using System;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace XddCards.Server.Model.Auth
{
    public static class AuthOptions
    {
        public const string ISSUER = "MyAuthServer";
        public const string AUDIENCE = "MyAuthClient";
        const string KEY = "WAoe@#opHJF_H#^_)*AW_Sdfnsay*C_dBSYG_7";
        private const int _LIFETIME = 5;
        public static TimeSpan Lifetime => TimeSpan.FromMinutes(_LIFETIME);

        public static SymmetricSecurityKey SecurityKey => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
    }
}
