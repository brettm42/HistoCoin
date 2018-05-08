
namespace HistoCoin.Server.Infrastructure.Models
{
    using System.Collections.Generic;

    public class JsonResponse
    {
        public string Key { get; set; }

        public Dictionary<string, JsonEntry> Response { get; set; }
    }
}
