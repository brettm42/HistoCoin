
namespace HistoCoin.Server.Services.CacheService
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Newtonsoft.Json;
    using HistoCoin.Server.Infrastructure.Interfaces;
    using HistoCoin.Server.Infrastructure.Models;
    using HistoCoin.Server.Services.UserService;
    using static HistoCoin.Server.Infrastructure.Constants;
    
    public class CacheService<T> : ICacheService<T>
    {
        public CacheService()
        {
            this.Username = CacheService<T>.GetUsername();
        }

        public CacheService(string cacheLocation) : this()
        {
            this.StorageLocation = Path.GetFullPath(cacheLocation);
            this.Load();
        }

        public CacheService(string cacheLocation, T store) : this(cacheLocation)
        {
            this.Cache = new Cache<T>(store);
        }

        public CacheService(IUserService userService) : this()
        {
            this.AddUserService(
                userService, 
                userService.GetServiceUser(DebugUserId));
        }

        public string Username { get; set; }

        public string StorageLocation { get; set; }

        public Cache<T> Cache { get; set; }
        
        public Result Store()
        {
            if (this.StorageLocation is null)
            {
                return new Result(false, "Storage path is null!");
            }

            if (!Directory.Exists(this.StorageLocation))
            {
                Directory.CreateDirectory(this.StorageLocation);
            }

            var json = JsonConvert.SerializeObject(this.Cache.Get(), Formatting.Indented);

            try
            {
                var filename =
                    Path.Combine(
                        this.StorageLocation,
                        $"{DefaultCacheFilename.Replace(DefaultCacheDatePlaceholder, DateTime.Now.ToOADate().ToString(CultureInfo.InvariantCulture))}");

                File.WriteAllText(filename, json);

                return File.Exists(filename)
                    ? new Result(true, $"Saved to {filename}")
                    : new Result(false, $"Cache not saved to {filename}!");
            }
            catch (Exception ex)
            {
                return new Result(false, ex.ToString());
            }
        }

        public Result Load()
        {
            if (this.StorageLocation is null)
            {
                return new Result(false, "Storage path is null!");
            }
            
            try
            {
                if (!Directory.Exists(this.StorageLocation))
                {
                    Directory.CreateDirectory(this.StorageLocation);
                }

                var latestStore =
                    Directory.EnumerateFiles(
                            this.StorageLocation, $"*.{DefaultCacheExtension}", SearchOption.AllDirectories)
                        .OrderBy(i => i)
                        .LastOrDefault();

                if (string.IsNullOrWhiteSpace(latestStore))
                {
                    this.Cache = new Cache<T>(default);

                    return new Result(false, $"Cache at {this.StorageLocation} is null!");
                }

                var json = File.ReadAllText(latestStore);

                this.Cache =
                    new Cache<T>(
                        JsonConvert.DeserializeObject<T>(json));

                return this.Cache.Get() != null
                    ? new Result(true, $"Loaded cache from {latestStore}")
                    : new Result(false, $"Cache at {latestStore} is null!");
            }
            catch (Exception ex)
            {
                return new Result(false, ex.ToString());
            }
        }

        public Result Cleanup()
        {
            if (this.StorageLocation is null)
            {
                return new Result(false, "Storage path is null!");
            }

            try
            {
                if (!Directory.Exists(this.StorageLocation))
                {
                    return new Result(true, $"Cache at {this.StorageLocation} is clean");
                }

                var legacyCache =
                    Directory.EnumerateFiles(
                            this.StorageLocation, $"*.{DefaultCacheExtension}", SearchOption.AllDirectories)
                        .OrderBy(i => i);

                foreach (var file in legacyCache.Skip(MaxCachedFiles))
                {
                    File.Delete(file);
                }

                return new Result(true, $"Cache at {this.StorageLocation} is clean");
            }
            catch (Exception ex)
            {
                return new Result(false, ex.ToString());
            }
        }

        public CacheService<T> AddUserService(IUserService userService, IUser user)
        {
            if (userService is null || user is null)
            {
                return this;
            }

            //userService.AddUser(user);

            this.StorageLocation =
                Path.Combine(
                    DefaultCacheStoreLocation,
                    userService.GetUserStoreCacheLocation(user));

            this.Load();

            return this;
        }

        public IEnumerable<Cache<T>> PollHistoricalCache()
        {
            if (this.StorageLocation is null || !Directory.Exists(this.StorageLocation))
            {
                yield return default;
            }

            var legacyStore =
                Directory.EnumerateFiles(
                        this.StorageLocation, $"*.{DefaultCacheExtension}", SearchOption.AllDirectories)
                    .OrderBy(i => i)
                    .Take(MaxCachedFiles);

            foreach (var store in legacyStore)
            {
                if (string.IsNullOrWhiteSpace(store))
                {
                    yield return default;
                }

                Cache<T> cache;
                try
                {
                    var json = File.ReadAllText(store);

                    cache =
                        new Cache<T>(
                            JsonConvert.DeserializeObject<T>(json));
                }
                catch
                {
                    cache = null;
                }

                yield return cache;
            }
        }

        private static string GetUsername() => Environment.MachineName + @"\" + Environment.UserDomainName;
    }
}