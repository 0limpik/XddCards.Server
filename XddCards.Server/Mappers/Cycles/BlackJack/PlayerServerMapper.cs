using System;
using static XddCards.Grpc.Cycles.BlackJack.Controllers.OnResultReply.Types;
using static XddCards.Grpc.Games.BlackJack.StatusReply.Types;

namespace XddCards.Server.Mappers.Cycles.BlackJack
{
    public static class PlayerServerMapper
    {
        public static Grpc.Games.BlackJack.OnResultReply.Types.GameResult Map(this Xdd.Model.Games.BlackJack.GameResult result) =>
            result switch
            {
                Xdd.Model.Games.BlackJack.GameResult.Win => Grpc.Games.BlackJack.OnResultReply.Types.GameResult.Win,
                Xdd.Model.Games.BlackJack.GameResult.Lose => Grpc.Games.BlackJack.OnResultReply.Types.GameResult.Lose,
                Xdd.Model.Games.BlackJack.GameResult.Push => Grpc.Games.BlackJack.OnResultReply.Types.GameResult.Push,
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

        public static GameResult MapToController(this Xdd.Model.Games.BlackJack.GameResult result) =>
            result switch
            {
                Xdd.Model.Games.BlackJack.GameResult.Win => GameResult.Win,
                Xdd.Model.Games.BlackJack.GameResult.Lose => GameResult.Lose,
                Xdd.Model.Games.BlackJack.GameResult.Push => GameResult.Push,
                _ => throw new ArgumentException(),
            };
    }
}
