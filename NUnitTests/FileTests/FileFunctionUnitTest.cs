//using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
//using Microsoft.Extensions.Logging;
//using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Calculator;

[TestFixture]
public class FileFunctionUnitTest
{
    [Test]
    public void GetFileNames_ReturnsOkResult()
    {
        // Arrange
        var mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<CalculatorFunction>>();
        var function = new CalculatorFunction(mockLogger.Object); // <-- Use .Object to get the ILogger instance

        var context = new DefaultHttpContext();
        context.Request.QueryString = new QueryString("?x=10&y=5");
        context.Request.Headers["Authorization"] = TestContext.Parameters["adminAuthHeader"];

        // Act
        var result = function.Subtract(context.Request) as OkObjectResult;

        // Assert
        Assert.That(result?.Value, Is.Not.Null);
        Assert.That(result?.Value, Is.EqualTo(5));
    }

    [Test]
    public void Subtract_ReturnsOkResult_WithInvalidInput()
    {
        // Arrange
        // Assuming CalculatorFunction is in another project referenced by this test project
        var mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<CalculatorFunction>>();
        var function = new CalculatorFunction(mockLogger.Object); // <-- Use .Object to get the ILogger instance

        var context = new DefaultHttpContext();
        context.Request.QueryString = new QueryString("?x=10&y=Q");

        // Act
        var result = function.Subtract(context.Request) as ObjectResult;

        // Assert
        Assert.That(result?.Value, Is.Not.Null);
        Assert.That(result?.Value, Is.EqualTo("Query parameters 'x' and 'y' must be valid integers."));
    }
}