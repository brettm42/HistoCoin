﻿
namespace HistoCoin.Server.Services.CoinService
{
    using System.Collections.Generic;
    using HistoCoin.Server.Infrastructure;
    using static HistoCoin.Server.Infrastructure.Constants;
    
    public interface ICoinService
    {
        IList<ICoin> GetAll();

        Currencies BaseCurrency { get; set; }

        ICoin GetById(int id);

        int Add(ICoin record);

        void Update(ICoin record);

        void Delete(int id);
    }
}
