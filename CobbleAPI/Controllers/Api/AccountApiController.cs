using Microsoft.AspNetCore.Mvc;
using CobbleAPI.Models;
using CobbleAPI.Service;

namespace CobbleAPI.Controllers.Api;

[ApiController]
[Route("module/[controller]")]
public class AccountApiController : ControllerBase
{
    private readonly UsersService _userService;

    public AccountApiController(UsersService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// user register
    /// </summary>
    /// <param name="user"></param>
    /// <param name="email"></param>
    /// <returns></returns>
    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterDto model)
    {
        byte[]? ava = null;
        try
        {
            ava = model.Avatar != null && model.Avatar.Length > 0 ? Convert.FromBase64String(model.Avatar) : null;
        }
        catch (FormatException) { }
        var result = _userService.Register(new Users
        {
            Avatar = ava,
            EmergencyContactEmail = model.EmergencyContactEmail,
            FirstName = model.FirstName,
            LastName = model.LastName,
            OtherUsableEmail = model.OtherUsableEmail,
            Password = model.Password,
            PersonalMobileNumber = model.PersonalMobileNumber,
            WorkMobileNumber = model.WorkMobileNumber
        }, new UserEmail
        {
            Email = model.Email
        });
        if (result.Item1)
        {
            return Ok();
        }
        return BadRequest(result.Item2);
    }

    /// <summary>
    /// verify email
    /// </summary>
    /// <returns></returns>
    [HttpPost("verify-email")]
    public IActionResult VerifyEmail(string code)
    {
        var user = _userService.GetUserByEmailCode(code);
        if (user == null)
            return NotFound("Code not found");
        user.IsValidEmail = true;
        _userService.Update(user);
        return Ok();
    }

    /// <summary>
    /// delete address
    /// </summary>
    /// <returns></returns>
    [HttpDelete("user-address/{addressId}")]
    public IActionResult DeleteUserAddress(int addressId)
    {
        var address = _userService.GetUserAddressById(addressId);
        if (address == null) return NotFound();
        _userService.DeleteUserAddress(addressId);
        return Ok();
    }

    ///// <summary>
    ///// login
    ///// </summary>
    ///// <returns></returns>
    //[HttpPost("login")]
    //public IActionResult Login([FromBody] LoginDto model)
    //{
    //    var user = _userService.Authenticate(model.Email, model.Password);
    //    if (user.Item1 == null)
    //        return BadRequest("login fail，Please check your email and password.！");
    //    if (!user.Item2.IsValidEmail)
    //        return BadRequest("login fail，Your email has not been verified yet. Please verify it before logging in.");
    //    return Ok();
    //}

    /// <summary>
    /// get user
    /// </summary>
    /// <returns></returns>
    [HttpGet("get-user-info/{userId}")]
    public IActionResult GetUserInfo(int userId)
    {
        var user = _userService.GetUserById(userId);
        var userEmail = _userService.GetUserByUserId(userId);
        var address = _userService.GetUserAddressByUserId(userId);
        if (user == null)
            return NotFound();
        return Ok(new UserDto
        {
            Id = user.Id,
            Avatar = user.Avatar != null ? Convert.ToBase64String(user.Avatar) : null,
            EmergencyContactEmail = user.EmergencyContactEmail,
            FirstName = user.FirstName,
            LastName = user.LastName,
            OtherUsableEmail = user.OtherUsableEmail,
            PersonalMobileNumber = user.PersonalMobileNumber,
            WorkMobileNumber = user.WorkMobileNumber,
            Email = userEmail.Email,
            Address = address.Select(a => new UserAddressDto
            {
                Id = a.Id,
                Address = a.Address,
            }).ToList()
        });
    }

    /// <summary>
    /// update user
    /// </summary>
    /// <returns></returns>
    [HttpPost("user-info/{userId}")]
    public IActionResult UpdateUserInfo(int userId, UpdateUserDto model)
    {
        _userService.Update(new Users
        {
            Id = userId,
            Password = model.Password,
            Avatar = model.Avatar != null && model.Avatar.Length > 0 ? Convert.FromBase64String(model.Avatar) : null,
            FirstName = model.FirstName,
            LastName = model.LastName,
            OtherUsableEmail = model.OtherUsableEmail,
            PersonalMobileNumber = model.PersonalMobileNumber,
            WorkMobileNumber = model.WorkMobileNumber,
            EmergencyContactEmail = model.EmergencyContactEmail,
        });
        model.Addresses.Where(t => !t.Id.HasValue).Select(a => new UserAddress
        {
            UsersId = userId,
            Address = a.Address,
        }).ToList().ForEach(_userService.AddUserAddress);
        model.Addresses.Where(t => t.Id.HasValue).Select(a => new UserAddress
        {
            Id = a.Id.Value,
            UsersId = userId,
            Address = a.Address,
        }).ToList().ForEach(_userService.UpdateUserAddress);
        return Ok();
    }
}
