
namespace HistoCoin.Server.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using Newtonsoft.Json;
    using HistoCoin.Server.Infrastructure.Models;
    using static HistoCoin.Server.Infrastructure.Constants;

    public static class DataFetcher
    {
        internal const string CryptoCompareApi = "https://min-api.cryptocompare.com/data/";
        
        public static IEnumerable<(string Handle, double Count)> BuildCurrenciesPair()
        {
            var walletPath = Path.Combine(DefaultCacheStoreLocation, DefaultWalletFilename);

            if (File.Exists(walletPath))
            {
                var currencyList =
                    JsonConvert.DeserializeObject<Dictionary<string, (double Count, double PricePer)>>(
                        File.ReadAllText(walletPath));

                foreach (var entry in currencyList ?? new Dictionary<string, (double Count, double PricePer)>())
                {
                    yield return (entry.Key, entry.Value.Count);
                }
            }
            else
            {
                yield return default;
            }
        }

        public static IEnumerable<(string Handle, double Count, double StartingValue)> BuildCurrencies()
        {
            var walletPath = Path.Combine(DefaultCacheStoreLocation, DefaultWalletFilename);

            if (File.Exists(walletPath))
            {
                var currencyList =
                    JsonConvert.DeserializeObject<Dictionary<string, (double Count, double PricePer)>>(
                        File.ReadAllText(walletPath));

                foreach (var entry in currencyList ?? new Dictionary<string, (double Count, double PricePer)>())
                {
                    yield return (entry.Key, entry.Value.Count, entry.Value.PricePer);
                }
            }
            else
            {
                yield return default;
            }
        }

        public static double CalculateDelta(double currentValue, double purchasePrice, Currencies currency)
        {
            const int Accuracy = 2;

            switch (currency)
            {
                case Currencies.USD:
                    return Math.Round(currentValue - purchasePrice, Digits[currency] + Accuracy);
                case Currencies.BTC:
                    return Math.Round(BtcToUsd(currentValue) - purchasePrice, Digits[currency] + Accuracy);
                case Currencies.ETH:
                    return Math.Round(EthToUsd(currentValue) - purchasePrice, Digits[currency] + Accuracy);
                default:
                    return 0;
            }
        }

        public static double CalculateDelta(string handle, double currentValue, double purchasePrice, Currencies currency)
        {
            const int Accuracy = 2;

            switch (currency)
            {
                case Currencies.USD:
                    return Math.Round(currentValue - purchasePrice, Digits[currency] + Accuracy);
                case Currencies.BTC:
                    return Math.Round(BtcToUsd(currentValue) - purchasePrice, Digits[currency] + Accuracy);
                case Currencies.ETH:
                    return Math.Round(EthToUsd(currentValue) - purchasePrice, Digits[currency] + Accuracy);
                default:
                    return 0;
            }
        }
        
        public static (string Handle, Packet<Dictionary<string, string>> Result) FetchComparison(string handle, bool fullResponse = false)
        {
            if (string.IsNullOrWhiteSpace(handle))
            {
                return (handle, null);
            }

            var request = 
                CryptoCompareApi 
                + "price" 
                + (fullResponse ? "full" : string.Empty) 
                + $"?fsym={handle}&tsyms={nameof(Currencies.BTC)},{nameof(Currencies.USD)}";
            
            return (handle, SendRequest(request).FirstOrDefault());
        }

        public static (string Handles, List<Packet<Dictionary<string, string>>> Results) FetchComparisons(IEnumerable<string> handles, bool fullResponse = false)
        {
            if (handles is null || !handles.Any())
            {
                return (null, null);
            }

            var request = 
                CryptoCompareApi 
                + "pricemulti" 
                + (fullResponse ? "full" : string.Empty)
                + "?fsyms=";

            request = 
                handles
                    .Where(handle => !string.IsNullOrWhiteSpace(handle))
                    .Aggregate(
                        request, 
                        (i, handle) => i + (handle.Trim() + ","));

            request = request.TrimEnd(',') + $"&tsyms={nameof(Currencies.BTC)},{nameof(Currencies.USD)}";

            var response = SendRequest(request);

            return 
                (response
                    .Aggregate(string.Empty, (i, packet) => i + $"{packet.Name},"), 
                response);
        }

        private static double BtcToUsd(double btcValue)
        {
            var (_, result) = DataFetcher.FetchComparison(nameof(Currencies.BTC));

            return btcValue * double.Parse(result.Contents[nameof(Currencies.USD)]);
        }

        private static double EthToUsd(double ethValue)
        {
            var (_, result) = DataFetcher.FetchComparison(nameof(Currencies.ETH));

            return ethValue * double.Parse(result.Contents[nameof(Currencies.USD)]);
        }

        private static List<Packet<Dictionary<string, string>>> SendRequest(string request)
        {
            var output = new List<Packet<Dictionary<string, string>>>();

            if (string.IsNullOrWhiteSpace(request))
            {
                return output;
            }

            try
            {
                var client = new HttpClient();
                var response = client.GetAsync(request).Result;

                if (!response.IsSuccessStatusCode)
                {
                    return output;
                }

                var data =
                    response.Content.ReadAsStringAsync().Result;

                if (data.Contains("Response\":\"Error"))
                {
                    return output;
                }

                if (data.Count(c => c == ':') < 3)
                {
                    var dataObject =
                        JsonConvert.DeserializeObject<Dictionary<string, string>>(data);

                    var key =
                        dataObject
                            .FirstOrDefault(
                                i => i.Value.Equals(1.ToString(), StringComparison.InvariantCultureIgnoreCase)).Key;

                    output.Add(
                        new Packet<Dictionary<string, string>>(key, dataObject));
                }
                else
                {
                    var dataObjects =
                        JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(data);

                    foreach (var item in dataObjects)
                    {
                        output.Add(
                            new Packet<Dictionary<string, string>>(item.Key, item.Value));
                    }
                }
            }
            catch
            {
                // ignore exceptions for now
            }

            return output;
        }
    }
}
