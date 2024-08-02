using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;


namespace SlotService.API.REST.Common;

public class BasicAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    private static string AcceptedUserName => "techuser";
    private static string AcceptedPassword => "secretpassWord";

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
        {
            return AuthenticateResult.Fail("Missing Authorization Header");
        }
        var incomingAuthorizationHeader = Request.Headers["Authorization"].ToString();
        if (string.IsNullOrEmpty(incomingAuthorizationHeader))
        {
            return AuthenticateResult.Fail("The authorization header is empty");
        }

        var authorizationHeader = AuthenticationHeaderValue.Parse(incomingAuthorizationHeader);

        if (authorizationHeader.Parameter == null)
        {
            return AuthenticateResult.Fail("Information provided for authorization is not valid");
        }

        string[] credentials;
        try
        {
            credentials = Encoding.UTF8
                                  .GetString(
                                       Convert.FromBase64String(authorizationHeader.Parameter))
                                  .Split(':', 2);
        }
        catch (FormatException ex)
        {
            return AuthenticateResult.Fail(ex);
        }
        var username = credentials[0];
        var password = credentials[1];

        if (username != AcceptedUserName ||
            password != AcceptedPassword)
        {
            return AuthenticateResult.Fail("Invalid Username or Password");
        }

        var claims = new[]
                     {
                         new Claim(ClaimTypes.NameIdentifier, username),
                         new Claim(ClaimTypes.Name, username)
                     };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}
