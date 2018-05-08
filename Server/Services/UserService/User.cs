
namespace HistoCoin.Server.Services.UserService
{
    using System;
    using System.Security;
    using HistoCoin.Server.Infrastructure.Interfaces;

    public class User : IUser
    {
        public int Id { get; set; }

        public DateTimeOffset LastLoginTime { get; set; }

        public SecureString Username { get; set; }

        public SecureString Email { get; set; }

        public SecureString Password { get; set; }

        public SecureString LocalCache { get; set; }
    }
}
