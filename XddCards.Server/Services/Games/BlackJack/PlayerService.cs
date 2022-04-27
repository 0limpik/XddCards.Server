using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using XddCards.Grpc.Games.BlackJack;
using XddCards.Server.Mappers.Cycles.BlackJack;
using XddCards.Server.Model.Games.BlackJack;
using XddCards.Server.Repositories.Games.BlackJack;

namespace XddCards.Server.Services.Games.BlackJack
{
    public class PlayerService : PlayerGrpc.PlayerGrpcBase
    {
        public override Task<CanTurnReply> CanTurn(Player request, ServerCallContext context)
        {
            var player = GetPlayer(context, request.Id);
            return Task.FromResult(new CanTurnReply { Value = player.CanTurn });
        }

        public override Task<ScoresReply> GetScores(Player request, ServerCallContext context)
        {
            var player = GetPlayer(context, request.Id);

            var reply = new ScoresReply();
            player.GetScores().ToList().ForEach(x => reply.Scores.Add(x));

            return Task.FromResult(reply);
        }

        public override Task<StatusReply> GetStatus(Player request, ServerCallContext context)
        {
            var player = GetPlayer(context, request.Id);
            return Task.FromResult(new StatusReply { Status = player.GetStatus().Map() });
        }

        public override Task<HitReply> Hit(Player request, ServerCallContext context)
        {
            var player = GetPlayer(context, request.Id);
            var canTurn = player.Hit();
            return Task.FromResult(new HitReply { CanTurn = canTurn });
        }

        public override Task<StandReply> Stand(Player request, ServerCallContext context)
        {
            var player = GetPlayer(context, request.Id);
            player.Stand();
            return Task.FromResult(new StandReply());
        }

        private GameModel GetGame(ServerCallContext context)
            => GamesRepository.Get(context.GetUser());

        private PlayerModel GetPlayer(ServerCallContext context, int id)
            => GetGame(context).playerModels.First(x => x.Id == id);
    }
}
