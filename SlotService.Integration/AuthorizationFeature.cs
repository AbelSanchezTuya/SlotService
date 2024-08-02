using System.Net;
using System.Net.Http.Headers;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using SlotService.Integration.Common;


namespace SlotService.Integration;

[FeatureFixture]
public class AuthorizationFeatureTests(TestApplicationFactory factory)
    : AvailabilityFeatureBase(factory)
{
    private HttpResponseMessage _response = new(HttpStatusCode.ServiceUnavailable);

    [Scenario]
    public void Unauthorized_user_attempts_to_access_GetWeeklyAvailability()
    {
        Runner.RunScenario(
            _ => Given_an_unauthorized_user(),
            _ => When_the_user_tries_to_access_GetWeeklyAvailability(),
            _ => Then_the_user_is_not_authorized());
    }

    [Scenario]
    public void Authorized_user_accesses_GetWeeklyAvailability()
    {
        Runner.RunScenario(
            _ => Given_an_authorized_user(),
            _ => When_the_user_tries_to_access_GetWeeklyAvailability(),
            _ => Then_the_user_is_authorized());
    }

    [Scenario]
    public void Unauthorized_user_attempts_to_access_TakeSlot()
    {
        Runner.RunScenario(
            _ => Given_an_unauthorized_user(),
            _ => When_the_user_tries_to_access_TakeSlot(),
            _ => Then_the_user_is_not_authorized());
    }

    [Scenario]
    public void Authorized_user_accesses_TakeSlot()
    {
        Runner.RunScenario(
            _ => Given_an_authorized_user(),
            _ => When_the_user_tries_to_access_TakeSlot(),
            _ => Then_the_user_is_authorized());
    }

    private void Given_an_unauthorized_user()
    {
        // No authorization header set
    }

    private void Given_an_authorized_user()
    {
        // Set authorization header with valid credentials
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Basic",
            Convert.ToBase64String("techuser:secretpassWord"u8.ToArray()));
    }

    private void When_the_user_tries_to_access_GetWeeklyAvailability()
    {
        var url = $"{GetWeeklyAvailabilityUrl}/{DateTime.Now:yyyyMMdd}";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        _response = Client.SendAsync(request).Result;
    }

    private void When_the_user_tries_to_access_TakeSlot()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, TakeSlotUrl);
        _response = Client.SendAsync(request).Result;
    }

    private void Then_the_user_is_not_authorized()
    {
        Assert.Equal(HttpStatusCode.Unauthorized, _response.StatusCode);
    }

    private void Then_the_user_is_authorized()
    {
        Assert.NotEqual(HttpStatusCode.Unauthorized, _response.StatusCode);
    }
}
