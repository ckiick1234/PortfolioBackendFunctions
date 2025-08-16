using Azure.Core;
using jwtAuth.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auth;

public class AuthFunction
{
    private readonly ILogger<AuthFunction> _logger;

    public AuthFunction(ILogger<AuthFunction> logger)
    {
        _logger = logger;
    }

    [Function("Login")]
    public IActionResult Subtract([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        if (req.Query["username"] == "admin" && req.Query["password"] == "password")
        {
            var token = GenerateJwtToken(req.Query["username"]);
            return new OkObjectResult(new { token });
        }
        return new UnauthorizedResult();
    }

    private string GenerateJwtToken(string username)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_super_super_super_secret_key"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "yourdomain.com",
            audience: "yourdomain.com",
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}



//[ApiController]
//[Route("api/[controller]")]
//public class AuthController : ControllerBase
//{
//    [HttpPost("login")]
//    public IActionResult Login([FromBody] UserLogin user)
//    {
//        if (user.Username == "admin" && user.Password == "password")
//        {
//            var token = GenerateJwtToken(user.Username);
//            return Ok(new { token });
//        }
//        return Unauthorized();
//    }

    
//}