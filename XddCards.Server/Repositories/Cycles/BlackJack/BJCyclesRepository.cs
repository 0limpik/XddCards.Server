using System.Collections.Generic;
using System.Linq;
using XddCards.Server.Model.Cycles.BlackJack;
using User = XddCards.Server.Model.User;

namespace XddCards.Server.Repositories.Cycles.BlackJack
{
    public static class BJCyclesRepository
    {
        private static List<BJCycleModel> cycles = new();
        private static int number = 0;

        public static BJCycleModel Get(User user)
            => cycles.Single(x => x.Users.Any(x => x.User == user) || x.creator == user);

        public static (BJCycleModel cycle, UserModel user) Create(User creator)
        {
            var cycle = new BJCycleModel(creator, ++number);
            cycles.Add(cycle);
            var user = cycle.AddUser(creator);

            return (cycle, user);
        }

        public static (BJCycleModel cycle, UserModel user) Connect(int id, User user)
        {
            var cycle = cycles.Single(x => x.Id == id);

            var userModel = cycle.AddUser(user);
            return (cycle, userModel);
        }

        public static IEnumerable<BJCycleModel> GetAll()
            => cycles;
    }
}
