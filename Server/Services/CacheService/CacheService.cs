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
            this.Username = GetUsername();
            this.StorageLocation = Path.GetFullPath(cacheLocation);
            this.Load();
        }

        public CacheService(string cacheLocation, T store)
        {
            this.Username = GetUsername();
            this.StorageLocation = Path.GetFullPath(cacheLocation);
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

            if (!Directory.Exists(this.StorageLocation))
            {
                Directory.CreateDirectory(this.StorageLocation);
            }

            var json = JsonConvert.SerializeObject(this.Cache.Get(), Formatting.Indented);

            try
            {
                var filename = Path.Combine(this.StorageLocation, $"store_{DateTime.Now.ToOADate()}.hcc");

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
                    Directory.GetFiles(this.StorageLocation).LastOrDefault();

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

        private static string GetUsername() => Environment.MachineName + @"\" + Environment.UserDomainName;
    }
}