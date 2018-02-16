
namespace HistoCoin.Server.Services.EmployeeService
{
    using System.Collections.Generic;

    public interface IEmployeeService
    {
        IList<EmployeeModel> GetAll();

        EmployeeModel GetById(int id);

        int Add(EmployeeModel record);

        void Update(EmployeeModel record);

        void Delete(int id);
    }
}
