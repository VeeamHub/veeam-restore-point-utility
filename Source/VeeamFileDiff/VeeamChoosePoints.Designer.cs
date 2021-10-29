namespace VeeamFileDiff
{
    partial class frmChoosePoints
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmChoosePoints));
            this.grpChoosePoints = new System.Windows.Forms.GroupBox();
            this.lsvPoints = new System.Windows.Forms.ListView();
            this.cmbCreds = new System.Windows.Forms.ComboBox();
            this.lblCreds = new System.Windows.Forms.Label();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.grpChoosePoints.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpChoosePoints
            // 
            this.grpChoosePoints.Controls.Add(this.lsvPoints);
            this.grpChoosePoints.Controls.Add(this.cmbCreds);
            this.grpChoosePoints.Controls.Add(this.lblCreds);
            this.grpChoosePoints.Font = new System.Drawing.Font("Marlett", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpChoosePoints.Location = new System.Drawing.Point(12, 12);
            this.grpChoosePoints.Name = "grpChoosePoints";
            this.grpChoosePoints.Size = new System.Drawing.Size(561, 357);
            this.grpChoosePoints.TabIndex = 0;
            this.grpChoosePoints.TabStop = false;
            this.grpChoosePoints.Text = "groupBox1";
            // 
            // lsvPoints
            // 
            this.lsvPoints.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lsvPoints.Location = new System.Drawing.Point(6, 28);
            this.lsvPoints.Name = "lsvPoints";
            this.lsvPoints.Size = new System.Drawing.Size(549, 275);
            this.lsvPoints.TabIndex = 14;
            this.lsvPoints.UseCompatibleStateImageBehavior = false;
            // 
            // cmbCreds
            // 
            this.cmbCreds.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbCreds.FormattingEnabled = true;
            this.cmbCreds.Location = new System.Drawing.Point(6, 328);
            this.cmbCreds.Name = "cmbCreds";
            this.cmbCreds.Size = new System.Drawing.Size(355, 24);
            this.cmbCreds.Sorted = true;
            this.cmbCreds.TabIndex = 13;
            // 
            // lblCreds
            // 
            this.lblCreds.AutoSize = true;
            this.lblCreds.Font = new System.Drawing.Font("Marlett", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCreds.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.lblCreds.Location = new System.Drawing.Point(3, 310);
            this.lblCreds.Name = "lblCreds";
            this.lblCreds.Size = new System.Drawing.Size(139, 18);
            this.lblCreds.TabIndex = 12;
            this.lblCreds.Text = "Mount Credentials";
            // 
            // btnBack
            // 
            this.btnBack.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnBack.Font = new System.Drawing.Font("Marlett", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBack.Location = new System.Drawing.Point(354, 375);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(109, 31);
            this.btnBack.TabIndex = 1;
            this.btnBack.Text = "&Back";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // btnNext
            // 
            this.btnNext.Font = new System.Drawing.Font("Marlett", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNext.Location = new System.Drawing.Point(105, 375);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(98, 32);
            this.btnNext.TabIndex = 2;
            this.btnNext.Text = "&Next";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // frmChoosePoints
            // 
            this.AcceptButton = this.btnNext;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnBack;
            this.ClientSize = new System.Drawing.Size(591, 420);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.grpChoosePoints);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmChoosePoints";
            this.Text = "Veeam Skunkworks";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmChoosePoints_Closing);
            this.Load += new System.EventHandler(this.frmChoosePoints_Load);
            this.Shown += new System.EventHandler(this.frmChoosePoints_Shown);
            this.grpChoosePoints.ResumeLayout(false);
            this.grpChoosePoints.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpChoosePoints;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.ComboBox cmbCreds;
        private System.Windows.Forms.Label lblCreds;
        private System.Windows.Forms.ListView lsvPoints;
    }
}