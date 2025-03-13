using SqlSugar;
using System.ComponentModel.DataAnnotations;

namespace CobbleAPI.Models;

/// <summary>
/// 用户表
/// </summary>
[SugarTable("Users")]
public class Users
{
    /// <summary>
    /// 主键
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }
    /// <summary>
    /// 密码
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
    /// 用户邮箱id
    /// </summary>
    public int EmailId { get; set; }
    /// <summary>
    /// 其他可用邮箱
    /// </summary>
    public string? OtherUsableEmail { get; set; }
    /// <summary>
    /// 工作手机号码
    /// </summary>
    public string? WorkMobileNumber { get; set; }
    /// <summary>
    /// 个人手机号码
    /// </summary>
    public string? PersonalMobileNumber { get; set; }
    /// <summary>
    /// 紧急联系人邮箱
    /// </summary>
    public string? EmergencyContactEmail { get; set; }
    /// <summary>
    /// 头像
    /// </summary>
    public byte[]? Avatar { get; set; }
}
