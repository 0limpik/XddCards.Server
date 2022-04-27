using Xdd.Model.Cycles.BlackJack;
using Xdd.Model.Cycles.BlackJack.Controllers;
using XddCards.Server.Base;

namespace XddCards.Server.Model.Cycles.BlackJack.Controllers
{
    public class HandControllerModel
    {
        private IHandController controller;

        public IHand[] Hands => controller.Hands;

        public CustomAwaiter OnHandsChange = new();

        public HandControllerModel(IHandController controller)
        {
            this.controller = controller;
        }

        public void HandsChange()
        {
            OnHandsChange.Execute();
        }
    }
}
