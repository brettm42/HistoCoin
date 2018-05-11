
namespace HistoCoin.Server.Services.UserService
{
    using System.Collections.Generic;
    using HistoCoin.Server.Infrastructure.Interfaces;
    using HistoCoin.Server.Infrastructure.Models;

    public interface IUserService
    {
        IEnumerable<IUser> GetAllServiceUsers();

        IUser GetServiceUser(int userId);

        Result AddUser(IUser newUser);

        Result RemoveUser(int userId);

        string GetUserStoreCacheLocation(int userId);
    }
}