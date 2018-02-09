
namespace HistoCoin.Server.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using Newtonsoft.Json;
    using HistoCoin.Server.Infrastructure;
    using static HistoCoin.Server.Infrastructure.Constants;

    public static class DataFetcher
    {
        internal const string CryptoCompareApi = "https://min-api.cryptocompare.com/data/";

        internal static readonly Dictionary<string, (double Count, double PricePer)> CurrencyList =
            new Dictionary<string, (double Count, double PricePer)>
            {
                { "BTC", (0.00820034, 11292.57) },
                { "ETH", (0.09955317, 974.85) },
                { "LTC", (0.504, 244.41) },
                { "BNB", (7.05380502, 12.9) },
                { "VEN", (10.996, 4.90) },
                { "XLM", (181.849, 0.4301) },
                { "ICX", (11.386, 5.60) },
                { "IOTA", (23.0, 2.44) },
                { "TRX", (4, 0.07173)},
            };

        public static IEnumerable<(string Handle, double Count)> BuildCurrenciesPair()
        {
            // todo: load from config - handles & current wallet total
            foreach (var entry in CurrencyList)
            {
                yield return (entry.Key, entry.Value.Count);
            }
        }

        public static IEnumerable<(string Handle, double Count, double StartingValue)> BuildCurrencies()
        {
            // todo: load from config - handles & current wallet total
            foreach (var entry in CurrencyList)
            {
                yield return (entry.Key, entry.Value.Count, entry.Value.PricePer);
            }
        }

        public static double CalculateDelta(string handle, double currentValue, Currencies currency)
        {
            switch (currency)
            {
                case Currencies.USD:
                    return Math.Round(currentValue - CurrencyList?[handle].PricePer ?? 0, 4);
                case Currencies.BTC:
                    return Math.Round(BtcToUsd(currentValue) - CurrencyList?[handle].PricePer ?? 0, 8);
                case Currencies.ETH:
                    return Math.Round(EthToUsd(currentValue) - CurrencyList?[handle].PricePer ?? 0, 6);
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
            var (_, Result) = DataFetcher.FetchComparison(nameof(Currencies.BTC));

            return btcValue * double.Parse(Result.Contents[nameof(Currencies.USD)]);
        }

        private static double EthToUsd(double ethValue)
        {
            var (_, Result) = DataFetcher.FetchComparison(nameof(Currencies.ETH));

            return ethValue * double.Parse(Result.Contents[nameof(Currencies.USD)]);
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
