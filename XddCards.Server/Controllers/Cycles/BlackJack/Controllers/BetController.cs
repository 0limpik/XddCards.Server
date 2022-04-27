using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using XddCards.Grpc.Cycles.BlackJack.Controllers;

namespace XddCards.Server.Controllers.Cycles.BlackJack.Controllers
{
    [Authorize]
    public class BetController : BetControllerGrpc.BetControllerGrpcBase
    {
        public override Task OnBet(OnBetRequest request, IServerStreamWriter<OnBetResponse> responseStream, ServerCallContext context)
        {
            return base.OnBet(request, responseStream, context);
        }
    }
}
