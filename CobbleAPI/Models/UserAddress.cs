using SqlSugar;

namespace CobbleAPI.Models;

/// <summary>
/// 用户地址表
/// </summary>
[SugarTable("UserAddress")]
public class UserAddress
{
    /// <summary>
    /// 主键
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }
    /// <summary>
    /// 用户id
    /// </summary>
    public int UsersId { get; set; }
    /// <summary>
    /// 地址
    /// </summary>
    public string Address { get; set; }
}
