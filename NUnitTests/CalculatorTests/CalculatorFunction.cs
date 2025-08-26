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
public class CalculatorFunctionUnitTest
{
    //private readonly ILogger<CalculatorFunction> _logger;

    //public CalculatorFunctionUnitTest(ILogger<CalculatorFunction> logger)
    //{
    //    _logger = logger;
    //}

    [Test]
    public void Subtract_ReturnsOkResult_WithValidInput()
    {
        // Arrange
        // Assuming CalculatorFunction is in another project referenced by this test project
        var mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<CalculatorFunction>>();
        var function = new CalculatorFunction(mockLogger.Object); // <-- Use .Object to get the ILogger instance

        var context = new DefaultHttpContext();
        context.Request.QueryString = new QueryString("?x=10&y=5");

        // Act
        var result = function.Subtract(context.Request) as OkObjectResult;

        // Assert
        Assert.That(result?.Value, Is.Not.Null);
        Assert.That(result?.Value, Is.EqualTo(5));
    }
}