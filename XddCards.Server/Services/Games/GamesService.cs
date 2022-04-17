using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using XddCards.Grpc.Games;
using XddCards.Server.Model;
using XddCards.Server.Repositories.Games.BlackJack;
using XddCards.Server.Services.Auth;

namespace XddCards.Server.Services.Games
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class GamesService : GamesGrpc.GamesGrpcBase
    {
        [Authorize]
        public override Task<BlackJackReply> CreateBlackJack(BlackJackRequest request, ServerCallContext context)
        {
            var user = context.GetUser();

            var game = GamesRepository.Create(user);

            return Task.FromResult(new BlackJackReply() { Id = -1 });
        }
    }

    public static class ServerCallContextExtensions
    {
        public static User GetUser(this ServerCallContext context)
        {
            var claim = context.GetHttpContext().User.Claims.First();

            return AuthService.Users
                .Where(x => x.Identity != null && x.Identity.Claims.Any())
                .First(x => x.Identity.Claims
                    .First().Value == claim.Value);
        }
    }
}
