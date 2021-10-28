using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

 namespace w32FileInfo
{
    public class myFileInfo
    {
        private String fP;
        private String rP;
        private Int64 fSize;
        private DateTime lastWrite;
        private bool encrypted;

        public myFileInfo(String rPath, String fPath, DateTime fWriteDate)
        {
            fP = fPath;
            rP = rPath;
            lastWrite = fWriteDate;
        }

        public Int64 Length
        {
            get { return fSize; }
            set { fSize = value; }
        }
        public String FullPath
        {
            get { return fP; }
        }

        public String RelativePath
        {
            get { return rP; }
        }

        public DateTime LastWrite
        {
            get { return lastWrite; }
        }

        private Boolean isSymLinked;
        public bool isSymLink
        {
            get { return isSymLinked; }
            set { isSymLinked = value; }
        }
        public bool IsEncrypted
        {
            get { return encrypted; }
            set { encrypted = value; }
        }
    }
    public class w32Files
    {
        private threadFileData sourceData;
        private threadFileData targetData;

        public w32Files(String srcFolder, int srcMountLen, String tgtFolder, int tgtMountLen)
        {
            sourceData = new threadFileData(srcFolder, srcMountLen);
            targetData = new threadFileData(tgtFolder, tgtMountLen);
        }

        private class threadFileData
        {
            private String rF;
            private int rFMountLen;
            private Dictionary<String, myFileInfo> fD;
            private List<String> symLinks;

            public threadFileData(String rootFolder, int rootFolderMountLen)
            {
                rF = rootFolder;
                rFMountLen = rootFolderMountLen;
                fD = new Dictionary<String, myFileInfo>();
                symLinks = new List<String>();
            }

            public String RootFolder
            {
                get { return rF; }
            }

            public List<String> SymLinks
            {
                get { return symLinks; }
            }

            public int RootFolderMountLength
            {
                get { return rFMountLen; }
            }
            public Dictionary<String, myFileInfo> FileDictionary
            {
                get { return fD; }
            }
        }

        private class w32
        {
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            public struct WIN32_FIND_DATA
            {
                public uint dwFileAttributes;
                public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
                public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
                public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
                public uint nFileSizeHigh;
                public uint nFileSizeLow;
                public uint dwReserved0;
                public uint dwReserved1;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
                public string cFileName;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
                public string cAlternateFileName;
            }
            [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
            public static extern bool FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATA lpFindFileData);

            [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern IntPtr FindFirstFile(string lpFileName, out WIN32_FIND_DATA lpFindFileData);

            [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern bool FindClose(IntPtr handle);
        }

        public Dictionary<String, myFileInfo> SourceFileDictionary
        {
            get { return sourceData.FileDictionary; }
        }

        public Dictionary<String, myFileInfo> TargetFileDictionary
        {
            get { return targetData.FileDictionary; }
        }

        public String SourceRootFolder
        {
            get { return sourceData.RootFolder; }
        }

        public String TargetRootFolder
        {
            get { return targetData.RootFolder; }
        }

        private static DateTime filetimeToDateTime(System.Runtime.InteropServices.ComTypes.FILETIME fileTime)
        {
            try
            {
                long hFT2 = (((long)fileTime.dwHighDateTime) << 32) | ((uint)fileTime.dwLowDateTime);
                return DateTime.FromFileTimeUtc(hFT2);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Error in convert legacy date/time - {0}", ex.Message.ToString()), "Veeam File Diff", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (DateTime.Now);
            }
        }

        /*
         * going to have to account for cases where no subfolder is chosen e.g. took the default C:\VeeamFLR\<mounted VM path> which means the mounted volumes are
         * reparse point which will need to be skipped over
         * 
         * ignore for now...
         * maybe bottom up search fixes
         *  
         */
        private bool parentSymlink(String folder, threadFileData thrdData)
        {
            String[] path = folder.Split('\\');
            String tmpPath = null, testPath = null;

            try
            {
                foreach (String pathElement in path)
                {
                    if (tmpPath == null)
                        tmpPath = pathElement;
                    else
                    {
                        testPath = String.Format("{0}\\{1}", tmpPath, pathElement);

                        if (testPath.Length >= thrdData.RootFolder.Length && testPath.Length > (thrdData.RootFolderMountLength+8) ) //"toss" the subfolders upstream from the one(s) we intend to compare
                        {                                                                                                           //and in the case where no subfolder is chosen we have to ignore the
                            if (thrdData.SymLinks.Contains(testPath))                                                               //first level "\VolumeX" folder which is a symlink as it will trigger
                                return true;                                                                                        //the rest of the disk deltas to be 0 since everything will be symlinked
                            tmpPath = testPath;
                        }
                        else
                            tmpPath =  testPath;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public void loadFileData()
        {
            Thread a = new Thread(getFiles);
            Thread b = new Thread(getFiles);

            a.Start(sourceData);
            b.Start(targetData);

            a.Join();
            b.Join();
        }

        public void getFiles(Object thrdData)
        {
            threadFileData t = (threadFileData)thrdData;

            ListDirTree(t.RootFolder, t);           
        }

        private void ListDirTree(String root, threadFileData t)
        {

            string dir;
            dir = string.Format("{0}\\*.*", root);
            w32.WIN32_FIND_DATA fileData;

            try
            {

                IntPtr handle = w32.FindFirstFile(dir, out fileData);
                do
                {
                    uint rval = fileData.dwFileAttributes & 16; //determine whether or not this is a folder
                    if (rval != 0) //if true keep recursing
                    {
                        if (!fileData.cFileName.Equals(".") && !fileData.cFileName.Equals("..")) //ignore "." and ".."
                        {
                            uint symLink = fileData.dwFileAttributes & 1024;  //check to see if reparse point is true
                            if (symLink != 0)
                            {
                                t.SymLinks.Add(String.Format("{0}\\{1}", root, fileData.cFileName));
                            }
                            string newRoot = string.Format("{0}\\{1}", root, fileData.cFileName);
                            ListDirTree(newRoot, t);
                        }
                    }
                    else
                    {
                        DateTime fTime = filetimeToDateTime(fileData.ftLastWriteTime);
                        myFileInfo mf = new myFileInfo(String.Format("{0}\\{1}", root.Substring(t.RootFolder.Length), fileData.cFileName), string.Format("{0}\\{1}", root, fileData.cFileName), fTime);

                        if (parentSymlink(root, t))
                        {
                            mf.Length = 0;
                            mf.isSymLink = true;
                        }
                        else
                        {
                            mf.IsEncrypted = false;
                            uint encryptedFile = fileData.dwFileAttributes & 16384;  //check to see if file is encrypted
                            if (encryptedFile != 0)
                                mf.IsEncrypted = true;

                            //mf.Length = findData.nFileSizeHigh;
                            mf.Length = fileData.nFileSizeLow;
                            mf.isSymLink = false;
                        }
                        t.FileDictionary.Add(String.Format("{0}\\{1}", root.Substring(t.RootFolder.Length), fileData.cFileName), mf);
                    }
                } while (w32.FindNextFile(handle, out fileData));
                w32.FindClose(handle);
                return;
            }
            catch (Exception ex) {
                MessageBox.Show(String.Format("Error in ListDirTree - {0}", ex.Message.ToString()), "Veeam File Diff", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}