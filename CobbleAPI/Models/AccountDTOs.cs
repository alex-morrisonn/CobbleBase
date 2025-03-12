namespace CobbleAPI.Models
{
    // DTOs (Data Transfer Objects)
    public class CreateAccountRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class UpdateAccountRequest
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    public class AccountResponse
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Password { get; set; }
    }
}