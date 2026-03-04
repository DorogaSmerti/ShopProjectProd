using System.Collections.Concurrent;

namespace MyFirstProject.BackgroundServices;

public class RateLimitStore : IRateLimitStore
{
    private readonly ConcurrentDictionary<string, int> _cache = new();

    public int IncrementAndGet(string ip)
    {
        return _cache.AddOrUpdate(ip, 1, (ip, oldValue) => oldValue + 1);
    }

    public void CleanAll()
    {
        _cache.Clear();
    }
}