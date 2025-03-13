using SqlSugar;

namespace CobbleAPI.Models;

[SugarTable("UserEmail")]
public class UserEmail
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }
    public string Email { get; set; }
    /// <summary>
    /// Email验证Code
    /// </summary>
    public string EmailCode { get; set; }
    /// <summary>
    /// 是否验证邮箱
    /// </summary>
    public bool IsValidEmail { get; set; }
    /// <summary>
    /// 外键，关联用户表
    /// </summary>
    public int UserId { get; set; } 
    /// <summary>
    /// 注册时间
    /// </summary>
    public DateTime RegistrationDate { get; set; }
}
