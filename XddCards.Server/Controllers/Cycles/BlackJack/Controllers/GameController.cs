using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using XddCards.Grpc.Cycles.BlackJack;
using XddCards.Grpc.Cycles.BlackJack.Controllers;
using XddCards.Server.Mappers.Cycles.BlackJack;
using XddCards.Server.Model.Cycles.BlackJack;
using XddCards.Server.Repositories.Cycles.BlackJack;
using XddCards.Server.Services.Games;
using XddCards.Server.Services.Games.BlackJack;

namespace XddCards.Server.Controllers.Cycles.BlackJack.Controllers
{
    [Authorize]
    public class GameController : GameControllerGrpc.GameControllerGrpcBase
    {
        public async override Task OnDillerUpHiddenCard(OnDillerUpHiddenCardRequest request, IServerStreamWriter<OnDillerUpHiddenCardReply> responseStream, ServerCallContext context)
        {
            var cycle = GetCycle(context);

            while (true)
            {
                var card = await cycle.gameModel.OnDillerUpHiddenCard.SetToken(context.CancellationToken);

                await responseStream.WriteAsync(new OnDillerUpHiddenCardReply { Card = card.Map() });
            }
        }

        public async override Task OnGameEnd(GameEndRequest request, IServerStreamWriter<GameEndReply> responseStream, ServerCallContext context)
        {
            var cycle = GetCycle(context);

            while (true)
            {
                await cycle.gameModel.OnGameEnd;

                await responseStream.WriteAsync(new GameEndReply());
            }
        }

        public async override Task OnCardAdd(OnCardAddRequest request, IServerStreamWriter<OnCardAddReply> responseStream, ServerCallContext context)
        {
            var cycle = GetCycle(context);

            while (true)
            {
                var message = await cycle.gameModel.OnCardAdd.SetToken(context.CancellationToken);

                await responseStream.WriteAsync(new OnCardAddReply
                {
                    Hand = new Hand { Id = message.hand.Id },
                    Card = message.card.Map(),
                    CanTurn = message.hand.CanTurn
                });
            }
        }

        public async override Task OnResult(OnResultRequest request, IServerStreamWriter<OnResultReply> responseStream, ServerCallContext context)
        {
            var cycle = GetCycle(context);

            while (true)
            {
                var onResult = await cycle.gameModel.OnResult.SetToken(context.CancellationToken);

                var scores = new Grpc.Games.BlackJack.ScoresReply();

                foreach (var score in onResult.hand.Scores)
                {
                    scores.Scores.Add(score);
                }

                var response = new OnResultReply
                {
                    Hand = new Hand { Id = onResult.hand.Id },
                    Status = new Grpc.Games.BlackJack.StatusReply { Status = onResult.hand.Status.Map() },
                    Scores = scores,
                    Result = onResult.result.MapToController(),
                };

                await responseStream.WriteAsync(response);
            }
        }

        public override Task<PlayersReply> Players(PlayersRequest request, ServerCallContext context)
        {
            var cycle = GetCycle(context);

            var reply = new PlayersReply();

            foreach (var hand in cycle.hands)
            {
                reply.Hands.Add(new Hand { Id = hand.Id });
            }

            return Task.FromResult(reply);
        }

        public override Task<DealerReply> Dealer(DealerRequest request, ServerCallContext context)
        {
            var cycle = GetCycle(context);

            return Task.FromResult(new DealerReply { Hand = new Hand { Id = cycle.gameModel.dealer.Id } });
        }

        private BJCycleModel GetCycle(ServerCallContext context)
        {
            return BJCyclesRepository.Get(context.GetUser());
        }
    }
}
