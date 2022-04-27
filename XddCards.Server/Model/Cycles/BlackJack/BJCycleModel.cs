using System.Collections.Generic;
using System.Threading.Tasks;
using Xdd.Model.Cash;
using Xdd.Model.Cycles.BlackJack;
using XddCards.Server.Base;
using XddCards.Server.Model.Cycles.BlackJack.Controllers;

namespace XddCards.Server.Model.Cycles.BlackJack
{
    public class BJCycleModel
    {
        public int Id { get; init; }

        public User creator;

        public List<UserModel> Users { get; set; } = new();
        public List<HandModel> hands = new();
        private IBJCycleController cycle = BJCycleFabric.Create();

        public CustomAwaiter<(IState state, bool execute)> OnStateChange = new();
        public CustomAwaiter<string> OnSwitchMessage = new();

        public HandControllerModel handModel;
        public BetControllerModel betModel;
        public GameControllerModel gameModel;

        private Task cycleTask;

        public BJCycleModel(User creator, int id)
        {
            this.Id = id;
            this.creator = creator;
            cycle.HandController.OnChangeExecute += (execute) => OnStateChange.Execute((cycle.HandController, execute));
            cycle.BetController.OnChangeExecute += (execute) => OnStateChange.Execute((cycle.BetController, execute));
            cycle.GameController.OnChangeExecute += (execute) => OnStateChange.Execute((cycle.GameController, execute));

            handModel = new HandControllerModel(cycle.HandController);
            betModel = new BetControllerModel(cycle.BetController);
            gameModel = new GameControllerModel(cycle.GameController, Users);

            cycle.Init(6);

            hands.Clear();

            var ids = 0;

            foreach (var hand in handModel.Hands)
            {
                hands.Add(new HandModel(hand) { Id = ++ids });
            }
            gameModel.Init(hands);

            handModel.HandsChange();
        }

        static int testc = 0;

        public UserModel AddUser(User user)
        {
            var wallet = new Wallet(10000);

            var iUser = cycle.AddUser(wallet);

            var userModel = new UserModel(++testc, iUser, handModel) { User = user };
            Users.Add(userModel);

            cycleTask ??= Task.Run(Cycle);

            return userModel;
        }

        private async void Cycle()
        {
            await Task.Delay(5000);

            cycle.Start();

            while (Users.Count > 0)
            {
                await Task.Delay(5000);

                if (!TrySwitchState())
                {
                    await Task.Delay(5000);
                    cycle.Start();
                    continue;
                }

                await Task.Delay(5000);

                if (!TrySwitchState())
                {
                    await Task.Delay(5000);
                    cycle.Start();
                    continue;
                }

                await gameModel.OnGameEnd;
                await Task.Delay(5000);

                if (!TrySwitchState())
                {
                    await Task.Delay(5000);
                    cycle.Start();
                    continue;
                }
            }
        }

        private bool TrySwitchState()
        {
            if (cycle.CanSwitchState(out string message))
            {
                cycle.SwitchState();
                return true;
            }

            OnSwitchMessage.Execute(message);

            cycle.Reset();

            return false;
        }
    }
}
