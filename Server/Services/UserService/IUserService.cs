
namespace HistoCoin.Server.Services.UserService
{
    using System.Collections.Generic;
    using HistoCoin.Server.Infrastructure;

    public interface IUserService
    {
        List<IUser> GetAllServiceUsers();

        IUser GetServiceUser(int userId);

        Result AddUser(IUser newUser);

        Result RemoveUser(int userId);
    }
}