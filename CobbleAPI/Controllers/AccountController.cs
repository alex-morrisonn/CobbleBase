using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CobbleAPI.Models;
using CobbleAPI.Service;

namespace CobbleAPI.Controllers;

public class AccountController : Controller
{
    private readonly UsersService _userService;

    public AccountController(UsersService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }


    /// <summary>
    /// 登录接口
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Login(RegisterViewModel model)
    {
        var user = _userService.Authenticate(model.UserEmail.Email, model.User.Password);//查询用户
        if (user.Item1 == null)
        {
            ModelState.AddModelError(string.Empty, "login fail，Please check your email and password.！");
            return View();
        }
        if (!user.Item2.IsValidEmail)
        {
            ModelState.AddModelError(string.Empty, "login fail，Your email has not been verified yet. Please verify it before logging in.");
            return View();
        }
        // 创建用户的声明信息
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Item2.Email),
            new(ClaimTypes.PrimarySid, user.Item1.Id.ToString()),
        };

        // 创建身份信息
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        // 设置认证Cookie的属性
        var authProperties = new AuthenticationProperties
        {

        };
        // 使用HttpContext.SignInAsync方法设置用户的认证信息
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        return RedirectToAction("Index", "Home");//跳转页面
    }

    /// <summary>
    /// 注册页面
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public IActionResult Register()
    {
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
        user.IsValidEmail = true;
        _userService.Update(user);
        return RedirectToAction("Login");
    }
    /// <summary>
    /// 注册
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public IActionResult Register(RegisterViewModel model)
    {
        try
        {
            var regResult = _userService.Register(model.User, model.UserEmail);
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
        return View();
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
        userEmail.IsValidEmail = true;
        _userService.Update(userEmail);
        return RedirectToAction("Login");
    }
}
