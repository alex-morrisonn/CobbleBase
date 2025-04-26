using Microsoft.AspNetCore.Mvc;
using UserInfoManager.Models;
using UserInfoManager.Service;

namespace UserInfoManager.Controllers;

public class AccountViewController(UsersService _userService, IConfiguration _configuration) : Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        ViewBag.ApiHost = _configuration.GetSection("Settings")["apihost"];
        return View();
    }

    /// <summary>
    /// 注册页面
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public IActionResult Register()
    {
        ViewBag.ApiHost = _configuration.GetSection("Settings")["apihost"];
        return View();
    }

    /// <summary>
    /// 验证邮箱
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public IActionResult VerifyEmail(string code)
    {
        var user = _userService.GetUserByEmailCode(code);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "code not found");
            return View();
        }
        user.Verified = true;
        _userService.UpdateUserContact(user);
        return RedirectToAction("Login");
    }

    /// <summary>
    /// 验证邮箱验证码
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult VerifyEmailCode(string code)
    {
        var userEmail = _userService.GetUserByEmailCode(code);
        if (userEmail == null)
        {
            ModelState.AddModelError(string.Empty, "Verification code error, please re-enter.");
            return View("VerifyEmailPage");
        }
        userEmail.Verified = true;
        _userService.UpdateUserContact(userEmail);
        return RedirectToAction("Login");
    }

    /// <summary>
    /// 注册
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Register(MembersDto model)
    {
        try
        {
            var regResult = await _userService.RegisterAsync(new Members
            {
                Name = model.Name,
                UserName = model.UserName,
                Name_Visible = model.Name_Visible,
                Status = model.Status,
                PostDate = model.PostDate,
                NameLast_Visible = model.NameLast_Visible,
                Name_Last = model.Name_Last,
                UserPoints = model.UserPoints,
                Type = model.Type,
                ProfileIntroduction = model.ProfileIntroduction
            }, model.Email, model.ParentMemberUserName);
            if (!regResult.Item1)
            {
                ModelState.AddModelError(string.Empty, regResult.Item2);
                return View(model);
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"register failed, {ex.Message}");
            return View(model);
        }
        return RedirectToAction("Login");
    }

    /// <summary>
    /// 验证邮箱页面
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public IActionResult VerifyEmailPage()
    {
        ViewBag.ApiHost = _configuration.GetSection("Settings")["apihost"];
        return View();
    }
}
