using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rary.HFS
{
    /// <summary>
    /// Provides a base for an HFS device, basically /dev but all devices should be in /.
    /// </summary>
    public class HFSDevice
    {
        /// <summary>
        /// The path that the device should be in. Rary expects this to always be on the bottom level or "/".
        /// </summary>
        public string DevicePath;

        /// <summary>
        /// Set this to true if your device can handle stdout aka being read from.
        /// </summary>
        public bool SupportsStdout = false;

        /// <summary>
        /// Set this to true if your device can handle stdin aka being written to.
        /// </summary>
        public bool SupportsStdin = false;

        /// <summary>
        /// Defines what the device will do when it's being requested to be read from.
        /// </summary>
        /// <returns>What the device will return to the HFS.</returns>
        public virtual string stdout()
        {
            return DevicePath + " doesn't have it's stdout method defined. No operation was executed.";
        }

        /// <summary>
        /// Defines what the device will do when it's being requested to be written to.
        /// </summary>
        /// <param name="input">What the HFS wants to write to this device.</param>
        public virtual void stdin(string input)
        {
            return;
        }
    }
}
