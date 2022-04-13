using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using XddCards.Server.Games;

namespace XddCards.Server.Services.Games
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class GamesService : GamesGrpc.GamesGrpcBase
    {
        [Authorize]
        public override Task<BlackJackReply> CreateBlackJack(BlackJackRequest request, ServerCallContext context)
        {
            var user = context.GetHttpContext().User;

            //GamesRepository.Create();

            return Task.FromResult(new BlackJackReply() { Id = -1 });
        }
    }

    public static class ServerCallContextExtensions
    {
        public static void User(this ServerCallContext context)
        {
        }
    }
}
