
namespace HistoCoin.Server.Services.UserService
{
    using System;
    using System.Security;
    using HistoCoin.Server.Infrastructure.Interfaces;
    using HistoCoin.Server.Infrastructure.Models;

    public class User : IUser
    {
        public int Id { get; set; }

        public DateTimeOffset LastLoginTime { get; set; }

        public Securable Username { get; set; }

        public Securable Email { get; set; }

        public Securable Password { get; set; }

        public string LocalCache { get; set; }
    }
}
