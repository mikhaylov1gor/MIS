using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MIS.Models.DB;

public class TokenCleanupService : IHostedService, IDisposable
{
    private Timer? _timer;
    private readonly IServiceProvider _serviceProvider; 

    public TokenCleanupService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(CleanOldTokens, null, TimeSpan.Zero, TimeSpan.FromHours(2));
        return Task.CompletedTask;
    }

    private async void CleanOldTokens(object? state)
    {
        using var scope = _serviceProvider.CreateScope(); 
        var dbContext = scope.ServiceProvider.GetRequiredService<MisDbContext>(); 

        var oldTokens = dbContext.TokenBlackList
            .Where(t => t.expirationTime < DateTime.UtcNow.AddHours(-8));

        dbContext.TokenBlackList.RemoveRange(oldTokens);
        await dbContext.SaveChangesAsync();
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
