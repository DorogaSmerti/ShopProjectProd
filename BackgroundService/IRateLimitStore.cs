namespace MyFirstProject.BackgroundServices;

public interface IRateLimitStore
{
    int IncrementAndGet(string ip);
    void CleanAll();
}