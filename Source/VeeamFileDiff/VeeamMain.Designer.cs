namespace VeeamFileDiff
{
    partial class frmVeeamMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmVeeamMain));
            this.fldBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnChoosePoints = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.trvBackups = new System.Windows.Forms.TreeView();
            this.sstVeeam = new System.Windows.Forms.StatusStrip();
            this.slVeeam = new System.Windows.Forms.ToolStripStatusLabel();
            this.prgVeeam = new System.Windows.Forms.ToolStripProgressBar();
            this.groupBox1.SuspendLayout();
            this.sstVeeam.SuspendLayout();
            this.SuspendLayout();
            // 
            // fldBrowser
            // 
            this.fldBrowser.RootFolder = System.Environment.SpecialFolder.MyComputer;
            this.fldBrowser.ShowNewFolderButton = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnExit);
            this.groupBox1.Controls.Add(this.btnChoosePoints);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.trvBackups);
            this.groupBox1.Font = new System.Drawing.Font("Marlett", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(16, 10);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox1.Size = new System.Drawing.Size(520, 298);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Windows Restore Point Diff";
            // 
            // btnExit
            // 
            this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnExit.Location = new System.Drawing.Point(299, 257);
            this.btnExit.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(136, 30);
            this.btnExit.TabIndex = 16;
            this.btnExit.Text = "E&xit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnChoosePoints
            // 
            this.btnChoosePoints.Location = new System.Drawing.Point(73, 257);
            this.btnChoosePoints.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnChoosePoints.Name = "btnChoosePoints";
            this.btnChoosePoints.Size = new System.Drawing.Size(136, 30);
            this.btnChoosePoints.TabIndex = 15;
            this.btnChoosePoints.Text = "&Select";
            this.btnChoosePoints.UseVisualStyleBackColor = true;
            this.btnChoosePoints.Click += new System.EventHandler(this.btnChoosePoints_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.label1.Location = new System.Drawing.Point(20, 27);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(123, 18);
            this.label1.TabIndex = 14;
            this.label1.Text = "Select Workload";
            // 
            // trvBackups
            // 
            this.trvBackups.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trvBackups.HideSelection = false;
            this.trvBackups.Location = new System.Drawing.Point(23, 48);
            this.trvBackups.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.trvBackups.Name = "trvBackups";
            this.trvBackups.Size = new System.Drawing.Size(472, 202);
            this.trvBackups.TabIndex = 13;
            // 
            // sstVeeam
            // 
            this.sstVeeam.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.slVeeam,
            this.prgVeeam});
            this.sstVeeam.Location = new System.Drawing.Point(0, 322);
            this.sstVeeam.Name = "sstVeeam";
            this.sstVeeam.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.sstVeeam.Size = new System.Drawing.Size(558, 22);
            this.sstVeeam.TabIndex = 3;
            this.sstVeeam.Text = "statusStrip1";
            // 
            // slVeeam
            // 
            this.slVeeam.Name = "slVeeam";
            this.slVeeam.Size = new System.Drawing.Size(50, 17);
            this.slVeeam.Text = "slVeeam";
            // 
            // prgVeeam
            // 
            this.prgVeeam.Name = "prgVeeam";
            this.prgVeeam.Size = new System.Drawing.Size(133, 16);
            // 
            // frmVeeamMain
            // 
            this.AcceptButton = this.btnChoosePoints;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 11F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnExit;
            this.ClientSize = new System.Drawing.Size(558, 344);
            this.Controls.Add(this.sstVeeam);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Marlett", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "frmVeeamMain";
            this.Text = "Veeam Skunkworks";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmVeeamMain_FormClosing);
            this.Load += new System.EventHandler(this.frmVeeamMain_Load);
            this.Shown += new System.EventHandler(this.frmVeeamMain_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.sstVeeam.ResumeLayout(false);
            this.sstVeeam.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.FolderBrowserDialog fldBrowser;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.StatusStrip sstVeeam;
        private System.Windows.Forms.ToolStripStatusLabel slVeeam;
        private System.Windows.Forms.ToolStripProgressBar prgVeeam;
        private System.Windows.Forms.Button btnChoosePoints;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TreeView trvBackups;
        private System.Windows.Forms.Button btnExit;
    }
}

