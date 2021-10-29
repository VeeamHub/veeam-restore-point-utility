using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//for PowerShell
using System.Management.Automation;
using System.Management.Automation.Runspaces;
//for file IO
using System.IO;
using System.Threading;
using w32FileInfo;

namespace VeeamFileDiff
{
    public partial class frmViewDiff : Form
    {
        private VeeamAppData vad;
        private Form callingForm;
        private mountInfo mountA;
        private mountInfo mountB;
        private Dictionary<String, myTreeElement> treeIndex = new Dictionary<string, myTreeElement>();

        /*
         * Class for storing mount/unmount info for multi-threaded mount/unmount
         */
        private class mountInfo
        {
            private RunspacePool rsp;
            private String uName, uDescription;
            private String mountID, pointID;
            private String mountPath;

            public mountInfo(String restorePtID, String credUserName, String credDescription)
            {
                pointID = restorePtID;
                uName = credUserName;
                uDescription = credDescription;
            }
            public RunspacePool VBRRunspacePool
            {
                get { return rsp; }
                set { rsp = value; }
            }
            public String MountID
            {
                get { return mountID; }
                set { mountID = value; }
            }
            public String RestorePointID
            {
                get { return pointID; }
            }
            public String MountPath
            {
                get { return mountPath; }
                set { mountPath = value; }
            }
            public String UserName
            {
                get { return uName; }
            }
            public String UserDescription
            {
                get { return uDescription; }
            }
        }

        /*
         * Rudimentary tree class for storing raw file data hierarchy
         */
        private class myTreeElement
        {
            private myTreeElement parentNode = null;
            public List<myTreeElement> children = new List<myTreeElement>();
            private List<String> leaves = new List<String>();
            private String nodeVal = null;
            private Int64 aggregateSize = 0;

            public myTreeElement()
            { }
            public myTreeElement Parent
            {
                get { return parentNode; }
                set { parentNode = value; }
            }
            public String NodeIndex
            {
                get { return nodeVal; }
                set { nodeVal = value; }
            }
            public Int64 AggregateSize
            {
                get { return aggregateSize; }
                set { aggregateSize = value; }
            }
            public bool Insert(myTreeElement node)
            {
                children.Add(node);
                return true;
            }
            public bool AddLeaf(String val)
            {
                leaves.Add(val);
                return true;
            }
        }

        public frmViewDiff(Form priorForm, VeeamAppData appData)
        {
            callingForm = priorForm;

            vad = appData;

            InitializeComponent();
        }

        private void frmViewDiff_Load(object sender, EventArgs e)
        {
            tssOperationInFlight.Text = "";
            prgViewDiff.Value = 0;
        }

        private void frmViewDiff_Shown(object sender, EventArgs e)
        {
            this.CenterToScreen();

            Cursor = Cursors.WaitCursor;

            tssOperationInFlight.Text = "Retrieve restore points";
            if (vad.restoPtIDA == "0")
                lblRestoPtA.Text = "Live workload";
            else
                lblRestoPtA.Text = displayPointNames(vad.restoPtIDA);
            prgViewDiff.Value = 50;
            lblRestoPtB.Text = displayPointNames(vad.restoPtIDB);
            prgViewDiff.Value = 100;
            Application.DoEvents();

            prgViewDiff.Value = 0;
            prgViewDiff.Visible = false;
            tssOperationInFlight.Text = "Mounting restore points...";
            Application.DoEvents();

            if (vad.VBRRunSpacePool.RunspacePoolStateInfo.State != RunspacePoolState.Opened)
                vad.VBRRunSpacePool.Open();

            mountA = new mountInfo(vad.restoPtIDA, vad.mntCreds.userName, vad.mntCreds.userDescription);
            mountB = new mountInfo(vad.restoPtIDB, vad.mntCreds.userName, vad.mntCreds.userDescription);

            Thread a = new Thread(mountBackup);
            Thread b = new Thread(mountBackup);

            a.Start(mountA);
            b.Start(mountB);

            a.Join();
            b.Join();

            if (vad.restoPtIDA != "0")
                mountA.MountPath = getMountPath(mountA.MountID);
            else
                mountA.MountPath = ""; //since unknown until user explicitly selects something from the live VM
            mountB.MountPath = getMountPath(mountB.MountID);

            if (vad.restoPtIDA == "0")
                lblFolderA.Text = "Use folder browse dialog to map running VM drive for comparison";
            else
                lblFolderA.Text = mountA.MountPath;
            lblFolderB.Text = mountB.MountPath;

            tssOperationInFlight.Text = "";
            Application.DoEvents();

            Cursor = Cursors.Default;
        }

