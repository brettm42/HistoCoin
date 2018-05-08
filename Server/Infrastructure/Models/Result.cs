
namespace HistoCoin.Server.Infrastructure.Models
{
    using System.Diagnostics;

    [DebuggerStepThrough]
    public class Result
    {
        public Result(bool success, string message)
        {
            this.Success = success;
            this.Message = message;
        }

        public bool Success { get; set; }

        public string Message { get; set; }
    }
}
