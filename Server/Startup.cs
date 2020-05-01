namespace HistoCoin.Server
{
    using System;
    using System.IO;
    using System.Text;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.SpaServices.Webpack;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;
    using DotNetify;
    using DotNetify.Security;
    using HistoCoin.Server.Infrastructure.Extensions;
    using static HistoCoin.Server.Infrastructure.Constants;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Add OpenID Connect server to produce JWT access tokens.
            services.AddAuthenticationServer();

            services.AddMemoryCache();
            services.AddSignalR();
            services.AddDotNetify();

            // TODO: dependency container until service inheritance is fixed
            //services.AddUserService(DefaultUserStoreLocation);
            //services.AddCacheService(DebugUserId);
            //services.AddCoinService();
            //services.AddCurrencyService();

            services.AddCurrencyServices();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseWebSockets();
            app.UseSignalR(routes => routes.MapDotNetifyHub());
            app.UseDotNetify(
                config =>
                {
                    // Middleware to do authenticate token in incoming request headers.
                    config.UseJwtBearerAuthentication(
                        new TokenValidationParameters
                        {
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(AuthServer.SecretKey)),
                            ValidateIssuerSigningKey = true,
                            ValidateAudience = false,
                            ValidateIssuer = false,
                            ValidateLifetime = true,
                            ClockSkew = TimeSpan.FromSeconds(0)
                        });

                    // Filter to check whether user has permission to access view models with [Authorize] attribute.
                    config.UseFilter<AuthorizeFilter>();
                });

            app.UseWebpackDevMiddleware(
                new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true,
                    ReactHotModuleReplacement = true
                });

            app.UseStaticFiles();

            app.Run(
                async context =>
                {
                    var uri = context.Request.Path.ToUriComponent();
                    if (uri.EndsWith(".map"))
                    {
                        return;
                    }

                    if (uri.EndsWith("_hmr")) // Fix HMR for deep links.
                    {
                        context.Response.Redirect("/dist/__webpack_hmr");
                    }

                    using var reader = new StreamReader(File.OpenRead("wwwroot/index.html"));

                    await context.Response.WriteAsync(reader.ReadToEnd());
                });
        }
    }
}