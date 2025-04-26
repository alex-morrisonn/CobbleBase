using SqlSugar;

namespace UserInfoManager.Models;

[SugarTable("mms_Contact")]
public class UserContact
{
    [SugarColumn(IsPrimaryKey = true)]
    public int ContactID { get; set; }
    public string? MemberID { get; set; }
    public string ContactType { get; set; }
    public string? ContactDetail { get; set; }
    public bool? Verified { get; set; }
    public string? Notes { get; set; }
    public int? PublicPrivate { get; set; }
    public DateTime? PostDate { get; set; }
}
