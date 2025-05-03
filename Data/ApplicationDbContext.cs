using SqlSugar;

namespace UserInfoManager.Data;

public class ApplicationDbContext
{
    public SqlSugarClient Db { get; private set; }

    public ApplicationDbContext(IConfiguration configuration)
    {
        Db = new SqlSugarClient(new ConnectionConfig()
        {
            ConnectionString = configuration.GetConnectionString("DefaultConnection"),
            DbType = DbType.SqlServer,
            IsAutoCloseConnection = true,
            InitKeyType = InitKeyType.Attribute
        });
    }
}
