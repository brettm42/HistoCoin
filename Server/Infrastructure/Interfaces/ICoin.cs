
namespace HistoCoin.Server.Infrastructure.Interfaces
{
    using static HistoCoin.Server.Infrastructure.Constants;
    using Newtonsoft.Json;

    public interface ICoin
    {
        int Id { get; set; }

        [JsonIgnore]
        bool IsModified { get; set; }

        string Handle { get; set; }

        double Count { get; set; }

        Currencies BaseCurrency { get; set; }

        double StartingValue { get; set; }

        double CurrentValue { get; set; }

        double Worth { get; }

        double Delta { get; }

        History History { get; set; }

        ICoin Update(ICoin coin);
    }
}
