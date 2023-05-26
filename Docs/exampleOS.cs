using System;
using Rary;
using Rary.HFS;
using Rary.GL;
using Sys = Cosmos.System;
using Rary.GL.Fonts;
using Cosmos.Core.Memory;
using System.Threading;

namespace exampleOS
{
    public class Kernel : Sys.Kernel
    {
        HFS hfs;
        SVGAIICanvas canvas;
        Rary.GL.Terminal term;
        protected override void BeforeRun()
        {
            Console.WriteLine("initializing canvas...");

            canvas = new(1920,1080);

            term = new(canvas,Font.Fallback);
            hfs = new("/");

            term.WriteLine("Rary exampleOS booted & initialized canvas successfully");
            hfs.Write("/vfs/vfs.txt","this text is being stored in /vfs/vfs.txt \n");
            term.WriteLine(hfs.Read("/vfs/vfs.txt"));

            term.WriteLine(hfs.Path.ToString());

            foreach (string item in hfs.ListDirs())
            {
                term.WriteLine(item);
            }

            foreach (string item in hfs.ListFiles())
            {
                term.WriteLine(item);
            }

            term.WriteLine();
            hfs.AddPathDir("vfs");

            term.WriteLine(hfs.Path.ToString());

            foreach (string item in hfs.ListDirs())
            {
                term.WriteLine(item);
            }

            foreach (string item in hfs.ListFiles())
            {
                term.WriteLine(item);
            }

            EventLogger.LogEvent(new(EventLogger.LogType.Error, "Test error", "demo event"));
            EventLogger.LogEvent(new(EventLogger.LogType.Warning, "Test warning", "demo event"));
            Thread.Sleep(4);
            EventLogger.LogEvent(new(EventLogger.LogType.Debug, "Test debug", "demo event"));

            term.WriteLine();

            foreach (LoggableEvent ev in EventLogger.GetAllEvents())
            {
                term.Write($"Event UUID: {ev.eventUuid} - Event timestamp: {ev.eventTimeStamp} - Event caller: {ev.eventCaller} --- {ev.eventData}\n");
            }
        }

        protected override void Run()
        {
        }
    }
}
