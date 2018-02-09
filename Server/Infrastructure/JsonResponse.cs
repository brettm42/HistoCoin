
namespace HistoCoin.Server.Infrastructure
{
    using System.Collections.Generic;

    public class JsonResponse
    {
        public string Key { get; set; }

        public Dictionary<string, JsonEntry> Response { get; set; }
    }
}
