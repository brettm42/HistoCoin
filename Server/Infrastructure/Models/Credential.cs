
namespace HistoCoin.Server.Infrastructure.Models
{
    using System.Diagnostics;

    [DebuggerStepThrough]
    public class Credential
    {
        public Credential(string username, string password)
        {
            this.Username = username;
            this.Password = password;
        }

        public int Id => this.GetHashCode();

        public string Username { get; }
        
        public string Password { get; }
    }
}
