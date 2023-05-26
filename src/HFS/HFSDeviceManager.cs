using Rary.HFS.HFSDevices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rary.HFS
{
    public class HFSDeviceManager
    {
        public List<HFSDevice> devices;

        public HFSDeviceManager()
        {
            // device init
            EventLogger.LogEvent(new(EventLogger.LogType.Debug, "Initializing Device list...", "HFSDeviceManager.new()"));

            devices = new();

            EventLogger.LogEvent(new(EventLogger.LogType.Debug, "Initialized Device list", "HFSDeviceManager.new()"));

            // text device init
            EventLogger.LogEvent(new(EventLogger.LogType.Debug, "Initializing VGA text device...", "HFSDeviceManager.new()"));

            devices.Add(new HFSTextModeDevice());

            EventLogger.LogEvent(new(EventLogger.LogType.Debug, "Initialized VGA text device", "HFSDeviceManager.new()"));
        }

        public string Read(string devicePath)
        {
            if (!devicePath.StartsWith('/'))
            {
                devicePath = devicePath.Insert(0, "/"); // insert / at beginning if it doesn't, as devices should always be at the root
            }



            foreach (HFSDevice device in devices)
            {
                if (device.DevicePath == devicePath)
                {
                    if (device.SupportsStdout) {
                        return device.stdout();
                    }
                    else
                    {
                        throw new Exception("HFS device \"" + devicePath + "\" doesn't support stdout!");
                    }
                }
            }
            throw new Exception("HFS device \"" + devicePath + "\" doesn't exist!");
        }



        public void Write(string devicePath, string data)
        {
            if (!devicePath.StartsWith('/'))
            {
                devicePath = devicePath.Insert(0, "/"); // insert / at beginning if it doesn't, as devices should always be at the root
            }



            foreach (HFSDevice device in devices)
            {
                if (device.DevicePath == devicePath)
                {
                    if (device.SupportsStdin)
                    {
                        device.stdin(data);
                        return;
                    }
                    else
                    {
                        throw new Exception("HFS device \"" + devicePath + "\" doesn't support stdin!");
                    }
                }
            }
            throw new Exception("HFS device \"" + devicePath + "\" doesn't exist!");
        }

        public void Delete(string devicePath)
        {
            if (!devicePath.StartsWith('/'))
            {
                devicePath = devicePath.Insert(0, "/"); // insert / at beginning if it doesn't, as devices should always be at the root
            }



            foreach (HFSDevice device in devices)
            {
                devices.Remove(device);
                return;
            }
            throw new Exception("HFS device \"" + devicePath + "\" doesn't exist!");
        }
    }
}
