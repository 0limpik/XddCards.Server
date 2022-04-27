using System;
using System.Collections.Generic;
using System.Linq;
using Xdd.Model.Cycles.BlackJack;
using XddCards.Server.Model.Cycles.BlackJack.Controllers;

namespace XddCards.Server.Model.Cycles.BlackJack
{
    public class UserModel
    {
        public User User;

        public int Id;

        private IUser user;

        public List<HandModel> handModels = new();

        private HandControllerModel handController;

        public decimal Cash => user.Cash;

        public UserModel(int id, IUser user, HandControllerModel handController)
        {
            this.Id = id;
            this.user = user;

            this.handController = handController;
        }

        public void Bet(decimal amount)
        {
            if (user.CanBet(amount))
                user.Bet(amount);
            else
                throw new ArgumentException();
        }

        public void Take(HandModel hand)
        {
            hand.Take(user);
            hand.owner = this;
            handModels.Add(hand);
            handController.HandsChange();
        }

        public void Release(int id)
        {
            var hand = handModels.Single(x => x.Id == id);
            hand.Release(user);
            hand.owner = null;
            handModels.Remove(hand);
            handController.HandsChange();
        }
    }
}
