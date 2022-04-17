using System;
using System.Collections.Generic;
using Xdd.Model.Games;
using Xdd.Model.Games.BlackJack;
using Xdd.Model.Games.BlackJack.Users;

namespace XddCards.Server.Model.Games.BlackJack
{
    public class PlayerModel
    {
        public User owner;

        public int id;
        public IPlayer player { get; init; }

        public bool CanTurn => player.CanTurn;
        public IEnumerable<int> GetScores() => player.GetScores();
        public PlayerStatus? GetStatus() => player.GetStatus();

        public event Action<ICard> OnCardAdd
        {
            add => player.OnCardAdd += value;
            remove => player.OnCardAdd -= value;
        } 
        public event Action<GameResult> OnResult
        {
            add => player.OnResult += value;
            remove => player.OnResult -= value;
        }

        public PlayerModel(IPlayer player)
        {
            this.player = player;
        }

        public bool Hit()
            => player.Hit();

        public void Stand()
            => player.Stand();
    }
}
