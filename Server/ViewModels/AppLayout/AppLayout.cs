namespace HistoCoin.Server.ViewModels.AppLayout
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using DotNetify;
    using DotNetify.Routing;
    using DotNetify.Security;
    using static HistoCoin.Server.Infrastructure.Helpers;

    [Authorize]
    public class AppLayout : BaseVM, IRoutable
    {
        private enum Route
        {
            Home,
            Dashboard,
            FormPage,
            TablePage
        };

        public static string FormPagePath => "Form";

        public RoutingState RoutingState { get; set; }

        public object Menus => 
            new List<object>
            {
                new { Title = "Dashboard", Icon = "assessment", Route = this.GetRoute(nameof(Route.Dashboard)) },
                new { Title = "Coin Details", Icon = "web", Route = this.GetRoute(nameof(Route.FormPage), $"{FormPagePath}/1") },
                new { Title = "Coin List", Icon = "grid_on", Route = this.GetRoute(nameof(Route.TablePage)) },
            };

        public string UserName { get; set; }

        public string UserAvatar { get; set; }

        public string UserBackground { get; set; }

        public string EmailAddress { get; set; }

        public string LastLogin { get; set; }

        public AppLayout(IPrincipalAccessor principalAccessor)
        {
            var userIdentity = principalAccessor.Principal.Identity as ClaimsIdentity;

            // TODO: create user object and lookup saved settings for user on login

            this.UserName = 
                string.IsNullOrWhiteSpace(userIdentity.Name) 
                    ? "New User" 
                    : userIdentity.Name;

            this.EmailAddress = 
                userIdentity.Claims.FirstOrDefault(i => i.Type == ClaimTypes.Email)?.Value;

            this.LastLogin = 
                TimeOffsetAsString(
                    DateTimeOffset.Now - TimeSpan.FromHours(new Random().NextDouble() * new Random().NextDouble()), 
                    DateTimeOffset.Now, 
                    System.Globalization.CultureInfo.CurrentUICulture);

            this.UserAvatar = User.DefaultAvatar;

            this.UserBackground = User.DefaultBackground;

            this.RegisterRoutes(
                "/", 
                new List<RouteTemplate>
                {
                    new RouteTemplate(nameof(Route.Home)) { UrlPattern = string.Empty, ViewUrl = nameof(Route.Dashboard) },
                    new RouteTemplate(nameof(Route.Dashboard)),
                    new RouteTemplate(nameof(Route.FormPage)) { UrlPattern = $"{FormPagePath}(/:id)" },
                    new RouteTemplate(nameof(Route.TablePage))
                });
        }
    }
}