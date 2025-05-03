﻿using SqlSugar;

namespace UserInfoManager.Models;

[SugarTable("mms_Address")]
public class UserAddress
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long AddressID { get; set; }
    public string? MemberID { get; set; }
    public string AddressType { get; set; }
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? Address3 { get; set; }
    public string? City { get; set; }
    public string? PostCode { get; set; }
    public string? RegionalCouncil { get; set; }
    public string? State { get; set; }
    public long? Country { get; set; }
    public int? PublicPrivate { get; set; }
    public DateTime? PostDate { get; set; }
}
