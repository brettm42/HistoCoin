namespace HistoCoin.Server.Services.CacheService
{
    using System;
    using System.IO;
    using System.Linq;
    using Newtonsoft.Json;
    using HistoCoin.Server.Infrastructure;
    
    public class CacheService<T> : ICacheService<T>
    {
        public CacheService(string cacheLocation)
        {
            this.Username = Environment.UserDomainName;

            this.StorageLocation = cacheLocation;

            this.Load();
        }

        public CacheService(string cacheLocation, T store)
        {
            this.Username = Environment.UserDomainName;

            this.StorageLocation = cacheLocation;

            this.Cache = new Cache<T>(store);
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

            var json = JsonConvert.SerializeObject(this.Cache);

            try
            {
                File.WriteAllText(this.StorageLocation, json);

                return File.Exists(this.StorageLocation) 
                    ? new Result(true, $"Saved to {this.StorageLocation}") 
                    : new Result(false, $"Cache not saved to {this.StorageLocation}!");
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
                var json = File.ReadAllText(this.StorageLocation);

                this.Cache =
                    JsonConvert.DeserializeObject<Cache<T>>(json);

                return this.Cache is null
                    ? new Result(true, $"Loaded cache from {this.StorageLocation}")
                    : new Result(false, $"Cache at {this.StorageLocation} is null!");
            }
            catch (Exception ex)
            {
                return new Result(false, ex.ToString());
            }
        }
    }
}