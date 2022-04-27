using Xdd.Model.Cycles.BlackJack.Controllers;
using XddCards.Server.Base;

namespace XddCards.Server.Model.Cycles.BlackJack.Controllers
{
    public class BetControllerModel
    {
        private IBetController controller;

        public CustomAwaiter<(UserModel user, decimal amount)> OnBet = new();

        public BetControllerModel(IBetController controller)
        {
            this.controller = controller;
        }

        public void OnBetInvoke(UserModel user, decimal amount)
        {
            OnBet.Execute((user, amount));
        }
    }
}
