using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SqlSugar;
using UserInfoManager.Models;

namespace UserInfoManager.Controllers.Api;

[ApiController]
[Route("module/[controller]")]
public class LookupsController(SqlSugarClient _db) : ControllerBase
{
    /// <summary>
    /// get address
    /// </summary>
    /// <returns></returns>
    [HttpGet("get-lookups")]
    public IActionResult GetAddress(string? keyword, string? lookupType)
    {
        return new JsonResult(new
        {
            Done = 1,
            Data = _db
                .Queryable<Lookups>()
                .WhereIF(!keyword.IsNullOrEmpty(),
                    x => x.LookupCode.Contains(keyword!) ||
                             x.LookupType.Contains(keyword!) ||
                             x.Description.Contains(keyword!))
                .WhereIF(!lookupType.IsNullOrEmpty(), x => x.LookupType == lookupType)
                .ToList()
        });
    }
}
