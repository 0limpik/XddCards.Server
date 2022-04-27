using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using XddCards.Grpc.Games.BlackJack;
using XddCards.Server.Repositories.Cycles.BlackJack;
using XddCards.Server.Services.Games;

namespace XddCards.Server.Controllers.Games
{
    [Authorize]
    public class BlackJackController : BlackJackGrpc.BlackJackGrpcBase
    {
        public override Task<CreateResponse> Create(CreateRequest request, ServerCallContext context)
        {
            var user = context.GetUser();

            var game = BJCyclesRepository.Create(user);

            return Task.FromResult(new CreateResponse
            {
                Game = new Game { Id = game.cycle.Id },
                User = new Grpc.Cycles.BlackJack.User
                {
                    Id = game.user.Id,
                    Nickname = game.user.User.Nickname
                }
            });
        }

        public override Task<ConnectResponse> Connect(ConnectRequest request, ServerCallContext context)
        {
            var user = context.GetUser();

            var game = BJCyclesRepository.Connect(request.Game.Id, user);

            return Task.FromResult(new ConnectResponse
            {
                User = new Grpc.Cycles.BlackJack.User
                {
                    Id = game.user.Id,
                    Nickname = game.user.User.Nickname
                }
            });
        }

        public override Task<AllResponse> GetAll(AllRequest request, ServerCallContext context)
        {
            var cycles = BJCyclesRepository.GetAll();

            var response = new AllResponse();

            foreach (var cycle in cycles)
            {
                response.Games.Add(new Game { Id = cycle.Id });
            }

            return Task.FromResult(response);
        }
    }
}
