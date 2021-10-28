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
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace VeeamFileDiff
{
    public partial class frmChoosePoints : Form
    {
        private VeeamAppData vad;
        private Form priorForm;
        private ArrayList restorePoints = new ArrayList();

        public frmChoosePoints(Form callingForm, VeeamAppData appData)
        {
            priorForm = callingForm;
            vad = appData;
           
            InitializeComponent();
        }

        private void frmChoosePoints_Load(object sender, EventArgs e)
        {
            lsvPoints.View = View.Details;

            lsvPoints.BorderStyle = BorderStyle.Fixed3D;
            lsvPoints.FullRowSelect = true;
            lsvPoints.AllowColumnReorder = false;
            lsvPoints.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lsvPoints.MultiSelect = true;
            lsvPoints.Visible = true;
            lsvPoints.Scrollable = true;
            lsvPoints.GridLines = false;

            lsvPoints.Columns.Add("Workload", (int)(lsvPoints.Width * 0.25), HorizontalAlignment.Left);
            lsvPoints.Columns.Add("Restore Point", (int)(lsvPoints.Width * 0.4), HorizontalAlignment.Left);
            lsvPoints.Columns.Add("Repo Location", (int)(lsvPoints.Width * 0.35), HorizontalAlignment.Left);
        }

        private void frmChoosePoints_Shown(object sender, EventArgs e)
        {
            this.CenterToScreen();

            grpChoosePoints.Text = String.Format("Restore Points for - {0}", vad.machineName);

            if (lsvPoints.Items.Count == 0) initVMsRestoPts(); //grab workload restore points if not already populated

            if (cmbCreds.Items.Count == 0) initCredsList(); //grab credentials if not already populated

        }

        private void frmChoosePoints_Closing(object sender, EventArgs e)
        {
            priorForm.Activate();
            priorForm.Visible = true;
            Dispose();
        }
        private Boolean initVMsRestoPts()
        {
            ListViewItem lstItem;

            try
            {
                Pipeline vPipe;

                Cursor = Cursors.WaitCursor;

                vPipe = vad.VBRRunSpace.CreatePipeline();

                //PS query will return all the restore points with VM name for the backup
                //vPipe.Commands.AddScript(String.Format("Get-VBRRestorePoint -Name '{0}' | Select-Object VmName, CreationTime, Type, Id | Sort-Object -Property CreationTime -Descending", vad.machineName));
                vPipe.Commands.AddScript(String.Format("Get-VBRRestorePoint -Name '{0}' | Select-Object VmName, CreationTime, Type, Id, {{$_.FindChainRepositories().Name}} | Sort-Object -Property CreationTime -Descending", vad.machineName));
                ICollection<PSObject> results = vPipe.Invoke();

                if (vPipe.Error.Count == 0)
                {
                    foreach (PSObject pso in results)
                    {
                        if (lsvPoints.Items.Count == 0) //add "live VM/workload" option
                        {
                            lstItem = new ListViewItem(pso.Properties["VmName"].Value.ToString());
                            lstItem.SubItems.Add("Live VM / workload");
                            lstItem.Tag = "0";
                            lsvPoints.Items.Add(lstItem);
                        }
                        lstItem = new ListViewItem(pso.Properties["VmName"].Value.ToString());
                        lstItem.SubItems.Add(pso.Properties["CreationTime"].Value.ToString());
                        lstItem.SubItems.Add(pso.Properties["$_.FindChainRepositories().Name"].Value.ToString());
                        lstItem.Tag = pso.Properties["Id"].Value.ToString();
                        lsvPoints.Items.Add(lstItem);
                    }
                }

                vPipe.Dispose();

                Cursor = Cursors.Default;
                Application.DoEvents();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Error enumerating restore points - {0}", ex.Message.ToString()), "Veeam File Diff", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private Boolean initCredsList()
        {
            Pipeline vPipe;
            Boolean rval = false;

            try
            {
                Cursor = Cursors.WaitCursor;
                Application.DoEvents();

                vPipe = vad.VBRRunSpace.CreatePipeline();
                vPipe.Commands.AddScript("Get-VBRCredentials");
                ICollection<PSObject> results = vPipe.Invoke();

                if (vPipe.Error.Count == 0)
                {
                    int cnt = 0;
                    foreach (PSObject pso in results)
                    {
                        //add to combobox
                        cmbCreds.Items.Add(String.Format("{0} ({1})", pso.Properties["Name"].Value.ToString(), pso.Properties["Description"].Value.ToString()));
                        //cmbCreds.
                        cnt++;
                        //prgVeeam.Value = (int)((cnt / results.Count) * 100);
                        Application.DoEvents();
                    }
                    rval = true;
                }
                vPipe.Dispose();

                Cursor = Cursors.Default;

                Application.DoEvents();

                return rval;
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Error enumerating available credentials - {0}", ex.Message.ToString()), "Veeam File Diff", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (lsvPoints.SelectedItems.Count == 2)
            {
                if (cmbCreds.SelectedItem != null)
                {
                    //extract credentials from combobox and init mount credentials property
                    string[] userName = cmbCreds.SelectedItem.ToString().Split(' ');  //extract credential username
                    string[] userDesc = cmbCreds.SelectedItem.ToString().Split('(');  //extract credential description
                    vad.mntCreds = new VeeamAppData.creds(userName[0], userDesc[1].Substring(0, userDesc[1].Length - 1));

                    int itmCnt = 0;
                    foreach (int itm in lsvPoints.SelectedIndices)
                    {
                        if (itmCnt == 0)
                            vad.restoPtIDA = lsvPoints.Items[itm].Tag.ToString();
                        else
                            vad.restoPtIDB = lsvPoints.Items[itm].Tag.ToString();
                        itmCnt++;
                    }
                    frmViewDiff nxtDialog = new frmViewDiff(this, vad);

                    nxtDialog.Visible = true;
                    nxtDialog.DesktopLocation = this.DesktopLocation;
                    nxtDialog.Activate();
                    this.Visible = false;
                }
                else
                    MessageBox.Show("Select mount server credentials!", "Veeam File Diff", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show("Select 2 points for comparison!", "Veeam File Diff", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
