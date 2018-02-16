
namespace HistoCoin.Server.Services.EmployeeService
{
    public class EmployeeModel
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int ReportTo { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }
}
