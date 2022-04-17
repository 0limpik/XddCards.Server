using System;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using XddCards.Grpc.Games.BlackJack;
using XddCards.Server.Model.Games.BlackJack;
using XddCards.Server.Repositories.Games.BlackJack;
using static XddCards.Grpc.Games.BlackJack.OnResultReply.Types;
using static XddCards.Grpc.Games.BlackJack.StatusReply.Types;

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
            => GetGame(context).playerModels.First(x => x.id == id);
    }

    public static class PlayerExtensions
    {
        public static GameResult Map(this Xdd.Model.Games.BlackJack.GameResult result) =>
            result switch
            {
                Xdd.Model.Games.BlackJack.GameResult.Win => GameResult.Win,
                Xdd.Model.Games.BlackJack.GameResult.Lose => GameResult.Lose,
                Xdd.Model.Games.BlackJack.GameResult.Push => GameResult.Push,
                _ => throw new ArgumentException(),
            };

        public static PlayerStatus Map(this Xdd.Model.Games.BlackJack.Users.PlayerStatus? status) =>
            status switch
            {
                Xdd.Model.Games.BlackJack.Users.PlayerStatus.Win => PlayerStatus.Win,
                Xdd.Model.Games.BlackJack.Users.PlayerStatus.Lose => PlayerStatus.Lose,
                Xdd.Model.Games.BlackJack.Users.PlayerStatus.Push => PlayerStatus.Push,
                Xdd.Model.Games.BlackJack.Users.PlayerStatus.Bust => PlayerStatus.Bust,
                null => PlayerStatus.Empty,
                _ => throw new ArgumentException(),
            };
    }
}
