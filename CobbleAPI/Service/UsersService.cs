using SqlSugar;
using System.Net.Mail;
using System.Net;
using CobbleAPI.Models;

namespace CobbleAPI.Service;

public class UsersService(SqlSugarClient db)
{
    private readonly SqlSugarClient _db = db;

    /// <summary>
    /// 登录查询
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public (Users, UserEmail) Authenticate(string email, string password)
    {
        var userEmail = _db.Queryable<UserEmail>().First(e => e.Email == email);
        if (userEmail != null)
        {
            var user = _db.Queryable<Users>().First(u => u.EmailId == userEmail.Id && u.Password == password);
            return (user, userEmail);
        }
        return (null, null);
    }

    public (bool, string) Register(Users user, UserEmail email)
    {
        //用户是否被注册
        if (UserExistsByEmail(email.Email))
            return (false, "registration failed, the email is already taken.");

        email.IsValidEmail = false;
        email.EmailCode = GenerateRandomString(6);
        // 设置发件人邮箱和授权码
        string fromEmail = "271520594@qq.com";
        string password = "hglelziqlsdhbibh";
        // 设置收件人邮箱
        string toEmail = email.Email;

        // 创建邮件对象
        MailMessage mail = new()
        {
            From = new MailAddress(fromEmail)
        };
        mail.To.Add(toEmail);
        mail.Subject = "Register Success";
        mail.Body = $"Registration successful, your verification code is: {email.EmailCode}， Please go to the opened page to complete the verification. Click on the link:http://142.93.0.167/Account/VerifyEmailPage Enter the verification code for verification.";

        mail.IsBodyHtml = false; // 如果正文是HTML格式，设置为true
                                 // 设置SMTP客户端
        SmtpClient smtpClient = new("smtp.qq.com")
        {
            Port = 465, // QQ邮箱SMTP端口
            Credentials = new NetworkCredential(fromEmail, password),
            EnableSsl = true // 启用SSL加密
        };
        // 发送邮件
        //smtpClient.Send(mail);
        email.RegistrationDate = DateTime.Now;
        email.Id = _db.Insertable(email).ExecuteReturnIdentity();
        user.EmailId = email.Id;
        user.Id = _db.Insertable(user).ExecuteReturnIdentity();
        email.UserId = user.Id;
        _db.Updateable(email).ExecuteCommand();
        return (true, string.Empty);
        string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string([.. Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)])]);
        }
    }

    /// <summary>
    /// 修改用户
    /// </summary>
    /// <param name="user"></param>
    public void Update(Users user)
    {
        _db.Updateable(user).ExecuteCommand();
    }

    /// <summary>
    /// 修改用户邮箱
    /// </summary>
    /// <param name="userEmail"></param>
    public void Update(UserEmail userEmail)
    {
        _db.Updateable(userEmail).ExecuteCommand();
    }
    /// <summary>
    /// 注册查询
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public bool UserExistsByEmail(string email)
    {
        return _db.Queryable<UserEmail>().Any(u => u.Email == email);
    }

    /// <summary>
    /// 邮箱验证码获取用户
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    public UserEmail GetUserByEmailCode(string emailCode)
    {
        return _db.Queryable<UserEmail>().Where(t => t.EmailCode == emailCode).First();
    }
    /// <summary>
    /// 用户Id获取用户
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public UserEmail GetUserByUserId(int userId)
    {
        return _db.Queryable<UserEmail>().Where(t => t.UserId == userId).First();
    }
    public List<UserAddress> GetUserAddresses(int userId)
    {
        return _db.Queryable<UserAddress>().Where(a => a.UsersId == userId).ToList();
    }

    public void AddUserAddress(UserAddress address)
    {
        _db.Insertable(address).ExecuteCommand();
    }

    public UserAddress GetUserAddressById(int id)
    {
        return _db.Queryable<UserAddress>().First(a => a.Id == id);
    }
    public List<UserAddress> GetUserAddressByUserId(int userId)
    {
        return _db.Queryable<UserAddress>().Where(a => a.UsersId == userId).ToList();
    }
    public void UpdateUserAddress(UserAddress address)
    {
        _db.Updateable(address).ExecuteCommand();
    }

    public void DeleteUserAddress(int id)
    {
        _db.Deleteable<UserAddress>().Where(a => a.Id == id).ExecuteCommand();
    }
    /// <summary>
    /// 删除七天未验证的用户
    /// </summary>
    public void DeleteUnverifiedUsers()
    {
        var sevenDaysAgo = DateTime.Now.AddDays(-7);
        var unverifiedEmails = _db.Queryable<UserEmail>()
           .Where(e => !e.IsValidEmail && e.RegistrationDate < sevenDaysAgo)
           .ToList();

        foreach (var email in unverifiedEmails)
        {
            _db.Deleteable<Users>().Where(u => u.EmailId == email.Id).ExecuteCommand();
            _db.Deleteable<UserEmail>().Where(e => e.Id == email.Id).ExecuteCommand();
        }
    }

    public Users GetUserById(int userId)
    {
        return _db.Queryable<Users>().First(u => u.Id == userId);
    }
}
