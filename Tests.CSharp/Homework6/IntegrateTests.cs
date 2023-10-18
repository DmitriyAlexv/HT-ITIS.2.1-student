using System.Globalization;
using System.Net;
using Hw6;
using Tests.RunLogic.Attributes;

namespace Tests.CSharp.Homework6;

public class IntegrateTests: IClassFixture<CustomWebApplicationFactory<App.Startup>>
{
    private readonly HttpClient _client;

    public IntegrateTests(CustomWebApplicationFactory<App.Startup> factory)
    {
        _client = factory.CreateClient();
    }
    
    [Homework(Homeworks.HomeWork6)]
    public async Task TestRootRouteReturnsExpectedResponse()
    {
        await RunTest("/", "Use //calculate?value1=<VAL1>&operation=<OPERATION>&value2=<VAL2>", HttpStatusCode.OK);
    }
    
    [HomeworkTheory(Homeworks.HomeWork6)]
    [InlineData("/calculate")]
    [InlineData("/calculate?operation=Plus&value2=3")]
    [InlineData("/calculate?value1=2&value2=3")]
    [InlineData("/calculate?value1=2&operation=Plus")]
    public async Task TestCalculateRouteWithoutOneMoreParamReturnsExpectedError(string requestUri)
    {
        await RunTest(requestUri, "Missing value for required property", HttpStatusCode.BadRequest);
    }

    [Homework(Homeworks.HomeWork6)]
    public async Task TestCalculateRouteReturnsExpectedResult()
    {
        await RunTest("/calculate?value1=2&operation=Plus&value2=3", "5", HttpStatusCode.OK);
    }

    [Homework(Homeworks.HomeWork6)]
    public async Task TestInvalidRequestReturnsExpectedError()
    {
        await RunTest("/calculate?value1=NotParsable&operation=NotParsable&value2=NotParsable", "Could not parse", HttpStatusCode.BadRequest);
    }

    [Homework(Homeworks.HomeWork6)]
    public async Task TestNotExistedRouteReturnsExpectedError()
    {
        await RunTest("/NotExist", "Not Found", HttpStatusCode.NotFound);
    }

    private async Task RunTest(string query, string expectedValue, HttpStatusCode expectedStatusCode)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, query);

        // Act
        var response = await _client.SendAsync(request);
        var responseString = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(expectedStatusCode, response.StatusCode);
        Assert.Contains(expectedValue, responseString);
    }
}