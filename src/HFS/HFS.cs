using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rary.HFS.HFSDevices;
using Sys = Cosmos.System;
using VFSIO = System.IO;

namespace Rary.HFS
{
    public class HFS
    {
        public Sys.FileSystem.CosmosVFS vfs;
        public HFSDeviceManager devmanager;
        public StringBuilder Path;

        public enum HFSDirType { Devices, VFS }
        
        // TODO: add check to make sure the hfs isn't started twice.
        public HFS(string startpath)
        {
            // vfs init
            EventLogger.LogEvent(new(EventLogger.LogType.Debug, "Initializing VFS...", "HFS.new()"));

            vfs = new();
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(vfs);

            EventLogger.LogEvent(new(EventLogger.LogType.Debug, "Initialized VFS", "HFS.new()"));

            // device init
            EventLogger.LogEvent(new(EventLogger.LogType.Debug, "Initializing devices...", "HFS.new()"));

            devmanager = new();

            EventLogger.LogEvent(new(EventLogger.LogType.Debug, "Initialized devices", "HFS.new()"));

            Path = new();

            Path.Clear();
            Path.Append(startpath);
        }

        public HFSDirType DetermineDirType(string path)
        {
            if (path.StartsWith("/vfs"))
            {
                return HFSDirType.VFS;
            }
            else
            {
                return HFSDirType.Devices;
            }
        }
        public string ExpandPath(string path)
        {
            if (path.StartsWith('/')) { return path; }
            else
            {
                return this.Path.ToString() + path;
            }
        }

        public string ConvertToDOSPath(string path)
        {
            string fullPath = ExpandPath(path);
            if (DetermineDirType(fullPath) == HFSDirType.VFS)
            {
                return fullPath.Replace("/", "\\").Replace("\\vfs\\", "0:\\");
            }
            else
            {
                throw new Exception("/ is reserved for devices!");
            }
        }

        public void AddPathDir(string dir)
        {
            this.Path.Append(dir);
            this.Path.Append('/');
        }

        public void GoBackOneLayer()
        {
            List<string> splitPath = Path.ToString().Split('/').ToList();
            splitPath.RemoveAt(splitPath.Count - 1);
            Path.Clear(); Path.AppendJoin('/', splitPath.ToArray());
        }


        public string[] ListFiles(string path)
        {
            string fullPath = ExpandPath(path);
            if (DetermineDirType(fullPath) == HFSDirType.VFS)
            {
                return VFSIO.Directory.GetFiles(ConvertToDOSPath(fullPath));
            }
            else
            {
                List<string> names = new();
                foreach (HFSDevice device in devmanager.devices)
                {
                    names.Add(device.DevicePath.Replace("/",""));
                }
                return names.ToArray();
            }
        }

        public string[] ListDirs(string path)
        {
            string fullPath = ExpandPath(path);
            if (DetermineDirType(fullPath) == HFSDirType.VFS)
            {
                return VFSIO.Directory.GetDirectories(ConvertToDOSPath(fullPath));
            }
            else
            {
                return new string[] { "vfs" };
            }
        }

        // --- DIRECTORY METHODS --- //
        #region dir methods
        public bool DirExists(string path)
        {
            string fullPath = ExpandPath(path);
            if (DetermineDirType(fullPath) == HFSDirType.VFS)
            {
                return VFSIO.Directory.Exists(ConvertToDOSPath(fullPath));
            }
            else
            {
                return (fullPath == "/vfs/" || fullPath == "/vfs");
            }
        }

        public void CreateDir(string path)
        {
            string fullPath = ExpandPath(path);

            if (DetermineDirType(fullPath) == HFSDirType.VFS)
            {
                VFSIO.Directory.CreateDirectory(ConvertToDOSPath(fullPath));
            }
            else
            {
                throw new Exception("/ is reserved for devices!");
            }
        }

        public void DeleteDir(string path)
        {
            string fullPath = ExpandPath(path);

            if (DetermineDirType(fullPath) == HFSDirType.VFS)
            {
                DeleteDOSDirRecursive(path);
            }
            else
            {
                foreach (HFSDevice dev in devmanager.devices)
                {
                    devmanager.Delete(dev.DevicePath);
                }
            }
        }

        // tbh i expect some kind of recursive error with this function
        private void DeleteDOSDirRecursive(string path)
        {
            path = ExpandPath(path);
            foreach(string dir in VFSIO.Directory.GetDirectories(path))
            {
                DeleteDOSDirRecursive(path + '/' + dir);
            }
            foreach (string file in VFSIO.Directory.GetFiles(path))
            {
                VFSIO.File.Delete(path + '/' + file);
            }
            VFSIO.Directory.Delete(path);
        }
        #endregion

        // --- FILE METHODS  --- //
        #region file methods
        public void Write(string path, string contents)
        {
            string fullPath = ExpandPath(path);
            if (DetermineDirType(fullPath) == HFSDirType.VFS)
            {
                VFSIO.File.WriteAllText(ConvertToDOSPath(fullPath), contents);
            }
            else
            {
                devmanager.Write(path,contents);
            }
        }

        public string Read(string path)
        {
            string fullPath = ExpandPath(path);
            if (DetermineDirType(fullPath) == HFSDirType.VFS)
            {
                return VFSIO.File.ReadAllText(ConvertToDOSPath(fullPath));
            }
            else
            {
                return devmanager.Read(path);
            }
        }

        public void Delete(string path)
        {
            string fullPath = ExpandPath(path);
            if (DetermineDirType(fullPath) == HFSDirType.VFS)
            {
                VFSIO.File.Delete(ConvertToDOSPath(fullPath));
            }
            else
            {
                devmanager.Delete(path);
            }
        }

        #endregion
    }
}
