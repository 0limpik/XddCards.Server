using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xdd.Model.Cycles.BlackJack;
using Xdd.Model.Games;
using Xdd.Model.Games.BlackJack;
using Xdd.Model.Games.BlackJack.Users;

namespace XddCards.Server.Model.Cycles.BlackJack
{
    public class HandModel
    {
        public event Action<ICard> OnCardAdd
        {
            add => hand.OnCardAdd += value;
            remove => hand.OnCardAdd -= value;
        }
        public event Action<GameResult> OnResult
        {
            add => hand.OnResult += value;
            remove => hand.OnResult -= value;
        }

        public int Id;

        public UserModel owner;

        public bool CanTurn => hand.CanTurn;

        public IEnumerable<int> Scores => hand.Scores;

        public PlayerStatus? Status => hand.Status;

        private IHand hand;

        public HandModel(IHand hand)
        {
            this.hand = hand;
        }

        public void Take(IUser user)
        {
            user.Take(hand);
        }

        public void Release(IUser user)
        {
            user.Release(hand);
        }

        public ValueTask<bool> Hit()
           => hand.Hit();

        public ValueTask Stand()
            => hand.Stand();

        public ValueTask DoubleUp()
            => hand.DoubleUp();

    }
}
