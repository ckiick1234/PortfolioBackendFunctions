using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Calculator;

public class CalculatorFunction
{
    private readonly ILogger<CalculatorFunction> _logger;

    public CalculatorFunction(ILogger<CalculatorFunction> logger)
    {
        _logger = logger;
    }

    [Function("Subtract")]
    [Authorize]
    public IActionResult Subtract([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        int x = int.Parse(req.Query["x"]);
        int y = int.Parse(req.Query["y"]);

        int result = x - y;

        return new OkObjectResult(result);
    }


    [Function("Sum")]
    public IActionResult Sum([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        int x = int.Parse(req.Query["x"]);
        int y = int.Parse(req.Query["y"]);

        int result = x + y;

        return new OkObjectResult(result);
    }
}