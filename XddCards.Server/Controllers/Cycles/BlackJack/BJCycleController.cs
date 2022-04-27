using System;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Xdd.Model.Cycles.BlackJack.Controllers;
using XddCards.Grpc.Cycles.BlackJack;
using XddCards.Server.Model.Cycles.BlackJack;
using XddCards.Server.Repositories.Cycles.BlackJack;
using XddCards.Server.Services.Games;
using State = XddCards.Grpc.Cycles.BlackJack.OnStateChangeResponse.Types.State;

namespace XddCards.Server.Controllers.Cycles.BlackJack
{

    [Authorize]
    public class BJCycleController : BJCycleGrpc.BJCycleGrpcBase
    {
        public async override Task OnStateChange(OnStateChangeRequest request, IServerStreamWriter<OnStateChangeResponse> responseStream, ServerCallContext context)
        {
            var cycle = GetCycle(context);

            while (!context.CancellationToken.IsCancellationRequested)
            {
                var message = await cycle.OnStateChange.SetToken(context.CancellationToken);

                State? state = null;
                if (message.state is IHandController)
                    state = State.Hand;
                if (message.state is IBetController)
                    state = State.Bet;
                if (message.state is IGameController)
                    state = State.Game;

                await responseStream.WriteAsync(new OnStateChangeResponse
                {
                    State = state ?? throw new ArgumentException(),
                    Execute = message.execute
                });
            }
        }

        public async override Task OnSwitchMessage(OnSwitchMessageRequest request, IServerStreamWriter<OnSwitchMessageResponse> responseStream, ServerCallContext context)
        {
            var cycle = GetCycle(context);

            while (!context.CancellationToken.IsCancellationRequested)
            {
                var message = await cycle.OnSwitchMessage.SetToken(context.CancellationToken);
                await responseStream.WriteAsync(new OnSwitchMessageResponse { Message = message });
            }
        }

        public override Task<InitResponse> Init(InitReqest request, ServerCallContext context)
        {
            var cycle = GetCycle(context);
            //cycle.Init(request.HandCount);

            return Task.FromResult(new InitResponse());
        }

        private BJCycleModel GetCycle(ServerCallContext context)
        {
            return BJCyclesRepository.Get(context.GetUser());
        }
    }
}
