
namespace HistoCoin.Server.Infrastructure.Extensions
{
    using System;
    using System.Linq;
    using System.Security;

    public static class StringExtensions
    {
        public static SecureString AppendString(this SecureString @this, string content)
        {
            foreach (var chara in content)
            {
                @this.AppendChar(chara);
            }

            return @this;
        }
    }
}
