using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Moq;
using SlotService.API.REST.Common;


namespace SlotService.API.REST.UnitTests.Authentication;

[TestFixture]
public class BasicAuthenticationHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        _options = new Mock<IOptionsMonitor<AuthenticationSchemeOptions>>();

        // This Setup is required for .NET Core 3.1 onwards.
        _options
           .Setup(x => x.Get(It.IsAny<string>()))
           .Returns(new AuthenticationSchemeOptions());

        var logger = new Mock<ILogger<BasicAuthenticationHandler>>();
        _loggerFactory = new Mock<ILoggerFactory>();
        _loggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(logger.Object);

        _encoder = new Mock<UrlEncoder>();

        _handler = new BasicAuthenticationHandler(
            _options.Object,
            _loggerFactory.Object,
            _encoder.Object);
    }

    private Mock<IOptionsMonitor<AuthenticationSchemeOptions>> _options;
    private Mock<ILoggerFactory> _loggerFactory;
    private Mock<UrlEncoder> _encoder;
    private BasicAuthenticationHandler _handler;
    private const string ValidUsername = "techuser";
    private const string ValidPassword = "secretpassWord";

    [Test]
    public async Task Authenticate_WhenAuthorizationHeaderIsMissing_Fails()
    {
        // Arrange
        var context = new DefaultHttpContext();

        // Act
        await _handler.InitializeAsync(
            new AuthenticationScheme("Basic", null, typeof(BasicAuthenticationHandler)),
            context);
        var result = await _handler.AuthenticateAsync();

        // Assert
        Assert.IsFalse(result.Succeeded);
    }

    [Test]
    public async Task Authenticate_WhenAuthorizationHeaderIsInvalid_Fails()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var authorizationHeader = "Basic " + Convert.ToBase64String(
                                      "invalid:credentials"u8.ToArray());
        context.Request.Headers.Add(HeaderNames.Authorization, authorizationHeader);

        // Act
        await _handler.InitializeAsync(
            new AuthenticationScheme("Basic", null, typeof(BasicAuthenticationHandler)),
            context);
        var result = await _handler.AuthenticateAsync();

        // Assert
        Assert.IsFalse(result.Succeeded);
    }

    [Test]
    public async Task Authenticate_WhenAuthorizationHeaderIsValid_Succeeds()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var authorizationHeader = "Basic " + Convert.ToBase64String(
                                      Encoding.UTF8.GetBytes(
                                          $"{ValidUsername}:{ValidPassword}"));
        context.Request.Headers.Add(HeaderNames.Authorization, authorizationHeader);

        // Act
        await _handler.InitializeAsync(
            new AuthenticationScheme("Basic", null, typeof(BasicAuthenticationHandler)),
            context);
        var result = await _handler.AuthenticateAsync();

        // Assert
        Assert.IsTrue(result.Succeeded);
        Assert.AreEqual(ValidUsername, result.Principal?.Identity?.Name);
    }

    [Test]
    public async Task Authenticate_WhenAuthorizationHeaderHasInvalidFormat_Fails()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var authorizationHeader = "Basic invalidBase64";
        context.Request.Headers.Add(HeaderNames.Authorization, authorizationHeader);

        // Act
        await _handler.InitializeAsync(
            new AuthenticationScheme("Basic", null, typeof(BasicAuthenticationHandler)),
            context);
        var result = await _handler.AuthenticateAsync();

        // Assert
        Assert.IsFalse(result.Succeeded);
        Assert.IsInstanceOf<FormatException>(result.Failure);
    }
}
