using System.Security.Claims;

namespace XddCards.Server.Model
{
    public class User
    {
        public string Nickname { get; set; }
        public ClaimsIdentity Identity { get; set; }
    }
}
