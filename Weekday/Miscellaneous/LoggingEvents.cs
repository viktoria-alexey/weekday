using Microsoft.Extensions.Logging;

namespace Weekday.Miscellaneous
{
    internal static class LoggingEvents
    {
        public static EventId DbInitializationFailed = new EventId(100, "Database intitialization finished with error.");
    }
}
