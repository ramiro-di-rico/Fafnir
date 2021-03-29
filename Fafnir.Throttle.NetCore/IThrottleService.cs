namespace Fafnir.Throttle.NetCore
{
    public interface IThrottleService
    {
        bool IsAllowed(string address);
    }
}
