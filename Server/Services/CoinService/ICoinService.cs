
namespace HistoCoin.Server.Services.CoinService
{
    using System.Collections.Generic;
    using static HistoCoin.Server.Infrastructure.Constants;
    
    public interface ICoinService
    {
        IList<CoinModel> GetAll();

        Currencies BaseCurrency { get; set; }

        CoinModel GetById(int id);

        int Add(CoinModel record);

        void Update(CoinModel record);

        void Delete(int id);
    }
}
