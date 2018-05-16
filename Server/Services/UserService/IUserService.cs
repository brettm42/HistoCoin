
namespace HistoCoin.Server.Services.UserService
{
    using System.Collections.Generic;
    using HistoCoin.Server.Infrastructure.Interfaces;
    using HistoCoin.Server.Infrastructure.Models;

    public interface IUserService
    {
        IEnumerable<IUser> GetAllServiceUsers();

        IUser GetServiceUser(Credential credentials);

        IUser GetServiceUser(int userId, Credential credentials);

        Result AddUser(IUser newUser);
        
        Result RemoveUser(int userId, Credential credentials);

        string GetUserStoreCacheLocation(int userId, Credential credentials);

        string GetUserStoreCacheLocation(IUser user, Credential credentials);
    }
}