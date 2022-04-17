using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace XddCards.Server.Model.Auth
{
    public static class AuthOptions
    {
        public const string ISSUER = "MyAuthServer"; // издатель токена
        public const string AUDIENCE = "MyAuthClient"; // потребитель токена
        const string KEY = "WAoe@#opHJF_H#^_)*AW_Sdfnsay*C_dBSYG_7";   // ключ для шифрации
        private const int _LIFETIME = 5;
        public static TimeSpan Lifetime => TimeSpan.FromMinutes(_LIFETIME);

        public static SymmetricSecurityKey SecurityKey => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));   
    }
}
