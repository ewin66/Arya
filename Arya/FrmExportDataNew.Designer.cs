namespace Arya
{
    partial class FrmExportDataNew
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmExportDataNew));
            this.tlpExport = new System.Windows.Forms.TableLayoutPanel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.tsExportWindow = new System.Windows.Forms.ToolStrip();
            this.tscbExports = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbtnExport = new System.Windows.Forms.ToolStripButton();
            this.llSaveExportOptions = new System.Windows.Forms.ToolStripLabel();
            this.llLoadOptions = new System.Windows.Forms.ToolStripLabel();
            this.pgExport = new System.Windows.Forms.PropertyGrid();
            this.pbExportProgress = new System.Windows.Forms.ProgressBar();
            this.statusTimer = new System.Windows.Forms.Timer(this.components);
            this.tlpExport.SuspendLayout();
            this.tsExportWindow.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpExport
            // 
            this.tlpExport.ColumnCount = 1;
            this.tlpExport.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpExport.Controls.Add(this.lblStatus, 0, 3);
            this.tlpExport.Controls.Add(this.tsExportWindow, 0, 0);
            this.tlpExport.Controls.Add(this.pgExport, 0, 1);
            this.tlpExport.Controls.Add(this.pbExportProgress, 0, 2);
            this.tlpExport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpExport.Location = new System.Drawing.Point(0, 0);
            this.tlpExport.Name = "tlpExport";
            this.tlpExport.RowCount = 4;
            this.tlpExport.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpExport.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpExport.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tlpExport.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tlpExport.Size = new System.Drawing.Size(597, 518);
            this.tlpExport.TabIndex = 0;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoEllipsis = true;
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblStatus.Location = new System.Drawing.Point(3, 468);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(591, 50);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "Ready";
            this.lblStatus.UseMnemonic = false;
            // 
            // tsExportWindow
            // 
            this.tsExportWindow.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tsExportWindow.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsExportWindow.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tscbExports,
            this.toolStripSeparator1,
            this.tsbtnExport,
            this.llSaveExportOptions,
            this.llLoadOptions});
            this.tsExportWindow.Location = new System.Drawing.Point(3, 5);
            this.tsExportWindow.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.tsExportWindow.Name = "tsExportWindow";
            this.tsExportWindow.Size = new System.Drawing.Size(591, 25);
            this.tsExportWindow.TabIndex = 1;
            this.tsExportWindow.Text = "toolStrip1";
            // 
            // tscbExports
            // 
            this.tscbExports.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tscbExports.MaxDropDownItems = 20;
            this.tscbExports.Name = "tscbExports";
            this.tscbExports.Size = new System.Drawing.Size(150, 25);
            this.tscbExports.SelectedIndexChanged += new System.EventHandler(this.tscbExports_SelectedIndexChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbtnExport
            // 
            this.tsbtnExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbtnExport.Image = ((System.Drawing.Image)(resources.GetObject("tsbtnExport.Image")));
            this.tsbtnExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbtnExport.Name = "tsbtnExport";
            this.tsbtnExport.Size = new System.Drawing.Size(44, 22);
            this.tsbtnExport.Text = "Export";
            this.tsbtnExport.Click += new System.EventHandler(this.tsbtnExport_Click);
            // 
            // llSaveExportOptions
            // 
            this.llSaveExportOptions.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.llSaveExportOptions.IsLink = true;
            this.llSaveExportOptions.Name = "llSaveExportOptions";
            this.llSaveExportOptions.Size = new System.Drawing.Size(76, 22);
            this.llSaveExportOptions.Text = "Save Options";
            this.llSaveExportOptions.Click += new System.EventHandler(this.llSaveExportOptions_Click);
            // 
            // llLoadOptions
            // 
            this.llLoadOptions.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.llLoadOptions.IsLink = true;
            this.llLoadOptions.Name = "llLoadOptions";
            this.llLoadOptions.Size = new System.Drawing.Size(78, 22);
            this.llLoadOptions.Text = "Load Options";
            this.llLoadOptions.Click += new System.EventHandler(this.llLoadOptions_Click);
            // 
            // pgExport
            // 
            this.pgExport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgExport.Location = new System.Drawing.Point(3, 33);
            this.pgExport.Name = "pgExport";
            this.pgExport.Size = new System.Drawing.Size(591, 392);
            this.pgExport.TabIndex = 0;
            this.pgExport.ToolbarVisible = false;
            this.pgExport.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.pgExport_PropertyValueChanged);
            // 
            // pbExportProgress
            // 
            this.pbExportProgress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbExportProgress.Location = new System.Drawing.Point(3, 431);
            this.pbExportProgress.Name = "pbExportProgress";
            this.pbExportProgress.Size = new System.Drawing.Size(591, 34);
            this.pbExportProgress.TabIndex = 2;
            // 
            // statusTimer
            // 
            this.statusTimer.Interval = 1000;
            this.statusTimer.Tick += new System.EventHandler(this.statusTimer_Tick);
            // 
            // FrmExportDataNew
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(597, 518);
            this.Controls.Add(this.tlpExport);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MinimizeBox = false;
            this.Name = "FrmExportDataNew";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Export Data";
            this.Load += new System.EventHandler(this.FrmExportDataNew_Load);
            this.tlpExport.ResumeLayout(false);
            this.tlpExport.PerformLayout();
            this.tsExportWindow.ResumeLayout(false);
            this.tsExportWindow.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpExport;
        private System.Windows.Forms.PropertyGrid pgExport;
        private System.Windows.Forms.ToolStrip tsExportWindow;
        private System.Windows.Forms.ToolStripComboBox tscbExports;
        private System.Windows.Forms.ProgressBar pbExportProgress;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsbtnExport;
        private System.Windows.Forms.Timer statusTimer;
        private System.Windows.Forms.ToolStripLabel llSaveExportOptions;
        private System.Windows.Forms.ToolStripLabel llLoadOptions;
    }
}