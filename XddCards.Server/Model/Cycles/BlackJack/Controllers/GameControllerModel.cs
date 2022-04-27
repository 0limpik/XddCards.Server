using System.Collections.Generic;
using Xdd.Model.Cycles.BlackJack.Controllers;
using Xdd.Model.Games;
using Xdd.Model.Games.BlackJack;
using XddCards.Server.Base;

namespace XddCards.Server.Model.Cycles.BlackJack.Controllers
{
    public class GameControllerModel
    {
        private IGameController controller;
        public List<UserModel> users;
        public HandModel dealer;

        public CustomAwaiter OnGameEnd = new CustomAwaiter();
        public CustomAwaiter<ICard> OnDillerUpHiddenCard = new CustomAwaiter<ICard>();

        public CustomAwaiter<(HandModel hand, ICard card)> OnCardAdd = new();
        public CustomAwaiter<(HandModel hand, GameResult result)> OnResult = new();

        public GameControllerModel(IGameController controller, List<UserModel> users)
        {
            this.controller = controller;
            this.users = users;
            this.dealer = new HandModel(this.controller.DealerHand) { Id = -1 };

            dealer.OnCardAdd += (card) => OnCardAdd.Execute((dealer, card));
            dealer.OnResult += (result) => OnResult.Execute((dealer, result));

            this.controller.OnGameEnd += OnGameEnd.Execute;
            this.controller.OnDillerUpHiddenCard += OnDillerUpHiddenCard.Execute;
        }

        public void Init(List<HandModel> hands)
        {
            foreach (var model in hands)
            {
                model.OnCardAdd += (card) => OnCardAdd.Execute((model, card));
                model.OnResult += (result) => OnResult.Execute((model, result));
            }
        }
    }
}
