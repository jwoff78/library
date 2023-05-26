using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VFSIO = System.IO;

namespace Rary
{
    /// <summary>
    /// A shitty HFS implementation that doesn't work.
    /// </summary>
    [Obsolete("This class is cursed so now it's deprecated. Use the new HFS class instead.", true)]
    public class HFSOld
    {

        public Cosmos.System.FileSystem.CosmosVFS vfs;
        public Ramdisk rd;
        public StringBuilder Path;

        public enum HFSDirType { Ramdisk, VFS }


        /// <param name="EnableVFS">An option that toggles the HDD or /vfs mount point.</param>
        public HFSOld(string StartingPath)
        {
            this.Path = new();
            this.Path.Clear(); this.Path.Append(StartingPath);

            // ramdisk init
            EventLogger.LogEvent(new(EventLogger.LogType.Debug, "Initializing Ramdisk...", "HFS.new()"));

            rd = new();

            EventLogger.LogEvent(new(EventLogger.LogType.Debug, "Initialized Ramdisk", "HFS.new()"));




            // vfs init
            EventLogger.LogEvent(new(EventLogger.LogType.Debug, "Initializing VFS...", "HFS.new()"));

            vfs = new();
            Cosmos.System.FileSystem.VFS.VFSManager.RegisterVFS(vfs);

            EventLogger.LogEvent(new(EventLogger.LogType.Debug, "Initialized VFS", "HFS.new()"));
        }

        public HFSDirType DetermineDirType(string path)
        {
            if (path.StartsWith("/vfs"))
            {
                return HFSDirType.VFS;
            }
            else
            {
                return HFSDirType.Ramdisk;
            }
        }

        public string ExpandPath(string path)
        {
            if (path.StartsWith('/')) { return path; }
            else
            {
                return (this.Path+path);
            }
        }

        public string ConvertToVFSPath(string path)
        {
            string fullPath = ExpandPath(path);
            if (DetermineDirType(fullPath) == HFSDirType.VFS)
            {
                return fullPath.Replace("/", "\\").Replace("\\vfs\\", "0:\\");
            }
            else
            {
                throw new Exception("Ramdisk directories are not supported!");
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

        public string[] ListFiles()
        {
            string fullPath = ExpandPath(this.Path.ToString());
            if (DetermineDirType(fullPath) == HFSDirType.VFS)
            {
                return VFSIO.Directory.GetFiles(ConvertToVFSPath(fullPath));
            }
            else
            {
                return rd.ListRD(); // <- cursed line of code
            }
        }

        public string[] ListDirs()
        {
            string fullPath = ExpandPath(this.Path.ToString());
            if (DetermineDirType(fullPath) == HFSDirType.VFS)
            {
                return VFSIO.Directory.GetDirectories(ConvertToVFSPath(fullPath));
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
                return VFSIO.Directory.Exists(ConvertToVFSPath(fullPath));
            }
            else
            {
                throw new Exception("Ramdisk directories are not supported!");
            }
        }

        public void CreateDir(string path)
        {
            string fullPath = ExpandPath(path);

            if (DetermineDirType(fullPath) == HFSDirType.VFS)
            {
                VFSIO.Directory.CreateDirectory(ConvertToVFSPath(fullPath));
            }
            else
            {
                throw new Exception("Ramdisk directories are not supported!");
            }
        }

        public void DeleteDir(string path)
        {
            string fullPath = ExpandPath(path);

            if (DetermineDirType(fullPath) == HFSDirType.VFS)
            {
                throw new NotImplementedException();
            }
            else
            {
                throw new Exception("Ramdisk directories are not supported!");
            }
        }
        #endregion

        // --- FILE METHODS  --- //
        #region file methods
        public void WriteText(string path, string contents)
        {
            string fullPath = ExpandPath(path);
            if (DetermineDirType(fullPath) == HFSDirType.VFS)
            {
                VFSIO.File.WriteAllText(ConvertToVFSPath(fullPath), contents);
            }
            else
            {
                List<char> rdpath = fullPath.ToCharArray().ToList();
                rdpath.RemoveAt(0);
                rd.WriteText(rdpath.ToString(),contents);
            }
        }

        public string ReadText(string path)
        {
            string fullPath = ExpandPath(path);
            if (DetermineDirType(fullPath) == HFSDirType.VFS)
            {
                return VFSIO.File.ReadAllText(ConvertToVFSPath(fullPath));
            }
            else
            {
                List<char> rdpath = fullPath.ToCharArray().ToList();
                rdpath.RemoveAt(0);
                return rd.ReadText(rdpath.ToString());
            }
        }

        public byte[] ReadBytes(string path)
        {
            string fullPath = ExpandPath(path);
            if (DetermineDirType(fullPath) == HFSDirType.VFS)
            {
                return VFSIO.File.ReadAllBytes(ConvertToVFSPath(fullPath));
            }
            else
            {
                List<char> rdpath = fullPath.ToCharArray().ToList();
                rdpath.RemoveAt(0);
                return rd.ReadBytes(rdpath.ToString());
            }
        }

        #endregion
    }
}