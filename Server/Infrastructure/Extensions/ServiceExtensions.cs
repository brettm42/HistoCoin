
namespace HistoCoin.Server.Infrastructure.Extensions
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;
    using HistoCoin.Server.Services.CacheService;
    using HistoCoin.Server.Services.CoinService;
    using HistoCoin.Server.Services.CurrencyService;
    using HistoCoin.Server.Services.UserService;
    using static HistoCoin.Server.Infrastructure.Constants;

    public static class ServiceExtensions
    {
        public static IServiceCollection AddUserService(this IServiceCollection services, string userStoreLocation)
        {
            //services
            //    .AddSingleton<IUserService, UserService>(
            //        service => 
            //            new UserService(userStoreLocation ?? DefaultUserStoreLocation));

            services
                .AddSingleton<IUserService, UserService>();

            return services;
        }

        public static IServiceCollection AddCacheService(this IServiceCollection services, int userId)
        {
            //var userService = services.GetRequiredService<IUserService>();

            //var cacheService =
            //    new CacheService<ConcurrentBag<Currency>>()
            //        .AddUserService(
            //            userService,
            //            userService.GetServiceUser(userId < 0 ? DebugUserId : userId));

            //services
            //    .AddSingleton<ICacheService<ConcurrentBag<Currency>>, CacheService<ConcurrentBag<Currency>>>(
            //        service => cacheService);

            services
                .AddSingleton<ICacheService<ConcurrentBag<Currency>>, CacheService<ConcurrentBag<Currency>>>();

            return services;
        }

        public static IServiceCollection AddCoinService(this IServiceCollection services)
        {
            //var cacheService = services.GetRequiredService<ICacheService<ConcurrentBag<Currency>>>();

            //var coinService =
            //    new CoinService()
            //        .AddCacheService(cacheService);

            //services
            //    .AddSingleton<ICoinService, CoinService>(
            //        service => coinService);

            services
                .AddSingleton<ICoinService, CoinService>();

            return services;
        }

        public static IServiceCollection AddCurrencyService(this IServiceCollection services)
        {
            //var cacheService = services.GetRequiredService<ICacheService<ConcurrentBag<Currency>>>();

            //var coinService = services.GetRequiredService<ICoinService>();

            //var currencyService =
            //    new CurrencyService()
            //        .AddCacheService(cacheService)
            //        .AddCoinService(coinService);

            //services
            //    .AddSingleton<ICurrencyService, CurrencyService>(
            //        service => currencyService);

            services
                .AddSingleton<ICurrencyService, CurrencyService>();

            return services;
        }
    }
}
