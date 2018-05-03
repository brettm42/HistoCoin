
namespace HistoCoin.Server.Services.UserService
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using HistoCoin.Server.Infrastructure;

    public class UserService : IUserService
    {
        public UserService()
        {

        }

        public List<IUser> GetAllServiceUsers()
        {
            return default;
        }

        public IUser GetServiceUser(int userId)
        {
            return default;
        }

        public Result Load(string location)
        {
            return new Result(false, default);
        }

        public Result AddUser(IUser newUser)
        {
            return new Result(false, default);
        }

        public Result RemoveUser(int userId)
        {
            return new Result(false, default);
        }

        public string UserStoreCache { get; }
    }
}
