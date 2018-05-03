
namespace HistoCoin.Server.Infrastructure
{
    using System;
    using System.Security;

    public interface IUser
    {
        int Id { get; set; }

        DateTimeOffset LastLoginTime { get; set; }

        SecureString Username { get; set; }

        SecureString Email { get; set; }

        SecureString Password { get; set; }

        SecureString LocalCache { get; set; }
    }
}
