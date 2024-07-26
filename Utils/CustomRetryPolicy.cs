using Microsoft.AspNetCore.SignalR.Client;

namespace HardWareMonitorService.Utils;

public class CustomRetryPolicy : IRetryPolicy
{
    public TimeSpan? NextRetryDelay(RetryContext retryContext)
    {
        return TimeSpan.FromSeconds(5);;
    }
}