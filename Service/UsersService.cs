using SendGrid.Helpers.Mail;
using SendGrid;
using SqlSugar;
using UserInfoManager.Models;
using Microsoft.IdentityModel.Tokens;

namespace UserInfoManager.Service;

public class UsersService(SqlSugarClient _db)
{
    /// <summary>
    /// 登录查询
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public (Members, UserContact) Authenticate(string email)
    {
        using (_db)
        {
            var con = _db.Queryable<UserContact>().First(e => e.ContactType == "Email" && e.ContactDetail == email);
            if (con == null) return (null, null);
            return (_db.Queryable<Members>().First(e => e.MemberID == con.MemberID), con);
        }
    }

    /// <summary>
    /// 注册新增
    /// </summary>
    /// <param name="user"></param>
    public async Task<(bool, string)> RegisterAsync(Members user, string email, string? parentUserName)
    {
        if (UserExistsByUserName(user.UserName) != null)
            return (false, "registration failed, the userName is already taken.");
        Members parentUser = null;
        if (!parentUserName.IsNullOrEmpty())
        {
            parentUser = UserExistsByUserName(parentUserName);
            if (parentUser == null)
                return (false, "registration failed, the parentUserName is already taken.");
        }
        if (_db.Queryable<UserContact>().Any(u => u.ContactDetail == email))
            return (false, "registration failed, the email is already taken.");
        string memberId;
        do
        {
            memberId = GenerateRandomString(64);
        } while (_db.Queryable<Members>().Any(t => t.MemberID == memberId));

        var emailCode = GenerateRandomString(6);
        var apiKey = "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";
        var client = new SendGridClient(apiKey);
        var from = new EmailAddress("zhengdonghao0112@gmail.com", "zhengdonghao0112");
        var to = new EmailAddress(email, email);
        var subject = "Register Success";
        var plainTextContent = $"Registration successful, your verification code is: {emailCode}， Please go to the opened page to complete the verification. Click on the link:http://170.64.235.76:8081/AccountView/VerifyEmailPage Enter the verification code for verification.";
        var htmlContent = $"<p>Registration successful, your verification code is: {emailCode}， Please go to the opened page to complete the verification. Click on the link:http://170.64.235.76:8081/AccountView/VerifyEmailPage Enter the verification code for verification.</p>";
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        user.PostDate = DateTime.Now;
        user.MemberID = memberId;

        try
        {
            _db.Ado.BeginTran();
            _db.Insertable(user).ExecuteCommand();
            _db.Insertable(new UserContact
            {
                ContactDetail = email,
                Verified = false,
                ContactType = "Email",
                MemberID = memberId,
                Notes = emailCode,
                PostDate = DateTime.Now,
                ContactID = _db.Queryable<UserContact>().Max(t => t.ContactID) + 1,
            }).ExecuteCommand();
            if (parentUser != null)
                _db.Insertable(new Relationship
                {
                    MemberID_Parent = parentUser.MemberID,
                    MemberID_Child = memberId,
                    Timestamp = DateTime.Now,
                    Notes = emailCode,
                }).ExecuteCommand();
            _db.Ado.CommitTran();
            var result = await client.SendEmailAsync(msg);
            return (true, string.Empty);
        }
        catch (Exception ex)
        {
            _db.Ado.RollbackTran();
            return (false, ex.Message);
        }
    }

    private string GenerateRandomString(int length)
    {
        const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        byte[] randomBytes = new byte[length];
        using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        char[] chars = new char[length];
        for (int i = 0; i < length; i++)
        {
            chars[i] = validChars[randomBytes[i] % validChars.Length];
        }
        return new string(chars);
    }
    /// <summary>
    /// 修改用户
    /// </summary>
    /// <param name="user"></param>
    public void Update(Members user)
    {
        _db.Updateable(user).ExecuteCommand();
    }

    public void UpdateUserContact(UserContact contact)
    {
        _db.Updateable(contact).ExecuteCommand();
    }
    /// <summary>
    /// 注册查询
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public Members UserExistsByUserName(string userName)
    {
        return _db.Queryable<Members>().First(u => u.UserName == userName);
    }

    /// <summary>
    /// 邮箱验证码获取用户
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    public UserContact GetUserByEmailCode(string emailCode)
    {
        return _db.Queryable<UserContact>().Where(t => t.Notes == emailCode).First();
    }
    /// <summary>
    /// 用户Id获取用户
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public Members GetUserByUserId(string userId)
    {
        return _db.Queryable<Members>().Where(t => t.MemberID == userId).First();
    }
    public List<UserAddress> GetUserAddresses(string userId)
    {
        return _db.Queryable<UserAddress>().Where(a => a.MemberID == userId).ToList();
    }

    public List<UserContact> GetUserConnect(string userId)
    {
        return _db.Queryable<UserContact>().Where(a => a.MemberID == userId).ToList();
    }

    public void AddUserAddress(UserAddress address)
    {
        address.PostDate = DateTime.Now;
        _db.Insertable(address).ExecuteCommand();
    }

    public void AddUserConn(UserContact connect)
    {
        connect.ContactID = _db.Queryable<UserContact>().OrderByDescending(t => t.ContactID).First().ContactID + 1;
        connect.PostDate = DateTime.Now;
        _db.Insertable(connect).ExecuteCommand();
    }

    public UserAddress GetUserAddressById(int id)
    {
        return _db.Queryable<UserAddress>().First(a => a.AddressID == id);
    }
    public UserContact GetUserConnectById(int id)
    {
        return _db.Queryable<UserContact>().First(a => a.ContactID == id);
    }
    public List<UserAddress> GetUserAddressByUserId(string userId)
    {
        return _db.Queryable<UserAddress>().Where(a => a.MemberID == userId).ToList();
    }
    public void UpdateUserAddress(UserAddress address)
    {
        _db.Updateable(address).ExecuteCommand();
    }

    public void UpdateUserConn(UserContact conn)
    {
        _db.Updateable(conn).ExecuteCommand();
    }

    public void DeleteUserAddress(int id)
    {
        _db.Deleteable<UserAddress>().Where(a => a.AddressID == id).ExecuteCommand();
    }
    public void DeleteUserConnect(int id)
    {
        _db.Deleteable<UserContact>().Where(a => a.ContactID == id).ExecuteCommand();
    }

    public Members GetUserById(string userId)
    {
        return _db.Queryable<Members>().First(u => u.MemberID == userId);
    }
}
