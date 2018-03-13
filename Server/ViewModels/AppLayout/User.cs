
namespace HistoCoin.Server.ViewModels.AppLayout
{
    public class User
    {
        public const string DefaultUsername = "New User";
        public const string DefaultEmail = "newuser@histocoin.com";
        public const string DefaultBackground = "../images/material_bg.png";
        public const string DefaultAvatar = "https://abs.twimg.com/sticky/default_profile_images/default_profile_400x400.png";

        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string LocalCache { get; set; }

        public string Avatar { get; set; } = DefaultAvatar;
    }
}
