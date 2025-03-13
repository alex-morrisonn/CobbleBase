using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using CobbleAPI.Models;
using CobbleAPI.Service;
using System.Security.Claims;

namespace CobbleAPI.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UsersService _userService;
    public HomeController(ILogger<HomeController> logger, UsersService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    public IActionResult Index()
    {
        var userId = GetUserIdFromClaims();
        var user = _userService.GetUserById(userId);
        var addresses = _userService.GetUserAddresses(userId);
        return View((addresses, user));
    }

    [HttpPost]
    public IActionResult UpdateUser(List<UserAddress> item1, Users item2, IFormFile Avatar)
    {
        var userId = GetUserIdFromClaims();
        item2.Id = userId;
        var user = _userService.GetUserById(userId);
        if (Avatar != null && Avatar.Length > 0)
        {
            using var memoryStream = new MemoryStream();
            Avatar.CopyTo(memoryStream);
            user.Avatar = memoryStream.ToArray();
        }
        user.FirstName = item2.FirstName;
        user.LastName = item2.LastName;
        user.WorkMobileNumber = item2.WorkMobileNumber;
        user.PersonalMobileNumber = item2.PersonalMobileNumber;
        user.OtherUsableEmail = item2.OtherUsableEmail;
        user.EmergencyContactEmail = item2.EmergencyContactEmail;
        _userService.Update(user);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(UserAddress model)
    {
        if (ModelState.IsValid)
        {
            model.UsersId = GetUserIdFromClaims();
            _userService.AddUserAddress(model);
            return RedirectToAction("Index");
        }
        return View(model);
    }
    private int GetUserIdFromClaims()
    {
        var studentIdClaim = User.FindFirst(ClaimTypes.PrimarySid);
        if (studentIdClaim != null)
        {
            return int.Parse(studentIdClaim.Value);
        }
        return 0;
    }
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var address = _userService.GetUserAddressById(id);
        if (address == null)
        {
            return NotFound();
        }
        return View(address);
    }

    [HttpPost]
    public IActionResult Edit(UserAddress model)
    {
        if (ModelState.IsValid)
        {
            model.UsersId = GetUserIdFromClaims();
            _userService.UpdateUserAddress(model);
            return RedirectToAction("Index");
        }
        return View(model);
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var address = _userService.GetUserAddressById(id);
        if (address == null)
        {
            return NotFound();
        }
        return View(address);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        _userService.DeleteUserAddress(id);
        return RedirectToAction("Index");
    }
    /// <summary>
    /// 注销
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        // 清除当前用户的身份认证信息
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // 清除任何之前登录时设置的临时信息
        TempData.Clear();

        return RedirectToAction("Login", "Account");
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
