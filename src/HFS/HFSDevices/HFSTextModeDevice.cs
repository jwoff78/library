using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rary.HFS.HFSDevices
{
    public class HFSTextModeDevice : HFSDevice
    {
        public HFSTextModeDevice()
        {
            DevicePath = "/console";
            SupportsStdin = true;
            SupportsStdout = true;
        }

        public override string stdout()
        {
            return Console.ReadLine();
        }

        public override void stdin(string input)
        {
            Console.Write(input);
        }
    }
}
