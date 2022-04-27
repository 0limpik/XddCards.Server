using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using XddCards.Grpc.Cycles.BlackJack;
using XddCards.Server.Model.Cycles.BlackJack;
using XddCards.Server.Repositories.Cycles.BlackJack;
using XddCards.Server.Services.Games;

namespace XddCards.Server.Controllers.Cycles.BlackJack
{
    [Authorize]
    public class HandController : HandGrpc.HandGrpcBase
    {
        public override Task<CantTurnReply> CanTurn(Hand request, ServerCallContext context)
        {
            var game = BJCyclesRepository.Get(context.GetUser());

            var user = GetUser(context);

            var hand = user.handModels.Single(x => x.Id == request.Id);

            return Task.FromResult(new CantTurnReply { CanTurn = hand.CanTurn });
        }

        public override async Task<DoubleUpReply> DoubleUp(Hand request, ServerCallContext context)
        {
            var user = GetUser(context);

            var hand = user.handModels.Single(x => x.Id == request.Id);

            await hand.DoubleUp();

            return new DoubleUpReply();
        }

        public override async Task<HitReply> Hit(Hand request, ServerCallContext context)
        {
            var user = GetUser(context);

            var hand = user.handModels.Single(x => x.Id == request.Id);

            var hit = await hand.Hit();

            return new HitReply { CanTurn = hit };
        }

        public override async Task<StandReply> Stand(Hand request, ServerCallContext context)
        {
            var user = GetUser(context);

            var hand = user.handModels.Single(x => x.Id == request.Id);

            await hand.Stand();

            return new StandReply();
        }

        private UserModel GetUser(ServerCallContext context)
        {
            var user = context.GetUser();
            return BJCyclesRepository.Get(context.GetUser()).Users.Single(x => x.User == user);
        }
    }
}
