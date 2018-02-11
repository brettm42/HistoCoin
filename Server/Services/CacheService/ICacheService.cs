
namespace HistoCoin.Server.Services.CacheService
{
    using HistoCoin.Server.Infrastructure;

    public interface ICacheService<T>
    {
        string Username { get; set; }

        string StorageLocation { get; set; }

        Cache<T> Cache { get; set; }

        Result Store();

        Result Load();
    }
}