using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rary
{
    /// <summary>
    /// A simple extender for the Console
    /// </summary>
    public class EventLogger
    {
        /// <summary>
        /// The log list.
        /// </summary>
        public static List<LoggableEvent> events = new();

        /// <summary>
        /// The type of log you will be making for the Log() method.
        /// </summary>
        public enum LogType
        {
            Error,
            Debug,
            Warning
        }

       /// <summary>
       /// Returns the last log in the list.
       /// </summary>
        public static LoggableEvent GetLatestEvent()
        {
            return events.Last();
        }

        /// <summary>
        /// Adds a LoggableEvent to the list of events.
        /// </summary>
        /// <param name="ev">The event that should be logged.</param>
        public static void LogEvent(LoggableEvent ev)
        {
            events.Add(ev);
        }

        /// <summary>
        /// Removes every event that isn't LogType.Error
        /// </summary>
        public static void RemoveAllNonErrorEvents()
        {
            foreach (LoggableEvent ev in events)
            {
                if (ev.eventType != LogType.Error)
                {
                    events.Remove(ev);
                }
            }
        }

        /// <summary>
        /// Returns every single log that has been logged.
        /// </summary>
        public static LoggableEvent[] GetAllEvents()
        {
            return events.ToArray();
        }
    }
}
