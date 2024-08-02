using LightBDD.XUnit2;


namespace SlotService.Integration.Common;

public class AvailabilityFeatureBase : FeatureFixture, IClassFixture<TestApplicationFactory>
{
    protected const string GetWeeklyAvailabilityUrl = "/api/availability/GetWeeklyAvailability";
    protected const string TakeSlotUrl = "/api/availability/TakeSlot";
    protected readonly TestApplicationFactory Factory;
    protected HttpClient Client;

    protected AvailabilityFeatureBase(TestApplicationFactory factory)
    {
        Factory = factory;
        Client = Factory.CreateClient();
        Client.BaseAddress = new Uri("https://localhost:7188");
    }
}
