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
public class TextFunctionUnitTest
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
        context.Request.Headers["Authorization"] = TestContext.Parameters["adminAuthHeader"];
        //Environment.GetEnvironmentVariable("AdminAuthHeader");//"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJhZG1pbiIsImp0aSI6ImM3MjI5OGQxLWM1MTAtNGYyZS04YjNmLWRkNmY0MGY0YzU1YSIsImV4cCI6MTc1NjI0NjcwMiwiaXNzIjoieW91cmRvbWFpbi5jb20iLCJhdWQiOiJ5b3VyZG9tYWluLmNvbSJ9.tmCdMV-7iKoV1WR4tRkYGIRyLCifYKnGR21cZbOPK_c";

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