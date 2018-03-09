
namespace HistoCoin.Server.ViewModels.AppLayout
{
    public class User
    {
        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string LocalCache { get; set; }

        public string Avatar { get; set; } =
            "https://abs.twimg.com/sticky/default_profile_images/default_profile_400x400.png";
    }
}
