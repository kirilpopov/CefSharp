namespace CefSharp
{
    using Internals;

    public interface ILogHandler
    {
        void Log(LogMessage message);
    }
}