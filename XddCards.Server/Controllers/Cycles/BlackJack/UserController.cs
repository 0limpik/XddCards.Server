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
    public class UserController : UserGrpc.UserGrpcBase
    {
        public override Task<CashResponse> GetCash(CashRequest request, ServerCallContext context)
        {
            var user = GetUser(context);

            return Task.FromResult(new CashResponse { Cash = user.Cash.ToString() });
        }

        public override Task<BetResponse> Bet(BetRequest request, ServerCallContext context)
        {
            var user = GetUser(context);

            user.Bet(decimal.Parse(request.Amount));

            return Task.FromResult(new BetResponse());
        }

        public override Task<TakeHandResponse> TakeHand(TakeHandRequest request, ServerCallContext context)
        {
            var user = GetUser(context);

            var hand = BJCyclesRepository.Get(context.GetUser()).hands.Single(x => x.Id == request.Hand.Id);

            user.Take(hand);

            return Task.FromResult(new TakeHandResponse());
        }

        public override Task<ReleaseHandResponse> ReleaseHand(ReleaseHandRequest request, ServerCallContext context)
        {
            var user = GetUser(context);

            user.Release(request.Hand.Id);

            return Task.FromResult(new ReleaseHandResponse());
        }

        private UserModel GetUser(ServerCallContext context)
        {
            var user = context.GetUser();
            return BJCyclesRepository.Get(user).Users.Single(x => x.User == user);
        }
    }
}
