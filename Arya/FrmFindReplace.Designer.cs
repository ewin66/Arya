namespace Arya
{
    internal partial class FrmFindReplace
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
            this.lblFind = new System.Windows.Forms.Label();
            this.txtFind = new System.Windows.Forms.TextBox();
            this.lblReplace = new System.Windows.Forms.Label();
            this.txtReplace = new System.Windows.Forms.TextBox();
            this.tblFindReplace = new System.Windows.Forms.TableLayoutPanel();
            this.lblOptions = new System.Windows.Forms.Label();
            this.ddFindType = new System.Windows.Forms.ComboBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnFindNext = new System.Windows.Forms.Button();
            this.btnReplace = new System.Windows.Forms.Button();
            this.btnReplaceAll = new System.Windows.Forms.Button();
            this.ddReplaceType = new System.Windows.Forms.ComboBox();
            this.tblFindOptions = new System.Windows.Forms.TableLayoutPanel();
            this.tblCase = new System.Windows.Forms.TableLayoutPanel();
            this.rbMatchCase = new System.Windows.Forms.RadioButton();
            this.rbIgnoreCase = new System.Windows.Forms.RadioButton();
            this.tblSelection = new System.Windows.Forms.TableLayoutPanel();
            this.rbSearchSelection = new System.Windows.Forms.RadioButton();
            this.rbSearchAllCells = new System.Windows.Forms.RadioButton();
            this.tblFindReplace.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tblFindOptions.SuspendLayout();
            this.tblCase.SuspendLayout();
            this.tblSelection.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblFind
            // 
            this.lblFind.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblFind.AutoSize = true;
            this.lblFind.Location = new System.Drawing.Point(3, 8);
            this.lblFind.Name = "lblFind";
            this.lblFind.Size = new System.Drawing.Size(27, 13);
            this.lblFind.TabIndex = 1;
            this.lblFind.Text = "Find";
            // 
            // txtFind
            // 
            this.txtFind.Location = new System.Drawing.Point(152, 5);
            this.txtFind.Margin = new System.Windows.Forms.Padding(5);
            this.txtFind.Name = "txtFind";
            this.txtFind.Size = new System.Drawing.Size(316, 20);
            this.txtFind.TabIndex = 2;
            // 
            // lblReplace
            // 
            this.lblReplace.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblReplace.AutoSize = true;
            this.lblReplace.Location = new System.Drawing.Point(3, 38);
            this.lblReplace.Name = "lblReplace";
            this.lblReplace.Size = new System.Drawing.Size(47, 13);
            this.lblReplace.TabIndex = 3;
            this.lblReplace.Text = "Replace";
            // 
            // txtReplace
            // 
            this.txtReplace.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.txtReplace.Location = new System.Drawing.Point(152, 35);
            this.txtReplace.Margin = new System.Windows.Forms.Padding(5);
            this.txtReplace.Name = "txtReplace";
            this.txtReplace.Size = new System.Drawing.Size(316, 20);
            this.txtReplace.TabIndex = 4;
            // 
            // tblFindReplace
            // 
            this.tblFindReplace.ColumnCount = 3;
            this.tblFindReplace.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblFindReplace.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblFindReplace.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblFindReplace.Controls.Add(this.lblOptions, 0, 2);
            this.tblFindReplace.Controls.Add(this.ddFindType, 1, 0);
            this.tblFindReplace.Controls.Add(this.lblFind, 0, 0);
            this.tblFindReplace.Controls.Add(this.lblStatus, 0, 3);
            this.tblFindReplace.Controls.Add(this.tableLayoutPanel2, 0, 4);
            this.tblFindReplace.Controls.Add(this.txtFind, 2, 0);
            this.tblFindReplace.Controls.Add(this.lblReplace, 0, 1);
            this.tblFindReplace.Controls.Add(this.ddReplaceType, 1, 1);
            this.tblFindReplace.Controls.Add(this.txtReplace, 2, 1);
            this.tblFindReplace.Controls.Add(this.tblFindOptions, 1, 2);
            this.tblFindReplace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblFindReplace.Location = new System.Drawing.Point(0, 0);
            this.tblFindReplace.Name = "tblFindReplace";
            this.tblFindReplace.RowCount = 5;
            this.tblFindReplace.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblFindReplace.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblFindReplace.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblFindReplace.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblFindReplace.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblFindReplace.Size = new System.Drawing.Size(474, 176);
            this.tblFindReplace.TabIndex = 0;
            // 
            // lblOptions
            // 
            this.lblOptions.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblOptions.AutoSize = true;
            this.lblOptions.Location = new System.Drawing.Point(3, 82);
            this.lblOptions.Name = "lblOptions";
            this.lblOptions.Size = new System.Drawing.Size(43, 13);
            this.lblOptions.TabIndex = 10;
            this.lblOptions.Text = "Options";
            // 
            // ddFindType
            // 
            this.ddFindType.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ddFindType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddFindType.FormattingEnabled = true;
            this.ddFindType.Location = new System.Drawing.Point(56, 4);
            this.ddFindType.Name = "ddFindType";
            this.ddFindType.Size = new System.Drawing.Size(88, 21);
            this.ddFindType.TabIndex = 0;
            this.ddFindType.SelectedIndexChanged += new System.EventHandler(this.ddFindType_SelectedIndexChanged);
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblStatus.AutoSize = true;
            this.tblFindReplace.SetColumnSpan(this.lblStatus, 3);
            this.lblStatus.Location = new System.Drawing.Point(3, 128);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(43, 13);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "Ready";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tblFindReplace.SetColumnSpan(this.tableLayoutPanel2, 3);
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.btnFindNext, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnReplace, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnReplaceAll, 2, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(228, 144);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(243, 29);
            this.tableLayoutPanel2.TabIndex = 6;
            // 
            // btnFindNext
            // 
            this.btnFindNext.Location = new System.Drawing.Point(3, 3);
            this.btnFindNext.Name = "btnFindNext";
            this.btnFindNext.Size = new System.Drawing.Size(75, 23);
            this.btnFindNext.TabIndex = 0;
            this.btnFindNext.Text = "Find Next";
            this.btnFindNext.UseVisualStyleBackColor = true;
            this.btnFindNext.Click += new System.EventHandler(this.btnFindNext_Click);
            // 
            // btnReplace
            // 
            this.btnReplace.Location = new System.Drawing.Point(84, 3);
            this.btnReplace.Name = "btnReplace";
            this.btnReplace.Size = new System.Drawing.Size(75, 23);
            this.btnReplace.TabIndex = 1;
            this.btnReplace.Text = "Replace";
            this.btnReplace.UseVisualStyleBackColor = true;
            this.btnReplace.Click += new System.EventHandler(this.btnReplace_Click);
            // 
            // btnReplaceAll
            // 
            this.btnReplaceAll.Location = new System.Drawing.Point(165, 3);
            this.btnReplaceAll.Name = "btnReplaceAll";
            this.btnReplaceAll.Size = new System.Drawing.Size(75, 23);
            this.btnReplaceAll.TabIndex = 2;
            this.btnReplaceAll.Text = "Replace All";
            this.btnReplaceAll.UseVisualStyleBackColor = true;
            this.btnReplaceAll.Click += new System.EventHandler(this.btnReplaceAll_Click);
            // 
            // ddReplaceType
            // 
            this.ddReplaceType.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ddReplaceType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddReplaceType.FormattingEnabled = true;
            this.ddReplaceType.Location = new System.Drawing.Point(56, 34);
            this.ddReplaceType.Name = "ddReplaceType";
            this.ddReplaceType.Size = new System.Drawing.Size(88, 21);
            this.ddReplaceType.TabIndex = 9;
            // 
            // tblFindOptions
            // 
            this.tblFindOptions.AutoSize = true;
            this.tblFindOptions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblFindOptions.ColumnCount = 2;
            this.tblFindReplace.SetColumnSpan(this.tblFindOptions, 2);
            this.tblFindOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblFindOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblFindOptions.Controls.Add(this.tblCase, 0, 0);
            this.tblFindOptions.Controls.Add(this.tblSelection, 1, 0);
            this.tblFindOptions.Location = new System.Drawing.Point(56, 63);
            this.tblFindOptions.Name = "tblFindOptions";
            this.tblFindOptions.RowCount = 1;
            this.tblFindOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblFindOptions.Size = new System.Drawing.Size(236, 52);
            this.tblFindOptions.TabIndex = 7;
            // 
            // tblCase
            // 
            this.tblCase.AutoSize = true;
            this.tblCase.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblCase.ColumnCount = 1;
            this.tblCase.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblCase.Controls.Add(this.rbMatchCase, 0, 0);
            this.tblCase.Controls.Add(this.rbIgnoreCase, 0, 1);
            this.tblCase.Location = new System.Drawing.Point(3, 3);
            this.tblCase.Name = "tblCase";
            this.tblCase.RowCount = 2;
            this.tblCase.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblCase.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblCase.Size = new System.Drawing.Size(88, 46);
            this.tblCase.TabIndex = 4;
            // 
            // rbMatchCase
            // 
            this.rbMatchCase.AutoSize = true;
            this.rbMatchCase.Checked = true;
            this.rbMatchCase.Location = new System.Drawing.Point(3, 3);
            this.rbMatchCase.Name = "rbMatchCase";
            this.rbMatchCase.Size = new System.Drawing.Size(82, 17);
            this.rbMatchCase.TabIndex = 0;
            this.rbMatchCase.TabStop = true;
            this.rbMatchCase.Text = "Match Case";
            this.rbMatchCase.UseVisualStyleBackColor = true;
            // 
            // rbIgnoreCase
            // 
            this.rbIgnoreCase.AutoSize = true;
            this.rbIgnoreCase.Location = new System.Drawing.Point(3, 26);
            this.rbIgnoreCase.Name = "rbIgnoreCase";
            this.rbIgnoreCase.Size = new System.Drawing.Size(82, 17);
            this.rbIgnoreCase.TabIndex = 1;
            this.rbIgnoreCase.Text = "Ignore Case";
            this.rbIgnoreCase.UseVisualStyleBackColor = true;
            // 
            // tblSelection
            // 
            this.tblSelection.AutoSize = true;
            this.tblSelection.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblSelection.ColumnCount = 1;
            this.tblSelection.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblSelection.Controls.Add(this.rbSearchSelection, 0, 0);
            this.tblSelection.Controls.Add(this.rbSearchAllCells, 0, 1);
            this.tblSelection.Location = new System.Drawing.Point(97, 3);
            this.tblSelection.Name = "tblSelection";
            this.tblSelection.RowCount = 2;
            this.tblSelection.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblSelection.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblSelection.Size = new System.Drawing.Size(136, 46);
            this.tblSelection.TabIndex = 5;
            // 
            // rbSearchSelection
            // 
            this.rbSearchSelection.AutoSize = true;
            this.rbSearchSelection.Checked = true;
            this.rbSearchSelection.Location = new System.Drawing.Point(3, 3);
            this.rbSearchSelection.Name = "rbSearchSelection";
            this.rbSearchSelection.Size = new System.Drawing.Size(130, 17);
            this.rbSearchSelection.TabIndex = 0;
            this.rbSearchSelection.TabStop = true;
            this.rbSearchSelection.Text = "Current Selection Only";
            this.rbSearchSelection.UseVisualStyleBackColor = true;
            // 
            // rbSearchAllCells
            // 
            this.rbSearchAllCells.AutoSize = true;
            this.rbSearchAllCells.Location = new System.Drawing.Point(3, 26);
            this.rbSearchAllCells.Name = "rbSearchAllCells";
            this.rbSearchAllCells.Size = new System.Drawing.Size(98, 17);
            this.rbSearchAllCells.TabIndex = 1;
            this.rbSearchAllCells.Text = "Search All Cells";
            this.rbSearchAllCells.UseVisualStyleBackColor = true;
            // 
            // FrmFindReplace
            // 
            this.AcceptButton = this.btnFindNext;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(474, 176);
            this.Controls.Add(this.tblFindReplace);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.KeyPreview = true;
            this.Location = new System.Drawing.Point(50, 50);
            this.Name = "FrmFindReplace";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Find / Replace";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FrmFindReplace_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmFindReplace_KeyDown);
            this.tblFindReplace.ResumeLayout(false);
            this.tblFindReplace.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tblFindOptions.ResumeLayout(false);
            this.tblFindOptions.PerformLayout();
            this.tblCase.ResumeLayout(false);
            this.tblCase.PerformLayout();
            this.tblSelection.ResumeLayout(false);
            this.tblSelection.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblFind;
        private System.Windows.Forms.TextBox txtFind;
        private System.Windows.Forms.Label lblReplace;
        private System.Windows.Forms.TextBox txtReplace;
        private System.Windows.Forms.TableLayoutPanel tblFindReplace;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button btnFindNext;
        private System.Windows.Forms.Button btnReplace;
        private System.Windows.Forms.Button btnReplaceAll;
        private System.Windows.Forms.TableLayoutPanel tblFindOptions;
        private System.Windows.Forms.TableLayoutPanel tblCase;
        private System.Windows.Forms.RadioButton rbMatchCase;
        private System.Windows.Forms.RadioButton rbIgnoreCase;
        private System.Windows.Forms.TableLayoutPanel tblSelection;
        private System.Windows.Forms.RadioButton rbSearchSelection;
        private System.Windows.Forms.RadioButton rbSearchAllCells;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ComboBox ddFindType;
        private System.Windows.Forms.ComboBox ddReplaceType;
        private System.Windows.Forms.Label lblOptions;
    }
}