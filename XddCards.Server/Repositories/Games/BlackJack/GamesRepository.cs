using System.Collections.Generic;
using System.Linq;
using Xdd.Model.Games.BlackJack;
using static XddCards.Server.Services.Games.BlackJack.GameService;

namespace XddCards.Server.Repositories.Games.BlackJack
{
    public class GamesRepository
    {
        private static List<GameData> games = new();

        public static GameData Get(int id)
            => games.Single(x => x.id == id);

        public static int Create(int creatorId)
        {
            var id = games.Count + 1;

            var data = new GameData
            {
                creatorId = creatorId,
                id = id,
                game = new Game(),
            };
            games.Add(data);

            return id;
        }
    }

    public class GameData
    {
        public int creatorId;
        public int id;
        public Game game;
        public List<PlayerData> players = new();
    }
}
