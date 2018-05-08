
namespace HistoCoin.Server.Infrastructure.Models
{
    using System.Diagnostics;

    [DebuggerStepThrough]
    public class Packet<T>
    {
        public Packet(string name, T contents)
        {
            this.Name = name;
            this.Contents = contents;
        }

        public Packet(string name, T contents, bool featured)
        {
            this.Name = name;
            this.Contents = contents;
            this.Featured = featured;
        }

        public int Id => this.GetHashCode();

        public string Name { get; set; }

        public T Contents { get; set; }

        public bool Featured { get; set; }
    }
}
