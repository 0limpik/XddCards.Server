using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Xdd.Model.Games;
using XddCards.Grpc.Games;
using XddCards.Grpc.Games.BlackJack;
using XddCards.Server.Mappers.Cycles.BlackJack;
using XddCards.Server.Model.Games.BlackJack;
using XddCards.Server.Repositories.Games.BlackJack;
using static XddCards.Grpc.Games.CardMessage.Types;
using ModelRanks = Xdd.Model.Enums.Ranks;
using ModelSuits = Xdd.Model.Enums.Suits;

namespace XddCards.Server.Services.Games.BlackJack
{
    public class GameService : GameGrpc.GameGrpcBase
    {
        public override async Task OnGameEnd(GameEndRequest request, IServerStreamWriter<GameEndReply> responseStream, ServerCallContext context)
        {
            var game = GetGame(context);

            while (true)
            {
                await game.OnGameEnd;

                Trace.WriteLine("OnGameEnd");
                await responseStream.WriteAsync(new GameEndReply());
            }
        }

        public override async Task OnDillerUpHiddenCard(OnDillerUpHiddenCardRequest request, IServerStreamWriter<OnDillerUpHiddenCardReply> responseStream, ServerCallContext context)
        {
            var game = GetGame(context);

            while (true)
            {
                var card = await game.OnDillerUpHiddenCard;

                await responseStream.WriteAsync(new OnDillerUpHiddenCardReply { Card = card.Map() });
            }
        }

        public override async Task OnCardAdd(OnCardAddRequest request, IServerStreamWriter<OnCardAddReply> responseStream, ServerCallContext context)
        {
            var game = GetGame(context);

            do
            {
                var onCardAdd = await game.OnCardAdd;

                Trace.WriteLine(onCardAdd);

                var response = new OnCardAddReply
                {
                    Player = new Player { Id = onCardAdd.player.Id },
                    Card = onCardAdd.card.Map()
                };

                await responseStream.WriteAsync(response);
            } while (true);
        }

        public async override Task OnResult(OnResultRequest request, IServerStreamWriter<OnResultReply> responseStream, ServerCallContext context)
        {
            var game = GetGame(context);

            do
            {
                var onResult = await game.OnResult;

                var scores = new ScoresReply();

                foreach (var score in onResult.player.GetScores())
                {
                    scores.Scores.Add(score);
                }

                var response = new OnResultReply
                {
                    Player = new Player { Id = onResult.player.Id },
                    Status = new StatusReply { Status = onResult.player.GetStatus().Map() },
                    Scores = scores,
                    Result = onResult.result.Map(),
                };

                await responseStream.WriteAsync(response);
            } while (true);
        }

        public override Task<IsGameReply> IsGame(IsGameRequest request, ServerCallContext context)
        {
            var game = GetGame(context);

            return Task.FromResult(new IsGameReply { Value = game.IsGame });
        }

        ///-------///
        ///Players///
        ///-------///

        public override Task<PlayersReply> Players(PlayersRequest request, ServerCallContext context)
        {
            var game = GetGame(context);

            var reply = new PlayersReply();
            foreach (var player in game.playerModels.Where(x => x != game.dealerModel))
            {
                reply.Players.Add(new Player { Id = player.Id });
            }

            return Task.FromResult(reply);
        }

        public override Task<DealerReply> Dealer(DealerRequest request, ServerCallContext context)
        {
            var game = GetGame(context);

            var player = new Player { Id = game.dealerModel.Id };

            return Task.FromResult(new DealerReply { Player = player });
        }

        public override Task<InitReply> Init(InitRequest request, ServerCallContext context)
        {
            var game = GetGame(context);

            game.Init(request.PlayerCount);

            return Task.FromResult(new InitReply());
        }

        public override Task<StartReply> Start(StartRequest request, ServerCallContext context)
        {
            Trace.WriteLine("Start GAME");
            var game = GetGame(context);

            game.Start();

            return Task.FromResult(new StartReply());
        }

        private GameModel GetGame(ServerCallContext context)
            => GamesRepository.Get(context.GetUser());
    }


    public static class Cards
    {
        public static CardMessage Map(this ICard card)
            => new CardMessage { Rank = card.rank.Map(), Suit = card.suit.Map() };

        public static Ranks Map(this ModelRanks rank) =>
            rank switch
            {
                ModelRanks.Ace => Ranks.Ace,
                ModelRanks.Two => Ranks.Two,
                ModelRanks.Three => Ranks.Three,
                ModelRanks.Four => Ranks.Four,
                ModelRanks.Five => Ranks.Five,
                ModelRanks.Six => Ranks.Six,
                ModelRanks.Seven => Ranks.Seven,
                ModelRanks.Eight => Ranks.Eight,
                ModelRanks.Nine => Ranks.Nine,
                ModelRanks.Ten => Ranks.Ten,
                ModelRanks.Jack => Ranks.Jack,
                ModelRanks.Queen => Ranks.Queen,
                ModelRanks.King => Ranks.King,
                _ => throw new ArgumentException(),
            };

        public static Suits Map(this ModelSuits suit)
            => suit switch
            {
                ModelSuits.Clubs => Suits.Clubs,
                ModelSuits.Diamonds => Suits.Diamonds,
                ModelSuits.Hearts => Suits.Hearts,
                ModelSuits.Spades => Suits.Spades,
                _ => throw new ArgumentException(),
            };
    }
}
