using SlotService.API.REST.Common;


namespace SlotService.API.REST.UnitTests.Common;

[TestFixture]
public class PascalCaseNamingPolicyTest
{
    private readonly PascalCaseNamingPolicy _policy = new();

    [TestCase("", ExpectedResult = "")]
    [TestCase(" ", ExpectedResult = " ")]
    [TestCase("test", ExpectedResult = "Test")]
    [TestCase("Test", ExpectedResult = "Test")]
    public string ConvertName(string name)
    {
        return _policy.ConvertName(name);
    }
}
