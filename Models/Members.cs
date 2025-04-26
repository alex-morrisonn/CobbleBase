using SqlSugar;

namespace UserInfoManager.Models;

[SugarTable("mms_Members")]
public class Members
{
    [SugarColumn(IsPrimaryKey = true)]
    public string MemberID { get; set; }
    public string UserName { get; set; }
    public string? Name { get; set; }
    public bool? Name_Visible { get; set; }
    public string? Name_Last { get; set; }
    public bool? NameLast_Visible { get; set; }
    public string? ProfileIntroduction { get; set; }
    public string? Type { get; set; }
    public string? Status { get; set; }
    public int? UserPoints { get; set; }
    public DateTime? PostDate { get; set; }
}
