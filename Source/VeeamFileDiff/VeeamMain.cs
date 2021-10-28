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
        private string[] VBRPSModule = new string[] { "Veeam.Backup.PowerShell" };
        private VeeamAppData appData = new VeeamAppData();

        public frmVeeamMain()
        {
            InitializeComponent();
            slVeeam.Text = "";
        }

        private void frmVeeamMain_Shown(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

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
                    catch {
                        MessageBox.Show("Error opening Powershell runspace", "Veeam File Diff", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            try {
                psPipe = appData.VBRRunSpace.CreatePipeline();
                psPipe.Commands.AddScript(String.Format("Get-Module -Name '{0}'", VBRPSModule[0]));
                ICollection<PSObject> results = psPipe.Invoke();

                if (results.Count > 0)
                    return (true);
                else
                    return (false);
            }
            catch {
                return (false);
            }
        }

        private Boolean initBackupList()
        {
            Pipeline vPipe;
            Boolean rval = false;

            try {
                trvBackups.Nodes.Add("Backups"); //"seed" the treeview

                slVeeam.Text = "Retrieving available backups";

                vPipe = appData.VBRRunSpace.CreatePipeline();
                //vPipe.Commands.AddScript("Get-VBRBackup | ? {$_.JobType -eq 'Backup'} | select jobname, jobtype, Id");
                vPipe.Commands.AddScript("Get-VBRBackup | ? {$_.JobType -eq 'Backup' -or $_.JobType -eq 'EndpointBackup' -or $_.JobType -eq 'EpAgentManagement'} | Select-Object JobName, JobType, Id");
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
                    vPipe.Commands.AddScript(String.Format("Get-VBRBackup -Name '{0}' | Get-VBRRestorePoint | Select-Object -Unique VmName", backup.Text));
                    ICollection<PSObject> results = vPipe.Invoke();

                    if (vPipe.Error.Count == 0) {
                        foreach (PSObject pso in results) {
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
                if (node.Level == 2) //if a "leaf" then restore point is selected proceed
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
