using Microsoft.Extensions.Logging;

namespace Weekday.Miscellaneous
{
    internal static class LoggingEvents
    {
        public static EventId DbInitializationFailed = new EventId(100, "Database intitialization finished with error.");
        public static EventId NewsEditingFailed = new EventId(200, "Editing of the news object finished with error.");
        public static EventId NewsCreatingFailed = new EventId(201, "Creating of the news object finished with error.");
    }
}
