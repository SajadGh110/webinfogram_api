using System.ComponentModel.DataAnnotations;

namespace consoleapi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        [Required(ErrorMessage = "UserName is Required!")]
        public required string UserName { get; set; }
        [Required(ErrorMessage = "Password is Required!")]
        public required string Password { get; set; }
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? GoogleRefreshToken { get; set; }
        public string? SiteUrl { get; set; }
        public string? ApplicationName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? PostalCode { get; set; }
        public string Role { get; set; }
        public string? Token { get; set; }
    }
}
