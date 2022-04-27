using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using XddCards.Grpc.Cycles.BlackJack;
using XddCards.Grpc.Cycles.BlackJack.Controllers;
using XddCards.Server.Model.Cycles.BlackJack;
using XddCards.Server.Repositories.Cycles.BlackJack;
using XddCards.Server.Services.Games;

namespace XddCards.Server.Controllers.Cycles.BlackJack.Controllers
{
    [Authorize]
    public class HandController : HandControllerGrpc.HandControllerGrpcBase
    {
        public async override Task OnHandsChange(HandsRequest request, IServerStreamWriter<HandsResponse> responseStream, ServerCallContext context)
        {
            var cycle = GetCycle(context);

            await responseStream.WriteAsync(CreateResponse(cycle));

            while (true)
            {
                await cycle.handModel.OnHandsChange.SetToken(context.CancellationToken);

                var response = CreateResponse(cycle);

                await responseStream.WriteAsync(response);

            }
        }

        private static HandsResponse CreateResponse(BJCycleModel cycle)
        {
            var response = new HandsResponse();

            foreach (var hand in cycle.hands)
            {
                User user = null;
                if (hand.owner is not null)
                {
                    user = new User
                    {
                        Id = hand.owner.Id,
                        Nickname = hand.owner.User.Nickname,
                    };
                }

                response.Hands.Add(new Hand
                {
                    Id = hand.Id,
                    Owner = user
                });
            }
            return response;
        }

        private BJCycleModel GetCycle(ServerCallContext context)
        {
            return BJCyclesRepository.Get(context.GetUser());
        }
    }
}
