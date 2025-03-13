namespace CobbleAPI.Models;

public class RegisterViewModel
{
    public Users User { get; set; }
    public UserEmail UserEmail { get; set; }
}

public class UserDto
{
    /// <summary>
    /// UserId
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Password
    /// </summary>
    public string Password { get; set; }
    /// <summary>
    /// FirstName
    /// </summary>
    public string? FirstName { get; set; }
    /// <summary>
    /// LastName
    /// </summary>
    public string? LastName { get; set; }
    /// <summary>
    /// OtherUsableEmail
    /// </summary>
    public string? OtherUsableEmail { get; set; }
    /// <summary>
    /// WorkMobileNumber
    /// </summary>
    public string? WorkMobileNumber { get; set; }
    /// <summary>
    /// PersonalMobileNumber
    /// </summary>
    public string? PersonalMobileNumber { get; set; }
    /// <summary>
    /// EmergencyContactEmail
    /// </summary>
    public string? EmergencyContactEmail { get; set; }
    /// <summary>
    /// Avatar(base64)
    /// </summary>
    public string? Avatar { get; set; }
    /// <summary>
    /// Email
    /// </summary>
    public string Email { get; set; }
    /// <summary>
    /// address
    /// </summary>
    public List<UserAddressDto> Address { get; set; }
}

public class RegisterDto
{
    /// <summary>
    /// Password
    /// </summary>
    public string Password { get; set; }
    /// <summary>
    /// FirstName
    /// </summary>
    public string? FirstName { get; set; }
    /// <summary>
    /// LastName
    /// </summary>
    public string? LastName { get; set; }
    /// <summary>
    /// OtherUsableEmail
    /// </summary>
    public string? OtherUsableEmail { get; set; }
    /// <summary>
    /// WorkMobileNumber
    /// </summary>
    public string? WorkMobileNumber { get; set; }
    /// <summary>
    /// PersonalMobileNumber
    /// </summary>
    public string? PersonalMobileNumber { get; set; }
    /// <summary>
    /// EmergencyContactEmail
    /// </summary>
    public string? EmergencyContactEmail { get; set; }
    /// <summary>
    /// Avatar(base64)
    /// </summary>
    public string? Avatar { get; set; }
    /// <summary>
    /// Email
    /// </summary>
    public string Email { get; set; }
}


public class UpdateUserDto
{
    /// <summary>
    /// Password
    /// </summary>
    public string Password { get; set; }
    /// <summary>
    /// FirstName
    /// </summary>
    public string? FirstName { get; set; }
    /// <summary>
    /// LastName
    /// </summary>
    public string? LastName { get; set; }
    /// <summary>
    /// OtherUsableEmail
    /// </summary>
    public string? OtherUsableEmail { get; set; }
    /// <summary>
    /// WorkMobileNumber
    /// </summary>
    public string? WorkMobileNumber { get; set; }
    /// <summary>
    /// PersonalMobileNumber
    /// </summary>
    public string? PersonalMobileNumber { get; set; }
    /// <summary>
    /// EmergencyContactEmail
    /// </summary>
    public string? EmergencyContactEmail { get; set; }
    /// <summary>
    /// Avatar(base64)
    /// </summary>
    public string? Avatar { get; set; }
    /// <summary>
    /// user Addresses
    /// </summary>
    public List<UserAddressDto> Addresses { get; set; }
}

public class UserAddressDto
{
    /// <summary>
    /// Id
    /// </summary>
    public int? Id { get; set; }
    /// <summary>
    /// address
    /// </summary>
    public string Address { get; set; }
}

public class LoginDto
{
    /// <summary>
    /// Email
    /// </summary>
    public string Email { get; set; }
    /// <summary>
    /// Password
    /// </summary>
    public string Password { get; set; }
}