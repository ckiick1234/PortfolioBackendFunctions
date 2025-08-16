using System.ComponentModel.DataAnnotations;

namespace Auth;

public class RegistrationRequest
{
    [Required]
    public string UserID { get; set; }

    [Required]
    public string Username { get; set; }

    [Required]
    public string Firstname { get; set; }

    [Required]
    public string Lastname { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}