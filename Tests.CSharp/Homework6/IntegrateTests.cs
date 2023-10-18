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
        // Act
        var response = await _client.GetAsync("/");
        var responseString = await response.Content.ReadAsStringAsync();
        
        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal("Use //calculate?value1=<VAL1>&operation=<OPERATION>&value2=<VAL2>", responseString);
    }
    
    [HomeworkTheory(Homeworks.HomeWork6)]
    [InlineData("/calculate")]
    [InlineData("/calculate?operation=Plus&value2=3")]
    [InlineData("/calculate?value1=2&value2=3")]
    [InlineData("/calculate?value1=2&operation=Plus")]
    public async Task TestCalculateRouteWithoutOneMoreParamReturnsExpectedError(string requestUri)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

        // Act
        var response = await _client.SendAsync(request);
        var responseString = await response.Content.ReadAsStringAsync();
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Missing value for required property", responseString);
    }

    [Homework(Homeworks.HomeWork6)]
    public async Task TestCalculateRouteReturnsExpectedResult()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/calculate?value1=2&operation=Plus&value2=3");

        // Act
        var response = await _client.SendAsync(request);
        var responseString = await response.Content.ReadAsStringAsync();
        
        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal("5", responseString);
    }

    [Homework(Homeworks.HomeWork6)]
    public async Task TestInvalidRequestReturnsExpectedError()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get,"/calculate?value1=NotParsable&operation=NotParsable&value2=NotParsable");
        
        // Act
        var response = await _client.SendAsync(request);
        var responseString = await response.Content.ReadAsStringAsync();
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Could not parse", responseString);
    }

    [Homework(Homeworks.HomeWork6)]
    public async Task TestNotExistedRouteReturnsExpectedError()
    {
        // Act
        var response = await _client.GetAsync("/NotExist");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}