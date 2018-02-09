namespace HistoCoin.Server.Services
{
    using System.Collections.Generic;

    public interface ICacheService
    {
        IList<Cache> GetAll();

        Cache GetById(int id);

        int Add(Cache record);

        void Update(Cache record);

        void Delete(int id);
    }
}