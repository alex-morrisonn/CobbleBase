using SqlSugar;
using UserInfoManager.Models;

namespace UserInfoManager.Service;

public class DeleteUnverifiedUsersService : IHostedService, IDisposable
{
    private Timer _timer;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public DeleteUnverifiedUsersService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(1));
        return Task.CompletedTask;
    }

    private void DoWork(object state)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var _db = scope.ServiceProvider.GetRequiredService<SqlSugarClient>();
            var sevenDaysAgo = DateTime.Now.AddDays(-7);
            var unverifiedEmails = _db.Queryable<UserContact>()
               .Where(e => !e.Verified.Value && e.PostDate < sevenDaysAgo && e.ContactType == "Email")
               .ToList();

            foreach (var u in unverifiedEmails)
            {
                _db.Deleteable<Members>().Where(u => u.MemberID == u.MemberID).ExecuteCommand();
                _db.Deleteable<UserContact>().Where(u => u.ContactID == u.ContactID).ExecuteCommand();
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}