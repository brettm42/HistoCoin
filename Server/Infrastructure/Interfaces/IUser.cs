
namespace HistoCoin.Server.Infrastructure.Interfaces
{
    using System;
    using HistoCoin.Server.Infrastructure.Models;

    public interface IUser
    {
        int Id { get; set; }

        DateTimeOffset LastLoginTime { get; set; }

        Securable Username { get; set; }

        Securable Email { get; set; }

        Securable Password { get; set; }

        string LocalCache { get; set; }
    }
}
