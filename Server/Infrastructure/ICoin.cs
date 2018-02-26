
namespace HistoCoin.Server.Infrastructure
{
    using static HistoCoin.Server.Infrastructure.Constants;

    public interface ICoin
    {
        int Id { get; set; }

        string Handle { get; set; }

        double Count { get; set; }

        Currencies BaseCurrency { get; set; }

        double StartingValue { get; set; }

        double CurrentValue { get; set; }

        double Worth { get; }

        double Delta { get; }

        History History { get; set; }
    }
}
