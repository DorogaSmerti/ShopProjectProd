namespace MyFirstProject.Services;

public class Error(string code, string message)
{
    public string Code { get; } = code;
    public string Message { get; } = message;

    public static readonly Error None = new(string.Empty, string.Empty);
}