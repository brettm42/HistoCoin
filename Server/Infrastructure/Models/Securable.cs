
namespace HistoCoin.Server.Infrastructure.Models
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public class Securable
    {
        public Securable()
        {
        }

        public Securable(string message)
        {
            this.SetContent(message);
        }

        public string Content { get; private set; }

        public string Salt { get; private set; }

        public void SetContent(string message)
        {
            var (salt, hash) =
                Securable.HashString(
                    Securable.SaltString(message));

            this.Salt = salt;
            this.Content = hash;
        }

        public bool Equals(string message)
        {
            var (_, hash) = 
                Securable.HashString((this.Salt, this.Salt + message));

            return this.Content.Equals(hash, StringComparison.InvariantCulture);
        }

        public override string ToString() => this.Salt + '_' + this.Content;

        private static (string Salt, string Text) SaltString(string message)
        {
            var random = new Random(nameof(HistoCoin).GetHashCode());

            var salt = random.Next().ToString();

            return (salt, salt + message);
        }

        private static (string Salt, string Hash) HashString((string Salt, string Text) text)
        {
            var sha = SHA256.Create();

            var bytes = Encoding.UTF8.GetBytes(text.Text);

            var hash = sha.ComputeHash(bytes);
            
            return (text.Salt, Convert.ToBase64String(hash));
        }
    }
}