        private void frmViewDiff_Closing(object sender, EventArgs e)
        {

            cleanupForExit();

            callingForm.Activate();
            callingForm.Visible = true;
            Dispose();
        }
        private String displayPointNames(String restoPtID)
        {
            try
            {
                Pipeline vPipe = vad.VBRRunSpace.CreatePipeline();
                vPipe.Commands.AddScript(String.Format("Get-VBRRestorePoint | Where {{$_.Id -eq '{0}'}}", restoPtID));
                ICollection<PSObject> results = vPipe.Invoke();

                if (vPipe.Error.Count == 0)
                    foreach (PSObject pso in results) //will only ever be a single result
                        return pso.Members["CreationTime"].Value.ToString();

                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Powershell error retrieving restore point name - {0}", "Veeam File Diff", ex.Message), "Veeam File Diff", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        private static void mountBackup(Object mountObject)
        {
            try
            {
                mountInfo mnt = (mountInfo)mountObject;

                if (mnt.RestorePointID != "0")
                {
                    PowerShell x = PowerShell.Create();

                    x.RunspacePool = mnt.VBRRunspacePool;
                    x.AddScript(String.Format("$restoPt=Get-VBRRestorePoint | Where {{$_.Id -eq '{0}'}}; " +
                                                            "$creds = Get-VBRCredentials -Name '{1}' | Where-Object {{$_.Description -eq '{2}'}}; " +
                                                            "Publish-VBRBackupContent -RestorePoint $restoPt -TargetServerName \"localhost\" -TargetServerCredentials $creds;",
                                                            mnt.RestorePointID, mnt.UserName, mnt.UserDescription));
                    ICollection<PSObject> results = x.Invoke();

                    if (x.InvocationStateInfo.State != PSInvocationState.Failed)
                    {
                        foreach (PSObject pso in results) //will only ever be a single result
                            mnt.MountID = pso.Members["Id"].Value.ToString();
                    }
                    else
                    {
                        MessageBox.Show(String.Format("Error mounting selected restore point - {0}!", mnt.RestorePointID), "Veeam File Diff", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    x.Dispose();
                }
                return;
            }
            catch
            {
                MessageBox.Show("Powershell error mounting selected restore point! Check mount credentials.", "Veeam File Diff", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        public static void unmountBackup(Object mountObject)
        {
            mountInfo mount = (mountInfo)mountObject;
            try
            {
                if (mount.RestorePointID != "0")
                {
                    PowerShell x = PowerShell.Create();

                    x.RunspacePool = mount.VBRRunspacePool;
                    x.AddScript(String.Format("$session = Get-VBRPublishedBackupContentSession -Id '{0}'; Unpublish-VBRBackupContent -Session $session", mount.MountID));
                    ICollection<PSObject> results = x.Invoke();
                    x.Dispose();
                }
            }
            catch
            {
                MessageBox.Show("Powershell error dismounting selected restore point!", "Veeam File Diff", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return ;
            }
            return;
        }
        private String getMountPath(String mountID)
        {
            String purgedMountID = "";

            try
            {
                Pipeline vPipe = vad.VBRRunSpace.CreatePipeline();

                //strip the hyphens out of the mountID!!!!!!!!!
                String[] temp = mountID.Split('-');
                for (int idx = 0; idx < temp.Count(); idx++)
                    purgedMountID = purgedMountID.Insert(purgedMountID.Length, temp[idx]);

                //vPipe.Commands.AddScript(String.Format("Get-IscsiSession | Where-Object {{$_.TargetNodeAddress -clike '*{0}'}} | Get-Disk | Get-Partition | Where-Object {{$_.OperationalStatus -eq 'Online'}} | select AccessPaths -First 1", purgedMountID));
                vPipe.Commands.AddScript(String.Format("Get-IscsiSession | Where-Object {{$_.TargetNodeAddress -clike '*{0}'}} | Get-Disk | Get-Partition | Where-Object {{$_.OperationalStatus -eq 'Online' -and $_.AccessPaths -ne $Null}} | select AccessPaths -First 1", purgedMountID));
                ICollection<PSObject> results = vPipe.Invoke();

                if (vPipe.Error.Count == 0)
                {
                    foreach (PSObject pso in results) //if multiple image disks mounted just need first one to get the leading C:\VeeamFLR\image_mountdescriptor path not full C:\VeeamFLR\image_mountdescriptor\Volume1...VolumeX
                    {
                        String[] paths = (String[])pso.Members["AccessPaths"].Value;
                        //chop the trailing '/VolumeX/' to get the root path of the mount point
                        int idx = paths[0].Substring(0, paths[0].Length - 2).LastIndexOf('\\');
                        return paths[0].Substring(0, idx);
                    }
                }
                else
                    MessageBox.Show("Powershell error retrieving mount path", "Veeam File Diff", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Exception in getMountpath - {0}", ex.Message.ToString()), "Veeam File Diff", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private void cleanupForExit()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                tssOperationInFlight.Text = "Dismounting...";
                Application.DoEvents();

                vad.VBRRunSpacePool.SetMaxRunspaces(2);
                if (vad.VBRRunSpacePool.RunspacePoolStateInfo.State != RunspacePoolState.Opened)
                    vad.VBRRunSpacePool.Open();

                mountA.VBRRunspacePool = vad.VBRRunSpacePool;

                mountB.VBRRunspacePool = vad.VBRRunSpacePool;
                
                //dismount
                Thread a = new Thread(unmountBackup);
                Thread b = new Thread(unmountBackup);

                a.Start(mountA);
                b.Start(mountB);

                a.Join();
                b.Join();

                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show(String.Format("Error cleaning up disk mount - {0}", ex.Message.ToString()), "Veeam File Diff", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnBrowseFolderA_Click(object sender, EventArgs e)
        {
            fbMountedBackup.RootFolder = Environment.SpecialFolder.MyComputer;
            fbMountedBackup.SelectedPath = mountA.MountPath;

            try //open it up so anything can be selected
            {
                if ((fbMountedBackup.ShowDialog() == DialogResult.OK) && (fbMountedBackup.SelectedPath.Length > 0))
            //        if (fbMountedBackup.SelectedPath.Substring(0, mountA.MountPath.Length) == mountA.MountPath)
                        lblFolderA.Text = fbMountedBackup.SelectedPath;
            //        else
            //            MessageBox.Show(String.Format("Folder in the mounted backup must be selected from \"{0}\"", mountA.MountPath), "Veeam File Diff", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch
            {
                MessageBox.Show(String.Format("Folder in the mounted backup must be selected from \"{0}\"", mountA.MountPath), "Veeam File Diff", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnBrowseFolderB_Click(object sender, EventArgs e)
        {
            fbMountedBackup.RootFolder = Environment.SpecialFolder.MyComputer;
            fbMountedBackup.SelectedPath = mountB.MountPath;

            try //open it up so anything can be selected
            {
                if ((fbMountedBackup.ShowDialog() == DialogResult.OK) && (fbMountedBackup.SelectedPath.Length > 0))
            //        if (fbMountedBackup.SelectedPath.Substring(0, mountB.MountPath.Length) == mountB.MountPath)
                        lblFolderB.Text = fbMountedBackup.SelectedPath;
            //        else
            //            MessageBox.Show(String.Format("Folder in the mounted backup must be selected from - \"{0}\"", mountB.MountPath), "Veeam File Diff", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch
            {
                MessageBox.Show(String.Format("Folder in the mounted backup must be selected from - \"{0}\"", mountB.MountPath), "Veeam File Diff", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private myTreeElement initPath(myTreeElement root, myFileInfo file)
        {
            String[] path = file.RelativePath.Split('\\');
            String tmpPath = "...\\";
            myTreeElement node = root;

            try
            {
                String filePath = file.RelativePath.Substring(0, file.RelativePath.LastIndexOf('\\')); //extract the directory component of the filename and check to see if already added to the tree structure
                //if not already added, create full path in tree structure
                foreach (String pathElement in path)
                {
                    if (pathElement != "") //will be true for element 0 of a string of the form "\Volume1\Element1\Element2"
                    {
                        String testPath = String.Format("{0}{1}\\", tmpPath, pathElement);
                        if (!treeIndex.ContainsKey(testPath))
                        {
                            myTreeElement m = new myTreeElement();
                            m.NodeIndex = testPath;
                            m.Parent = node;
                            node.children.Add(m);
                            treeIndex.Add(testPath, m);
                            node = m;
                        }
                        else
                        {
                            node = treeIndex[testPath];
                        }
                        tmpPath = testPath;
                    }
                }
                return (node);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Error initializing tree structure - {0}", ex.Message), "Veeam File Diff", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (null);
            }
        }
        private myTreeElement buildTreeStructure(Dictionary<String, myFileInfo> fileData)
        {
            myTreeElement myRootNode = new myTreeElement();

            try
            {
                myRootNode.NodeIndex = "...\\";

                treeIndex.Add("...\\", myRootNode);

                foreach (KeyValuePair<String, myFileInfo> file in fileData)
                {
                    myTreeElement node = initPath(myRootNode, file.Value);
                    node.AddLeaf(file.Value.RelativePath);
                    if (!file.Value.isSymLink)
                        node.AggregateSize = file.Value.Length;
                    else
                        node.AggregateSize = 0;
                }
                return myRootNode;
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Error building tree structure - {0}", ex.Message.ToString()), "Veeam File Diff", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private void populateUI(myTreeElement treeRoot, TreeNode uiTreeNode, TreeView ui)
        {
            TreeNode uiTree;

            try
            {
                if (uiTreeNode == null)
                    uiTree = ui.Nodes.Add("...\\");
                else
                {
                    int secondToLastSlash = treeRoot.NodeIndex.Substring(0, treeRoot.NodeIndex.Length - 1).LastIndexOf('\\');
                    uiTree = uiTreeNode.Nodes.Add(String.Format("{0} - ({1} KB)", treeRoot.NodeIndex.Substring(secondToLastSlash + 1, treeRoot.NodeIndex.Length - (secondToLastSlash + 2)), (((float)(treeRoot.AggregateSize / 1024)).ToString("F2"))));
                }
                foreach (myTreeElement child in treeRoot.children)
                {
                    populateUI(child, uiTree, ui);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Error populating treeview - {0}", ex.Message.ToString()), "Veeam File Diff", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private Int64 aggregateSize(myTreeElement treeRoot)
        {

            foreach (myTreeElement te in treeRoot.children)
                treeRoot.AggregateSize += aggregateSize(te);
            
            return treeRoot.AggregateSize;
        }
        private void btnCompare_Click(object sender, EventArgs e)
        {
            Dictionary<String, myFileInfo> uniqueToA, uniqueToB, commonChanged;
            myTreeElement treeRoot;

            Cursor = Cursors.WaitCursor;

            try
            {
                compareFolders(lblFolderA.Text, lblFolderB.Text, out uniqueToA, out uniqueToB, out commonChanged);

                prgViewDiff.Visible = true;
                prgViewDiff.Value = 0;
                tssOperationInFlight.Text = "Populating point A unique files data...";
                Application.DoEvents();

                treeIndex.Clear();
                treeRoot = buildTreeStructure(uniqueToA);
                aggregateSize(treeRoot);
                trvABDiff.Nodes.Clear();
                populateUI(treeRoot, null, trvABDiff);
                lblPointASize.Text = String.Format("{0} GB", ((float)treeRoot.AggregateSize / (float)1073741824).ToString("F3"));
                if (trvABDiff.Nodes[0].Nodes.Count > 0)
                    trvABDiff.Nodes[0].Expand();

                prgViewDiff.Value = 33;
                tssOperationInFlight.Text = "Populating point B unique files data...";
                Application.DoEvents();

                treeIndex.Clear();
                treeRoot = buildTreeStructure(uniqueToB);
                aggregateSize(treeRoot);
                trvBADiff.Nodes.Clear();
                populateUI(treeRoot, null, trvBADiff);
                lblPointBSize.Text = String.Format("{0} GB", ((float)treeRoot.AggregateSize / (float)1073741824).ToString("F3"));
                if (trvBADiff.Nodes[0].Nodes.Count > 0)
                    trvBADiff.Nodes[0].Expand();

                prgViewDiff.Value = 66;
                tssOperationInFlight.Text = "Populating common changed files data...";
                Application.DoEvents();

                treeIndex.Clear();
                treeRoot = buildTreeStructure(commonChanged);
                aggregateSize(treeRoot);
                trvFileDiff.Nodes.Clear();
                populateUI(treeRoot, null, trvFileDiff);
                lblChangedSize.Text = String.Format("{0} GB", ((float)treeRoot.AggregateSize / (float)1073741824).ToString("F3"));
                if (trvFileDiff.Nodes[0].Nodes.Count > 0)
                    trvFileDiff.Nodes[0].Expand();

                Cursor = Cursors.Default;
                prgViewDiff.Value = 100;
                Application.DoEvents();
                prgViewDiff.Value = 0;
                prgViewDiff.Visible = false;
                tssOperationInFlight.Text = "";
            }
            catch (Exception ex)
            {
                prgViewDiff.Visible = false;
                MessageBox.Show(String.Format("Error displaying folder comparison - {0}", ex.Message.ToString()), "Veeam File Diff", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void compareFolders(String srcFolder, String tgtFolder, out Dictionary<String, myFileInfo> filesUniqueToA, out Dictionary<String, myFileInfo> filesUniqueToB, out Dictionary<String, myFileInfo> commonChangedFiles)
        {
            filesUniqueToA = new  Dictionary<String, myFileInfo>();
            filesUniqueToB = new Dictionary<String, myFileInfo>();
            commonChangedFiles = new Dictionary<String, myFileInfo>();

            try
            {
                tssOperationInFlight.Text = "Retrieving file information from mounts...";
                Application.DoEvents();

                //process specified mount folders
                w32Files restoPointFiles = new w32Files(srcFolder, mountA.MountPath.Length, tgtFolder, mountB.MountPath.Length); //have to pass base mountpath lengths to provide a basis for skipping the VolumeX symlinks
                restoPointFiles.loadFileData();

                tssOperationInFlight.Text = "Comparing files...";
                Application.DoEvents();
                //grab unique-to-A and shared+modified files
                foreach (KeyValuePair<String, myFileInfo> yy in restoPointFiles.SourceFileDictionary)
                {
                    if (restoPointFiles.TargetFileDictionary.Keys.Contains(yy.Key)) //if file is common to both points
                    {
                        if (restoPointFiles.TargetFileDictionary[yy.Key].Length != yy.Value.Length) //if length changed
                        {
                            myFileInfo tmp = yy.Value;
                            tmp.Length = yy.Value.Length - restoPointFiles.TargetFileDictionary[yy.Key].Length;
                            commonChangedFiles.Add(yy.Value.RelativePath, tmp);
                        }
                        else //also check for modify date diffs
                        {
                            if (yy.Value.LastWrite != restoPointFiles.TargetFileDictionary[yy.Key].LastWrite)
                            {
                                myFileInfo tmp = yy.Value;
                                tmp.Length = yy.Value.Length - restoPointFiles.TargetFileDictionary[yy.Key].Length;
                                commonChangedFiles.Add(yy.Value.RelativePath, tmp);
                            }
                            else //also check for file's encrytion status change
                            {
                                if (yy.Value.IsEncrypted != restoPointFiles.TargetFileDictionary[yy.Key].IsEncrypted)
                                {
                                    myFileInfo tmp = yy.Value;
                                    tmp.Length = yy.Value.Length - restoPointFiles.TargetFileDictionary[yy.Key].Length;
                                    commonChangedFiles.Add(yy.Value.RelativePath, tmp);
                                }
                            }
                        }
                    }
                    else //unique to A
                        filesUniqueToA.Add(yy.Value.RelativePath, yy.Value);
                }
                    
                //grab unique-to-B
                foreach (KeyValuePair<String, myFileInfo> pointBFile in restoPointFiles.TargetFileDictionary)
                {
                    if (!restoPointFiles.SourceFileDictionary.Keys.Contains(pointBFile.Key))
                    {
                        filesUniqueToB.Add(pointBFile.Value.RelativePath, pointBFile.Value);
                    }
                }
                Application.DoEvents();

                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Exception comparing folders - {0}", ex.Message.ToString()), "Veeam File Diff", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}