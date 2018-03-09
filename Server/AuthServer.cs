namespace HistoCoin.Server
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using AspNet.Security.OpenIdConnect.Extensions;
    using AspNet.Security.OpenIdConnect.Primitives;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;

    public static class AuthServer
    {
        public const string SecretKey = "my_Obviously_secretkey_123!";

        // Source: https://github.com/aspnet-contrib/AspNet.Security.OpenIdConnect.Server
        public static void AddAuthenticationServer(this IServiceCollection services)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));

            services
                .AddAuthentication()
                .AddOpenIdConnectServer(
                    options =>
                    {
                        options.AccessTokenHandler = new JwtSecurityTokenHandler();
                        options.SigningCredentials.AddKey(signingKey);

                        options.AllowInsecureHttp = true;
                        options.TokenEndpointPath = "/token";

                        options.Provider.OnValidateTokenRequest = 
                            context =>
                            {
                                // Reject token requests that don't use grant_type=password or grant_type=refresh_token.
                                if (!context.Request.IsPasswordGrantType() && !context.Request.IsRefreshTokenGrantType())
                                {
                                    context.Reject(
                                        error: OpenIdConnectConstants.Errors.UnsupportedGrantType,
                                        description: "Only grant_type=password and refresh_token requests are accepted by this server.");

                                    return Task.CompletedTask;
                                }

                                // Note: you can skip the request validation when the client_id
                                // parameter is missing to support unauthenticated token requests.
                                // if (string.IsNullOrEmpty(context.ClientId))
                                // {
                                //     context.Skip();
                                // 
                                //     return Task.CompletedTask;
                                // }

                                // Note: to mitigate brute force attacks, you SHOULD strongly consider applying
                                // a key derivation function like PBKDF2 to slow down the secret validation process.
                                // You SHOULD also consider using a time-constant comparer to prevent timing attacks.
                                //if (string.Equals(context.ClientId, "histocoin", StringComparison.Ordinal) 
                                //    && string.Equals(context.ClientSecret, "client_secret", StringComparison.Ordinal))
                                if (string.Equals(context.ClientId, "histocoin", StringComparison.Ordinal))
                                {
                                    context.Validate();
                                }

                                // Note: if Validate() is not explicitly called,
                                // the request is automatically rejected.
                                return Task.CompletedTask;

                                //context.Validate();

                                //return Task.CompletedTask;
                            };

                        options.Provider.OnHandleTokenRequest = 
                            context =>
                            {
                                // Only handle grant_type=password token requests and let
                                // the OpenID Connect server handle the other grant types.
                                if (context.Request.IsPasswordGrantType())
                                {
                                    // Implement context.Request.Username/context.Request.Password validation here.
                                    // Note: you can call context Reject() to indicate that authentication failed.
                                    // Using password derivation and time-constant comparer is STRONGLY recommended.
                                    //if (!string.Equals(context.Request.Username, "Bob", StringComparison.Ordinal) 
                                    //    || !string.Equals(context.Request.Password, "P@ssw0rd", StringComparison.Ordinal))
                                    if (context.Request.Password != "dotnetify")
                                    {
                                        context.Reject(
                                            error: OpenIdConnectConstants.Errors.InvalidGrant,
                                            description: "Invalid user credentials.");

                                        return Task.CompletedTask;
                                    }

                                    var identity = 
                                        new ClaimsIdentity(
                                            context.Scheme.Name,
                                            OpenIdConnectConstants.Claims.Name,
                                            OpenIdConnectConstants.Claims.Role);

                                    // Add the mandatory subject/user identifier claim.
                                    //identity.AddClaim(OpenIdConnectConstants.Claims.Subject, "[unique id]");
                                    identity.AddClaim(OpenIdConnectConstants.Claims.Name, context.Request.Username);
                                    identity.AddClaim(OpenIdConnectConstants.Claims.Subject, context.Request.Username);

                                    // By default, claims are not serialized in the access/identity tokens.
                                    // Use the overload taking a "destinations" parameter to make sure
                                    // your claims are correctly inserted in the appropriate tokens.
                                    identity.AddClaim(
                                        ClaimTypes.Email, 
                                        context.Request.Username + "@histocoin.com");

                                    var ticket = 
                                        new AuthenticationTicket(
                                            new ClaimsPrincipal(identity),
                                            new AuthenticationProperties(),
                                            context.Scheme.Name);

                                    // Call SetScopes with the list of scopes you want to grant
                                    // (specify offline_access to issue a refresh token).
                                    ticket.SetScopes(
                                        OpenIdConnectConstants.Scopes.Profile,
                                        OpenIdConnectConstants.Scopes.OfflineAccess);

                                    context.Validate(ticket);
                                }

                                return Task.CompletedTask;

                                //if (context.Request.Password != "dotnetify")
                                //{
                                //    context.Reject(
                                //        error: OpenIdConnectConstants.Errors.InvalidGrant,
                                //        description: "Invalid user credentials.");

                                //    return Task.CompletedTask;
                                //}

                                //var identity = 
                                //    new ClaimsIdentity(
                                //        context.Scheme.Name,
                                //        OpenIdConnectConstants.Claims.Name,
                                //        OpenIdConnectConstants.Claims.Role);

                                //identity.AddClaim(OpenIdConnectConstants.Claims.Name, context.Request.Username);
                                //identity.AddClaim(OpenIdConnectConstants.Claims.Subject, context.Request.Username);

                                //identity.AddClaim(
                                //    ClaimTypes.Name, 
                                //    context.Request.Username,
                                //    OpenIdConnectConstants.Destinations.AccessToken,
                                //    OpenIdConnectConstants.Destinations.IdentityToken);

                                //identity.AddClaim(
                                //    ClaimTypes.Uri,
                                //    "https://abs.twimg.com/sticky/default_profile_images/default_profile_400x400.png",
                                //    OpenIdConnectConstants.Destinations.AccessToken,
                                //    OpenIdConnectConstants.Destinations.IdentityToken);

                                //var ticket = 
                                //    new AuthenticationTicket(
                                //        new ClaimsPrincipal(identity),
                                //        new AuthenticationProperties(),
                                //        context.Scheme.Name);

                                //ticket.SetScopes(
                                //    OpenIdConnectConstants.Scopes.Profile,
                                //    OpenIdConnectConstants.Scopes.OfflineAccess);

                                //context.Validate(ticket);

                                //return Task.CompletedTask;
                            };
                    });
        }
    }
}