using SqlSugar;

namespace UserInfoManager.Models;

[SugarTable("glb_Lookups")]
public class Lookups
{
    public long LookupID { get; set; }
    public char LookupSrc { get; set; }
    public string LookupType { get; set; }
    public string LookupCode { get; set; }
    public string Description { get; set; }
}
