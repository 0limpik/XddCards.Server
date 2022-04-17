using System.Collections.Generic;
using System.Linq;
using XddCards.Server.Model;
using XddCards.Server.Model.Games.BlackJack;

namespace XddCards.Server.Repositories.Games.BlackJack
{
    public class GamesRepository
    {
        private static List<GameModel> games = new();

        public static GameModel Get(User user)
            => games.Single(x => x.creator == user);

        public static GameModel Create(User creator)
        {
            var data = new GameModel
            {
                creator = creator,
            };
            games.Add(data);

            return data;
        }
    }
}
