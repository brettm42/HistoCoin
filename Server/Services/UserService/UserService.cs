
namespace HistoCoin.Server.Services.UserService
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security;
    using Newtonsoft.Json;
    using HistoCoin.Server.Infrastructure.Extensions;
    using HistoCoin.Server.Infrastructure.Interfaces;
    using HistoCoin.Server.Infrastructure.Models;
    using static HistoCoin.Server.Infrastructure.Constants;

    public class UserService : IUserService
    {
        private readonly string _localUserStoreLocation;

        public UserService()
        {
        }

        public UserService(string userStoreLocation)
        {
            this._localUserStoreLocation = userStoreLocation;
        }

        public IEnumerable<IUser> GetAllServiceUsers()
        {
            if (Directory.Exists(this._localUserStoreLocation))
            {
                return 
                    Directory.EnumerateFiles(
                        this._localUserStoreLocation,
                        $"*.{DefaultServiceUserExtension}",
                        SearchOption.TopDirectoryOnly)
                    .Select(LoadUser);
            }

            return default;
        }

        public IUser GetServiceUser(int userId)
        {
            // debug user ID
            if (userId == 0)
            {
                return new User
                {
                    Id = 0,
                    Email = new SecureString().AppendString("dev@histocoin.com"),
                    Password = new SecureString().AppendString("devDebugTest"),
                    Username = new SecureString().AppendString("debug"),
                    LocalCache = new SecureString().AppendString(DefaultCacheStoreLocation),
                    LastLoginTime = DateTimeOffset.Now - TimeSpan.FromMinutes(37),
                };
            }

            return default;
        }

        public Result AddUser(IUser newUser)
        {
            return new Result(false, default);
        }

        public Result RemoveUser(int userId)
        {
            return new Result(false, default);
        }

        public string GetUserStoreCacheLocation(int userId)
        {
            return default;
        }

        public string GetUserStoreCacheLocation(IUser user)
        {
            return user.LocalCache.ToString();
        }

        private static IUser LoadUser(string filePath)
        {
            var json = File.ReadAllText(filePath);

            return JsonConvert.DeserializeObject<User>(json);
        }
    }
}
