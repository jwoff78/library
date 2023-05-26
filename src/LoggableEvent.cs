using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rary
{
    public class LoggableEvent
    {
        public int eventUuid;
        public EventLogger.LogType eventType;
        public string eventTimeStamp;
        public string eventData;
        public string eventCaller;

        public LoggableEvent(EventLogger.LogType type, string data, string caller)
        {
            eventUuid = GenerateUuidRecursive();
            eventType = type;
            eventData = data;
            eventCaller = caller;
            eventTimeStamp = Cosmos.Core.CPU.GetCPUUptime().ToString();
        }

        private int GenerateUuid()
        {
            int attempt = (new Random()).Next();
            foreach (LoggableEvent ev in EventLogger.events)
            {
                if (attempt == ev.eventUuid)
                {
                    return -1;
                }
            }
            return attempt;
        }

        private int GenerateUuidRecursive()
        {
            while (true)
            {
                int attempt = GenerateUuid();
                if (attempt != -1)
                {
                    return attempt;
                }
            }
        }
    }
}