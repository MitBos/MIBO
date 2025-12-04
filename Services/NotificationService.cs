namespace MIBO.Services;

public enum ToastLevel
{
    Info,
    Success,
    Warning,
    Error
}

public record ToastMessage(string Title, string? Body, ToastLevel Level, DateTimeOffset CreatedAt);

public class NotificationService
{
    public static NotificationService Instance { get; } = new();

    public event Action<ToastMessage>? OnShow;

    public void ShowInfo(string title, string? body = null) => Publish(title, body, ToastLevel.Info);

    public void ShowSuccess(string title, string? body = null) => Publish(title, body, ToastLevel.Success);

    public void ShowWarning(string title, string? body = null) => Publish(title, body, ToastLevel.Warning);

    public void ShowError(string title, string? body = null) => Publish(title, body, ToastLevel.Error);

    private void Publish(string title, string? body, ToastLevel level)
    {
        OnShow?.Invoke(new ToastMessage(title, body, level, DateTimeOffset.UtcNow));
    }
}
