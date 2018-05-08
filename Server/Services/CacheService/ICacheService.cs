
namespace HistoCoin.Server.Services.CacheService
{
    using System.Collections.Generic;
    using HistoCoin.Server.Infrastructure.Models;

    public interface ICacheService<T>
    {
        string Username { get; set; }

        string StorageLocation { get; set; }

        Cache<T> Cache { get; set; }
        
        Result Store();

        Result Load();

        Result Cleanup();

        IEnumerable<Cache<T>> PollHistoricalCache();
    }
}