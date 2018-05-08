
namespace HistoCoin.Server.Infrastructure.Models
{
    using System.Collections.Generic;

    public class JsonEntry
    {
        public string Key { get; set; }

        public Dictionary<string, string> Items { get; set; }
    }
}
