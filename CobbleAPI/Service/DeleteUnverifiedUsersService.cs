
namespace CobbleAPI.Service;

public class DeleteUnverifiedUsersService : IHostedService, IDisposable
{
    private readonly UsersService _userService;
    private Timer _timer;
    public DeleteUnverifiedUsersService(UsersService userService)
    {
        _userService = userService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(1));
        return Task.CompletedTask;
    }

    private void DoWork(object state)
    {
        _userService.DeleteUnverifiedUsers();
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
