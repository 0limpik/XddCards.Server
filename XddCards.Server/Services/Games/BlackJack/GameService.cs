using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Xdd.Model.Games.BlackJack.Users;
using XddCards.Server.Games.BlackJack;
using XddCards.Server.Repositories.Games.BlackJack;

namespace XddCards.Server.Services.Games.BlackJack
{
    public class GameService : GameServiceGrpc.GameServiceGrpcBase
    {
        public class PlayerData
        {
            public PlayerReply data;
            public IPlayer player;
        }

        public override Task<IsGameReply> IsGame(IsGameRequest request, ServerCallContext context)
            => Task.FromResult(new IsGameReply { Value = GetGame(request.Id).game.isGame });
        public override Task<PlayerReply> Dealer(DealerRequest request, ServerCallContext context)
            => Task.FromResult(new PlayerReply { Id = -1 });

        public override Task<PlayersReply> Players(PlayersRequest request, ServerCallContext context)
        {
            var game = GetGame(request.Id);

            var reply = new PlayersReply(); ;

            game.players.ForEach(x => reply.Players.Add(x.data));

            return Task.FromResult(reply);
        }

        public override Task<InitReply> Init(InitRequest request, ServerCallContext context)
        {
            var game = GetGame(request.Id);

            game.game.Init(request.PlayerCount);
            game.players.Clear();

            var playyer = game.game.players.ToArray();

            for (int i = 0; i < playyer.Length; i++)
            {
                game.players.Add(new PlayerData { data = new PlayerReply { Id = i }, player = playyer[i] });
            }

            return Task.FromResult(new InitReply());
        }

        public override Task<StartReply> Start(StartRequest request, ServerCallContext context)
        {
            var game = GetGame(request.Id);

            game.game.Start();

            return Task.FromResult(new StartReply());
        }

        public override Task<HitReply> Hit(PlayerReply request, ServerCallContext context)
        {
            var game = GetGame(request.Id);

            var data = game.players.First(x => x.data.Id == x.data.Id);

            game.game.Hit(data.player);

            return Task.FromResult(new HitReply());
        }

        public override Task<StandReply> Stand(PlayerReply request, ServerCallContext context)
        {
            var game = GetGame(request.Id);

            var data = game.players.First(x => x.data.Id == x.data.Id);

            game.game.Hit(data.player);

            return Task.FromResult(new StandReply());
        }

        private GameData GetGame(int id)
            => GamesRepository.Get(id);
    }
}
