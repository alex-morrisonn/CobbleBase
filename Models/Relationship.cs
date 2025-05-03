using SqlSugar;

namespace UserInfoManager.Models;

[SugarTable("mms_Relationship")]
public class Relationship
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long RelationshipNo { get; set; }
    public string MemberID_Parent { get; set; }
    public string MemberID_Child { get; set; }
    public string? Notes { get; set; }
    public DateTime? Timestamp { get; set; }

}
