namespace Fafnir.Throttle
{
    public interface IThrottleService
    {
        bool IsAllowed(string address);
    }
}
