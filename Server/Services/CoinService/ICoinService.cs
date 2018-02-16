
namespace HistoCoin.Server.Services.CoinService
{
    using System.Collections.Generic;

    public interface ICoinService
    {
        IList<CoinModel> GetAll();

        CoinModel GetById(int id);

        int Add(CoinModel record);

        void Update(CoinModel record);

        void Delete(int id);
    }
}
