
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
            if (userId == DebugUserId)
            {
                return new User
                {
                    Id = DebugUserId,
                    Email = new SecureString().AppendString("dev@histocoin.com"),
                    Password = new SecureString().AppendString("devDebugTest"),
                    Username = new SecureString().AppendString("debug"),
                    LocalCache = new SecureString().AppendString(DefaultCacheStoreLocation),
                    LastLoginTime = DateTimeOffset.Now - TimeSpan.FromHours(new Random().NextDouble()),
                };
            }

            return default;
        }

        public Result AddUser(IUser newUser)
        {
            if (newUser.Id == DebugUserId)
            {
                return new Result(false, $"Adding user with ID of {DebugUserId} not supported.");
            }

            var json = JsonConvert.SerializeObject(newUser, Formatting.Indented);

            var filename = DefaultServiceUserFilename.Replace(DefaultServiceUserPlaceholder, newUser.Id.ToString());
            var filePath = Path.Combine(this._localUserStoreLocation, filename);

            try
            {
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                }

                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                return new Result(false, ex.ToString());
            }

            return File.Exists(filePath)
                ? new Result(true, filePath)
                : new Result(false, default);
        }

        public Result RemoveUser(int userId)
        {
            if (userId == DebugUserId)
            {
                return new Result(false, $"ID of {DebugUserId} is not supported.");
            }

            var filename = DefaultServiceUserFilename.Replace(DefaultServiceUserPlaceholder, userId.ToString());
            var filePath = Path.Combine(this._localUserStoreLocation, filename);

            try
            {
                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                return new Result(false, ex.ToString());
            }

            return File.Exists(filePath)
                ? new Result(false, "Unable to delete user file.")
                : new Result(true, $"User {userId} deleted.");
        }

        public string GetUserStoreCacheLocation(int userId)
        {
            if (userId == DebugUserId)
            {
                return default;
            }

            var filename = DefaultServiceUserFilename.Replace(DefaultServiceUserPlaceholder, userId.ToString());
            var filePath = Path.Combine(this._localUserStoreLocation, filename);

            return File.Exists(filePath)
                ? filePath
                : default;
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
