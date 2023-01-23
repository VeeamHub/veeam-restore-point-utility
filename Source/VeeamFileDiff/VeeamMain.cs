using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
//for Powershell add system.management.automation.dll reference from \windows\assembly\gac_msil\s...
using System.Management.Automation;
using System.Management.Automation.Runspaces;
//facilitate elevated backup/restore via opensource "processprivileges" class reference ProcessPrivileges.dll from - https://processprivileges.codeplex.com/
using ProcessPrivileges;
//also needed the following for process object
using System.Diagnostics;

namespace VeeamFileDiff
{
    public partial class frmVeeamMain : Form
    {
        private readonly string[] VBRPSModule = new string[] { "Veeam.Backup.PowerShell" };
        private VeeamAppData appData = new VeeamAppData();

        public frmVeeamMain()
        {
            InitializeComponent();
            slVeeam.Text = "";
        }

        private void frmVeeamMain_Shown(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            Application.DoEvents();
            try {
                this.CenterToScreen();
                //Add Veeam Powershell module
                slVeeam.Text = "Adding Veeam Powershell module";
                InitialSessionState init = InitialSessionState.CreateDefault();
                init.ImportPSModule(VBRPSModule); 

                //Create PS runspace
                try {
                    slVeeam.Text = "Creating Powershell runspace";
                    appData.VBRRunSpace = RunspaceFactory.CreateRunspace(init);
                    appData.VBRRunSpacePool = RunspaceFactory.CreateRunspacePool(init); // for parallel dismount...

                    //Open PS runspace
                    try {
                        slVeeam.Text = "Opening Powershell runspace";
                        appData.VBRRunSpace.Open();

                        if (verifyModuleLoad()) {
                            initBackupList();
                            trvBackups.Nodes[0].Expand();

                            initJobWorkloads();

                            btnChoosePoints.Enabled = true;
                        }
                        else {
                           MessageBox.Show(String.Format("Error opening Powershell module - '{0}'", VBRPSModule[0]), "Veeam File Diff", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch(Exception ex) {
                        MessageBox.Show(String.Format("Error opening Powershell runspace - {0}", ex.Message.ToString()), "Veeam File Diff", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch {
                    MessageBox.Show("Error creating Powershell runspace", "Veeam File Diff", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex) {
                MessageBox.Show(String.Format("Error initializing Veeam Powershell module - {0}", ex.Message.ToString()), "Veeam File Diff", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally {
                slVeeam.Text = "";
                prgVeeam.Visible = false;
                this.Cursor = Cursors.Default;
            }
        }

        private void frmVeeamMain_Load(object sender, EventArgs e)
        {
            btnChoosePoints.Enabled = false;

            //facilitate elevated backup/restore
            Process process = Process.GetCurrentProcess();
            process.EnablePrivilege(Privilege.Backup);
            process.EnablePrivilege(Privilege.Restore);
        }

        private void frmVeeamMain_FormClosing(object sender, FormClosingEventArgs e) {

            Cursor = Cursors.WaitCursor;
            Application.DoEvents();
            //probably unnecessary
            Process process = Process.GetCurrentProcess();
            process.DisablePrivilege(Privilege.Backup);
            process.DisablePrivilege(Privilege.Restore);

            //Cursor = Cursors.Default;
        }
        
        private Boolean verifyModuleLoad()
        {
            Pipeline psPipe;
            Boolean rval = false;

            try {
                psPipe = appData.VBRRunSpace.CreatePipeline();
                psPipe.Commands.AddScript(String.Format("Get-Module -Name '{0}'", VBRPSModule[0]));
                ICollection<PSObject> results = psPipe.Invoke();

                if (results.Count > 0)
                    rval = true;
                psPipe.Dispose();
                return (rval);
            }
            catch {
                return (false);
            }
        }

        //
        // due to changes with Get-VBRBackup in v12 per job types need to check VBR platform
        //
        private int detectVBRPlatform()
        {
            Pipeline vPipe;
            String verString;

            slVeeam.Text = "Detecting VBR platform version";
            prgVeeam.Visible = false;
            Application.DoEvents();

            try
            {
                vPipe = appData.VBRRunSpace.CreatePipeline();
                //vPipe.Commands.AddScript("Get-WmiObject -Class Win32_Product | Where-Object {$_.Name -eq 'Veeam Backup & Replication Server'} | Select-Object Version");
                // much faster way to accomplish from - https://devblogs.microsoft.com/scripting/use-powershell-to-find-installed-software/
                vPipe.Commands.AddScript("Get-ItemProperty HKLM:\\Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\* |  Where-Object {$_.DisplayName -eq 'Veeam Backup & Replication Server'} | Select-Object DisplayName, DisplayVersion");
                ICollection<PSObject> results = vPipe.Invoke();

                if (vPipe.Error.Count == 0)
                {
                    foreach (PSObject pso in results) //should only be one...
                    {
                        //extract VBR major version
                        verString = pso.Properties["DisplayVersion"].Value.ToString().Split('.')[0];
                        vPipe.Dispose();
                        return int.Parse(verString);
                    }
                }
                vPipe.Dispose();
                return 0;
            }
            catch(Exception ex)
            {
                MessageBox.Show(String.Format("Error detecting VBR platform version - {0}", ex.Message.ToString()), "Veeam File Diff", MessageBoxButtons.OK, MessageBoxIcon.Error);
                slVeeam.Text = "";
                Application.DoEvents();
                return 0;
            }
        }

        //
        // make v12+ the new default
        //
        private Boolean initBackupList()
        {
            Pipeline vPipe;
            Boolean rval = false;
            String psScriptStr;

            try {
                trvBackups.Nodes.Add("Backups"); //"seed" the treeview

                switch (detectVBRPlatform()) {
                    case 10:
                    case 11: //v11-
                        psScriptStr = "Get-VBRBackup | ? {$_.JobType -eq 'Backup' -or $_.JobType -eq 'EndpointBackup' -or $_.JobType -eq 'EpAgentManagement'} | Select-Object JobName, JobType, Id";
                        break;
                    default:
                        //added updated v12 job types and 'VmbApiPolicyTempJob' for RHV, AHV backups; might need to add "CloudBackup" and "ArchiveBackup"????
                        //ok removed VmbApiPolicyTemplateJob for now as it breaks the subsequent 'get-vbrbackup' call 
                        psScriptStr = "Get-VBRBackup | ? {$_.JobType -eq 'Backup' -or $_.JobType -eq 'PerVmParentBackup' -or $_.JobType -eq 'EndpointBackup' -or $_.JobType -eq 'EpAgentBackup'} | Select-Object JobName, JobType, Id";
                        break;
                }
                slVeeam.Text = "Retrieving available backups";
                prgVeeam.Visible = true;
                Application.DoEvents();

                vPipe = appData.VBRRunSpace.CreatePipeline();
                vPipe.Commands.AddScript(psScriptStr);
                ICollection<PSObject> results = vPipe.Invoke();

                if (vPipe.Error.Count == 0)
                {
                    TreeNode node = trvBackups.Nodes[0]; //set node pointer to root
                    decimal cnt = 0;
                    foreach (PSObject pso in results)
                    {
                        //add to treeview
                        node.Nodes.Add(pso.Properties["Id"].Value.ToString(), pso.Properties["JobName"].Value.ToString());
                        node.Name = pso.Properties["Id"].Value.ToString();
                        cnt++;
                        prgVeeam.Value = (int)((cnt / results.Count)*100);
                    }
                    rval = true;
                }
                vPipe.Dispose();
                prgVeeam.Value = 100;
                Application.DoEvents();

                return rval;
            }
            catch(Exception ex) {
                MessageBox.Show(String.Format("Error enumerating available backups - {0}", ex.Message.ToString()), "Veeam File Diff", MessageBoxButtons.OK, MessageBoxIcon.Error);
                slVeeam.Text = "";
                return false;
            }
        }

        private Boolean vmTreeNodeExists(TreeNode node, String vmName)
        {
            try {
                if (node.Nodes[vmName] != null)
                    return true;
                else
                    return false;
            }
            catch {
                return false;
            }
        }


        /*
         * 1/4/2023 - modified to filter out ineligible Linux-based workloads
        */
        private Boolean initJobWorkloads()
        {
            try {
                Pipeline vPipe;

                prgVeeam.Value = 0;

                slVeeam.Text = "Retrieving available workloads";
                decimal cnt = 0;

                foreach (TreeNode backup in trvBackups.Nodes[0].Nodes) {
                    vPipe = appData.VBRRunSpace.CreatePipeline();

                    //PS query will return unique image backup names w/legitimate restore points
                    vPipe.Commands.AddScript(String.Format("Get-VBRBackup -Name '{0}' | Get-VBRRestorePoint | Select-Object -Unique VmName, GuestInfo", backup.Text));
                    ICollection<PSObject> results = vPipe.Invoke();

                    if (vPipe.Error.Count == 0) {

                        foreach (PSObject pso in results) {
                            //*could* have added a Veeam.Backup.Model.DLL reference here to cast the "GuestInfo" value to a CGuestInfo and then checked the "IsUnixBackup"
                            //however this introduces a DLL version dependency which we can avoid altogether by assigning the value of "GuestInfo" to a dynamic var which
                            //doesn't throw any compile-time reference errors. Of course if this private property is changed it will cause problems though
                            dynamic guestInfo = pso.Properties["GuestInfo"].Value;
                            if (!guestInfo.IsUnixBased)
                                if (!vmTreeNodeExists(backup, pso.Properties["VmName"].Value.ToString()))
                                    backup.Nodes.Add(pso.Properties["VmName"].Value.ToString(), pso.Properties["VmName"].Value.ToString());
                        }
                    }

                    cnt++;
                    prgVeeam.Value = (int)((cnt / trvBackups.Nodes[0].Nodes.Count) * 100);
                    Application.DoEvents();

                    vPipe.Dispose();
                }
                prgVeeam.Value = 100;
                Application.DoEvents();
                return true;
            }
            catch (Exception ex)  {
                MessageBox.Show(String.Format("Error enumerating backup job workloads - {0}", ex.Message.ToString()), "Veeam File Diff", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void btnChoosePoints_Click(object sender, EventArgs e)
        {
            TreeNode node = trvBackups.SelectedNode;

            if (node != null)
                if (node.Level == 2) //if a "leaf" then specific workload / VM is selected proceed
                {
                    appData.machineName = node.Text; //save machine/VM name
                    frmChoosePoints nxtDialog = new frmChoosePoints(this, appData);

                    nxtDialog.Visible = true;
                    nxtDialog.DesktopLocation = this.DesktopLocation;
                    nxtDialog.Activate();
                    this.Visible = false;
                }
                else
                    MessageBox.Show("Choose workload/VM!", "Veeam File Diff", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Choose workload/VM!", "Veeam File Diff", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            //not sure why application exit doesn't shut everything down reliably...
            //Application.Exit();
            this.Cursor = Cursors.WaitCursor;
            Application.DoEvents();
            Environment.Exit(0);
        }
    }
}
