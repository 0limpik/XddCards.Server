using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using XddCards.Server.Users;

namespace XddCards.Server.Services.Users
{
    public class UserService : UserServiceGrpc.UserServiceGrpcBase
    {
        private class User
        {
            public string Id { get; set; }
        }

        private static List<User> users = new();

        public override Task<CreateGameReply> CreateGame(CreateGameRequest request, ServerCallContext context)
        {
            //GamesRepository.Create();
            return base.CreateGame(request, context);
        }
    }
}
