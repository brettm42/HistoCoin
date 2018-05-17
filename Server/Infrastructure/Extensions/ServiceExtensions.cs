
namespace HistoCoin.Server.Infrastructure.Extensions
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;
    using HistoCoin.Server.Infrastructure.Models;
    using HistoCoin.Server.Services.CacheService;
    using HistoCoin.Server.Services.CoinService;
    using HistoCoin.Server.Services.CurrencyService;
    using HistoCoin.Server.Services.UserService;
    using static HistoCoin.Server.Infrastructure.Constants;

    public static class ServiceExtensions
    {
        public static IServiceCollection AddCurrencyServices(this IServiceCollection services)
        {
            var userService = 
                new UserService(DefaultUserStoreLocation);
            var cacheService = 
                new CacheService<ConcurrentBag<Currency>>()
                    .AddUserService(
                        userService,
                        new Credential(DebugUsername, $"{DebugUsername}_!#T3ST"));
            var coinService =
                new CoinService()
                    .AddCacheService(cacheService);
            var currencyService =
                new CurrencyService()
                    .AddCacheService(cacheService)
                    .AddCoinService(coinService);

            services
                .AddSingleton<IUserService, UserService>(
                    service => userService);
            services
                .AddSingleton<ICacheService<ConcurrentBag<Currency>>, CacheService<ConcurrentBag<Currency>>>(
                    service => cacheService);
            services
                .AddSingleton<ICoinService, CoinService>(
                    service => coinService);
            services
                .AddSingleton<ICurrencyService, CurrencyService>(
                    service => currencyService);

            return services;
        }

        public static IServiceCollection AddUserService(this IServiceCollection services, string userStoreLocation)
        {
            services
                .AddSingleton<IUserService, UserService>(
                    service =>
                        new UserService(userStoreLocation ?? DefaultUserStoreLocation));

            return services;
        }

        public static IServiceCollection AddCacheService(this IServiceCollection services, int userId)
        {
            services
                .AddSingleton<ICacheService<ConcurrentBag<Currency>>, CacheService<ConcurrentBag<Currency>>>();

            return services;
        }

        public static IServiceCollection AddCoinService(this IServiceCollection services)
        {
            services
                .AddSingleton<ICoinService, CoinService>();

            return services;
        }

        public static IServiceCollection AddCurrencyService(this IServiceCollection services)
        {
            services
                .AddSingleton<ICurrencyService, CurrencyService>();

            return services;
        }
    }
}
