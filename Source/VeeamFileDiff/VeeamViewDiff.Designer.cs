namespace VeeamFileDiff
{
    partial class frmViewDiff
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
            System.Windows.Forms.Button btnBrowseFolderA;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmViewDiff));
            this.btnBack = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnBrowseFolderB = new System.Windows.Forms.Button();
            this.lblFolderB = new System.Windows.Forms.Label();
            this.lblFolderA = new System.Windows.Forms.Label();
            this.lblRestoPtB = new System.Windows.Forms.Label();
            this.lblRestoPtA = new System.Windows.Forms.Label();
            this.fbMountedBackup = new System.Windows.Forms.FolderBrowserDialog();
            this.btnCompare = new System.Windows.Forms.Button();
            this.trvABDiff = new System.Windows.Forms.TreeView();
            this.trvFileDiff = new System.Windows.Forms.TreeView();
            this.trvBADiff = new System.Windows.Forms.TreeView();
            this.lblABDiff = new System.Windows.Forms.Label();
            this.lblCommonDiffs = new System.Windows.Forms.Label();
            this.lblBADiff = new System.Windows.Forms.Label();
            this.lblPointASize = new System.Windows.Forms.Label();
            this.lblChangedSize = new System.Windows.Forms.Label();
            this.lblPointBSize = new System.Windows.Forms.Label();
            this.stsViewDiff = new System.Windows.Forms.StatusStrip();
            this.tssOperationInFlight = new System.Windows.Forms.ToolStripStatusLabel();
            this.prgViewDiff = new System.Windows.Forms.ToolStripProgressBar();
            btnBrowseFolderA = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.stsViewDiff.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnBrowseFolderA
            // 
            btnBrowseFolderA.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btnBrowseFolderA.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            btnBrowseFolderA.Location = new System.Drawing.Point(773, 30);
            btnBrowseFolderA.Name = "btnBrowseFolderA";
            btnBrowseFolderA.Size = new System.Drawing.Size(50, 29);
            btnBrowseFolderA.TabIndex = 4;
            btnBrowseFolderA.Text = ". . .";
            btnBrowseFolderA.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            btnBrowseFolderA.UseVisualStyleBackColor = true;
            btnBrowseFolderA.Click += new System.EventHandler(this.btnBrowseFolderA_Click);
            // 
            // btnBack
            // 
            this.btnBack.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnBack.Font = new System.Drawing.Font("Marlett", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBack.Location = new System.Drawing.Point(1047, 74);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(126, 37);
            this.btnBack.TabIndex = 0;
            this.btnBack.Text = "&Back";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btnBrowseFolderB);
            this.groupBox1.Controls.Add(btnBrowseFolderA);
            this.groupBox1.Controls.Add(this.lblFolderB);
            this.groupBox1.Controls.Add(this.lblFolderA);
            this.groupBox1.Controls.Add(this.lblRestoPtB);
            this.groupBox1.Controls.Add(this.lblRestoPtA);
            this.groupBox1.Font = new System.Drawing.Font("Marlett", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(22, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(833, 102);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Select Folders for Comparison";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Marlett", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 14);
            this.label1.TabIndex = 7;
            this.label1.Text = "Restore Point B -";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Marlett", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(135, 15);
            this.label2.TabIndex = 6;
            this.label2.Text = "Restore Point A -";
            // 
            // btnBrowseFolderB
            // 
            this.btnBrowseFolderB.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBrowseFolderB.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnBrowseFolderB.Location = new System.Drawing.Point(773, 60);
            this.btnBrowseFolderB.Name = "btnBrowseFolderB";
            this.btnBrowseFolderB.Size = new System.Drawing.Size(50, 29);
            this.btnBrowseFolderB.TabIndex = 5;
            this.btnBrowseFolderB.Text = ". . .";
            this.btnBrowseFolderB.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnBrowseFolderB.UseVisualStyleBackColor = true;
            this.btnBrowseFolderB.Click += new System.EventHandler(this.btnBrowseFolderB_Click);
            // 
            // lblFolderB
            // 
            this.lblFolderB.BackColor = System.Drawing.SystemColors.Window;
            this.lblFolderB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblFolderB.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFolderB.Location = new System.Drawing.Point(278, 60);
            this.lblFolderB.Name = "lblFolderB";
            this.lblFolderB.Size = new System.Drawing.Size(491, 24);
            this.lblFolderB.TabIndex = 3;
            // 
            // lblFolderA
            // 
            this.lblFolderA.BackColor = System.Drawing.SystemColors.Window;
            this.lblFolderA.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblFolderA.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFolderA.Location = new System.Drawing.Point(278, 30);
            this.lblFolderA.Name = "lblFolderA";
            this.lblFolderA.Size = new System.Drawing.Size(491, 24);
            this.lblFolderA.TabIndex = 2;
            // 
            // lblRestoPtB
            // 
            this.lblRestoPtB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRestoPtB.Location = new System.Drawing.Point(140, 60);
            this.lblRestoPtB.Name = "lblRestoPtB";
            this.lblRestoPtB.Size = new System.Drawing.Size(130, 14);
            this.lblRestoPtB.TabIndex = 1;
            this.lblRestoPtB.Text = "Restore Point B";
            // 
            // lblRestoPtA
            // 
            this.lblRestoPtA.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRestoPtA.Location = new System.Drawing.Point(140, 30);
            this.lblRestoPtA.Name = "lblRestoPtA";
            this.lblRestoPtA.Size = new System.Drawing.Size(130, 14);
            this.lblRestoPtA.TabIndex = 0;
            this.lblRestoPtA.Text = "Restore Point A";
            // 
            // btnCompare
            // 
            this.btnCompare.Font = new System.Drawing.Font("Marlett", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCompare.Location = new System.Drawing.Point(861, 74);
            this.btnCompare.Name = "btnCompare";
            this.btnCompare.Size = new System.Drawing.Size(126, 37);
            this.btnCompare.TabIndex = 3;
            this.btnCompare.Text = "&Compare";
            this.btnCompare.UseVisualStyleBackColor = true;
            this.btnCompare.Click += new System.EventHandler(this.btnCompare_Click);
            // 
            // trvABDiff
            // 
            this.trvABDiff.Location = new System.Drawing.Point(22, 138);
            this.trvABDiff.Name = "trvABDiff";
            this.trvABDiff.Size = new System.Drawing.Size(422, 424);
            this.trvABDiff.TabIndex = 5;
            // 
            // trvFileDiff
            // 
            this.trvFileDiff.Location = new System.Drawing.Point(469, 138);
            this.trvFileDiff.Name = "trvFileDiff";
            this.trvFileDiff.Size = new System.Drawing.Size(422, 424);
            this.trvFileDiff.TabIndex = 6;
            // 
            // trvBADiff
            // 
            this.trvBADiff.Location = new System.Drawing.Point(911, 138);
            this.trvBADiff.Name = "trvBADiff";
            this.trvBADiff.Size = new System.Drawing.Size(422, 424);
            this.trvBADiff.TabIndex = 7;
            // 
            // lblABDiff
            // 
            this.lblABDiff.Font = new System.Drawing.Font("Marlett", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblABDiff.Location = new System.Drawing.Point(22, 121);
            this.lblABDiff.Name = "lblABDiff";
            this.lblABDiff.Size = new System.Drawing.Size(200, 14);
            this.lblABDiff.TabIndex = 8;
            this.lblABDiff.Text = "Files Unique to A";
            // 
            // lblCommonDiffs
            // 
            this.lblCommonDiffs.Font = new System.Drawing.Font("Marlett", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCommonDiffs.Location = new System.Drawing.Point(469, 121);
            this.lblCommonDiffs.Name = "lblCommonDiffs";
            this.lblCommonDiffs.Size = new System.Drawing.Size(200, 14);
            this.lblCommonDiffs.TabIndex = 9;
            this.lblCommonDiffs.Text = "Shared Files Delta Change";
            // 
            // lblBADiff
            // 
            this.lblBADiff.Font = new System.Drawing.Font("Marlett", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBADiff.Location = new System.Drawing.Point(911, 121);
            this.lblBADiff.Name = "lblBADiff";
            this.lblBADiff.Size = new System.Drawing.Size(241, 14);
            this.lblBADiff.TabIndex = 10;
            this.lblBADiff.Text = "Files Unique to B";
            // 
            // lblPointASize
            // 
            this.lblPointASize.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPointASize.Location = new System.Drawing.Point(126, 121);
            this.lblPointASize.Name = "lblPointASize";
            this.lblPointASize.Size = new System.Drawing.Size(130, 14);
            this.lblPointASize.TabIndex = 8;
            // 
            // lblChangedSize
            // 
            this.lblChangedSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblChangedSize.Location = new System.Drawing.Point(625, 121);
            this.lblChangedSize.Name = "lblChangedSize";
            this.lblChangedSize.Size = new System.Drawing.Size(130, 14);
            this.lblChangedSize.TabIndex = 12;
            // 
            // lblPointBSize
            // 
            this.lblPointBSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPointBSize.Location = new System.Drawing.Point(1016, 121);
            this.lblPointBSize.Name = "lblPointBSize";
            this.lblPointBSize.Size = new System.Drawing.Size(130, 14);
            this.lblPointBSize.TabIndex = 13;
            // 
            // stsViewDiff
            // 
            this.stsViewDiff.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tssOperationInFlight,
            this.prgViewDiff});
            this.stsViewDiff.Location = new System.Drawing.Point(0, 578);
            this.stsViewDiff.Name = "stsViewDiff";
            this.stsViewDiff.Size = new System.Drawing.Size(1352, 22);
            this.stsViewDiff.TabIndex = 14;
            this.stsViewDiff.Text = "statusStrip1";
            // 
            // tssOperationInFlight
            // 
            this.tssOperationInFlight.Name = "tssOperationInFlight";
            this.tssOperationInFlight.Size = new System.Drawing.Size(114, 17);
            this.tssOperationInFlight.Text = "tssOperationInFlight";
            // 
            // prgViewDiff
            // 
            this.prgViewDiff.Name = "prgViewDiff";
            this.prgViewDiff.Size = new System.Drawing.Size(100, 16);
            // 
            // frmViewDiff
            // 
            this.AcceptButton = this.btnCompare;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnBack;
            this.ClientSize = new System.Drawing.Size(1352, 600);
            this.Controls.Add(this.stsViewDiff);
            this.Controls.Add(this.lblPointBSize);
            this.Controls.Add(this.lblChangedSize);
            this.Controls.Add(this.lblPointASize);
            this.Controls.Add(this.lblBADiff);
            this.Controls.Add(this.lblCommonDiffs);
            this.Controls.Add(this.lblABDiff);
            this.Controls.Add(this.trvBADiff);
            this.Controls.Add(this.trvFileDiff);
            this.Controls.Add(this.trvABDiff);
            this.Controls.Add(this.btnCompare);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnBack);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmViewDiff";
            this.Text = "Veeam Skunkworks";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmViewDiff_Closing);
            this.Load += new System.EventHandler(this.frmViewDiff_Load);
            this.Shown += new System.EventHandler(this.frmViewDiff_Shown);
            this.groupBox1.ResumeLayout(false);
            this.stsViewDiff.ResumeLayout(false);
            this.stsViewDiff.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnBrowseFolderB;
        private System.Windows.Forms.Label lblFolderB;
        private System.Windows.Forms.Label lblFolderA;
        private System.Windows.Forms.Label lblRestoPtB;
        private System.Windows.Forms.Label lblRestoPtA;
        private System.Windows.Forms.FolderBrowserDialog fbMountedBackup;
        private System.Windows.Forms.Button btnCompare;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TreeView trvABDiff;
        private System.Windows.Forms.TreeView trvFileDiff;
        private System.Windows.Forms.TreeView trvBADiff;
        private System.Windows.Forms.Label lblABDiff;
        private System.Windows.Forms.Label lblCommonDiffs;
        private System.Windows.Forms.Label lblBADiff;
        private System.Windows.Forms.Label lblPointASize;
        private System.Windows.Forms.Label lblChangedSize;
        private System.Windows.Forms.Label lblPointBSize;
        private System.Windows.Forms.StatusStrip stsViewDiff;
        private System.Windows.Forms.ToolStripStatusLabel tssOperationInFlight;
        private System.Windows.Forms.ToolStripProgressBar prgViewDiff;
    }
}