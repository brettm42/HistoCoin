﻿
namespace HistoCoin.Server.ViewModels.Dashboard
{
    using DotNetify.Routing;

    public class Currency
    {
        public string Handle { get; set; }

        public Route Route { get; set; }

        public string Value { get; set; }

        public string Count { get; set; }

        public string Worth { get; set; }
    }
}
