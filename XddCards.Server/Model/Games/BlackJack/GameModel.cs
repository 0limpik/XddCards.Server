using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Xdd.Model.Games;
using Xdd.Model.Games.BlackJack;

namespace XddCards.Server.Model.Games.BlackJack
{
    public class GameModel
    {
        public User creator;

        public List<PlayerModel> playerModels = new List<PlayerModel>();
        public PlayerModel dealerModel;

        private Game game = new Game();

        public CustomAwaiter OnGameEnd => new CustomAwaiter((x) => game.OnGameEnd += x);
        public CustomAwaiter<ICard> OnDillerUpHiddenCard = new CustomAwaiter<ICard>();

        public CustomAwaiter<(PlayerModel player, ICard card)> OnCardAdd = new CustomAwaiter<(PlayerModel player, ICard card)>();
        public CustomAwaiter<(PlayerModel player, GameResult result)> OnResult = new CustomAwaiter<(PlayerModel player, GameResult result)>();

        public bool IsGame => game.isGame;

        public GameModel()
        {
            game.OnDillerUpHiddenCard += OnDillerUpHiddenCard.Execute;
        }

        public void Init(int playersCount)
        {
            game.Init(playersCount);

            playerModels.Clear();

            dealerModel = new PlayerModel(game.dealer) { id = -1 };

            playerModels.Add(dealerModel);

            int ids = 0;
            foreach (var player in game.players)
            {
                var playerModel = new PlayerModel(player) { id = ++ids, owner = creator };
                playerModel.OnCardAdd += (card) => OnCardAdd.Execute((playerModel, card));
                playerModel.OnResult += (result) => OnResult.Execute((playerModel, result));
                playerModels.Add(playerModel);
            }
        }

        public void Start()
        {
            game.Start();
        }

        public void Hit(PlayerModel model)
        {
            model.player.Hit();
        }

        public void Stand(PlayerModel model)
        {
            model.player.Stand();
        }
    }

    public class CustomAwaiter<T> : INotifyCompletion
    {
        private Action continuation;
        private T value;

        public void Execute(T value)
        {
            this.value = value;
            continuation();
        }

        public bool IsCompleted => false;

        public T GetResult() => value;

        public void OnCompleted(Action continuation) { }

        public CustomAwaiter<T> GetAwaiter()
            => this;
    }

    public class CustomAwaiter : INotifyCompletion
    {
        private Action<Action> OnCompletedEvent;

        public CustomAwaiter(Action<Action> OnCompletedEvent)
        {
            this.OnCompletedEvent = OnCompletedEvent ?? throw new ArgumentException();
        }

        public bool IsCompleted => false;

        public void GetResult() { }

        public void OnCompleted(Action continuation)
        {
            OnCompletedEvent(continuation);
        }

        public CustomAwaiter GetAwaiter()
            => this;
    }
}