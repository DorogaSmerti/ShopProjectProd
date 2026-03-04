using System.Linq.Expressions;
using MyFirstProject.Middleware;
namespace MyFirstProject.BackgroundServices;

public class RateLimitCleaner : BackgroundService
{
    private readonly IRateLimitStore _store;

    public RateLimitCleaner(IRateLimitStore store)
    {
        _store = store;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var PeriodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(10));

        try
        {

        while (await PeriodicTimer.WaitForNextTickAsync())
        {
             _store.CleanAll();
        }
        }
        catch(OperationCanceledException)
        {
            
        }
    }
}