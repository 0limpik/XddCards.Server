using System.Collections.Generic;
using Xdd.Model.Games;
using Xdd.Model.Games.BlackJack;
using XddCards.Server.Base;

namespace XddCards.Server.Model.Games.BlackJack
{
    public class GameModel
    {
        public User creator;

        public List<PlayerModel> playerModels = new List<PlayerModel>();
        public PlayerModel dealerModel;

        private Game game = new Game();

        public CustomAwaiter OnGameEnd = new CustomAwaiter();
        public CustomAwaiter<ICard> OnDillerUpHiddenCard = new CustomAwaiter<ICard>();

        public CustomAwaiter<(PlayerModel player, ICard card)> OnCardAdd = new CustomAwaiter<(PlayerModel player, ICard card)>();
        public CustomAwaiter<(PlayerModel player, GameResult result)> OnResult = new CustomAwaiter<(PlayerModel player, GameResult result)>();

        public bool IsGame => game.isGame;

        public GameModel()
        {
            game.OnGameEnd += OnGameEnd.Execute;
            game.OnDillerUpHiddenCard += OnDillerUpHiddenCard.Execute;
        }

        public void Init(int playersCount)
        {
            game.Init(playersCount);

            playerModels.Clear();

            dealerModel = new PlayerModel(-1, game.dealer);
            Add(dealerModel);

            int ids = 0;
            foreach (var player in game.players)
            {
                var playerModel = new PlayerModel(++ids, player) { owner = creator };
                Add(playerModel);
            }

            void Add(PlayerModel playerModel)
            {
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
            model.Hit();
        }

        public void Stand(PlayerModel model)
        {
            model.Stand();
        }
    }
}