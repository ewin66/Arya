using System.Windows.Forms;

namespace Arya.HelperForms
{
    partial class FrmColorRules
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
            this.cdgv = new System.Windows.Forms.DataGridView();
            this.colRuleAttribute = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOperator = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBackColor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTextColor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colColorSample = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlButtons = new System.Windows.Forms.TableLayoutPanel();
            this.btnApply = new System.Windows.Forms.Button();
            this.ddRuleAttributes = new System.Windows.Forms.ComboBox();
            this.lnkMoveRuleUp = new System.Windows.Forms.LinkLabel();
            this.lnkMoveRuleDown = new System.Windows.Forms.LinkLabel();
            this.btnDeleteRules = new System.Windows.Forms.LinkLabel();
            this.btnAddRule = new System.Windows.Forms.LinkLabel();
            this.lnkSave = new System.Windows.Forms.LinkLabel();
            this.lnkLoad = new System.Windows.Forms.LinkLabel();
            this.cellColorDialog = new System.Windows.Forms.ColorDialog();
            this.openFileDialogXml = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialogXml = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.cdgv)).BeginInit();
            this.pnlButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // cdgv
            // 
            this.cdgv.AllowUserToAddRows = false;
            this.cdgv.AllowUserToDeleteRows = false;
            this.cdgv.AllowUserToResizeRows = false;
            this.cdgv.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.cdgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.cdgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colRuleAttribute,
            this.colOperator,
            this.colValue,
            this.colBackColor,
            this.colTextColor,
            this.colColorSample});
            this.cdgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cdgv.Location = new System.Drawing.Point(0, 26);
            this.cdgv.Margin = new System.Windows.Forms.Padding(2);
            this.cdgv.MultiSelect = false;
            this.cdgv.Name = "cdgv";
            this.cdgv.RowHeadersVisible = false;
            this.cdgv.RowTemplate.Height = 24;
            this.cdgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.cdgv.Size = new System.Drawing.Size(736, 266);
            this.cdgv.TabIndex = 0;
            this.cdgv.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.cdgv_CellBeginEdit);
            this.cdgv.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.cdgv_CellDoubleClick);
            this.cdgv.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.cdgv_CellEnter);
            this.cdgv.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.cdgv_CellPainting);
            // 
            // colRuleAttribute
            // 
            this.colRuleAttribute.DataPropertyName = "RuleAttribute";
            this.colRuleAttribute.HeaderText = "Rule Attribute";
            this.colRuleAttribute.Name = "colRuleAttribute";
            this.colRuleAttribute.ReadOnly = true;
            this.colRuleAttribute.Width = 120;
            // 
            // colOperator
            // 
            this.colOperator.DataPropertyName = "Operator";
            this.colOperator.HeaderText = "Operator";
            this.colOperator.Name = "colOperator";
            this.colOperator.Width = 120;
            // 
            // colValue
            // 
            this.colValue.DataPropertyName = "Value";
            this.colValue.HeaderText = "Value";
            this.colValue.Name = "colValue";
            this.colValue.Width = 400;
            // 
            // colBackColor
            // 
            this.colBackColor.DataPropertyName = "BackColor";
            this.colBackColor.HeaderText = "Back Color";
            this.colBackColor.Name = "colBackColor";
            this.colBackColor.ReadOnly = true;
            // 
            // colTextColor
            // 
            this.colTextColor.DataPropertyName = "TextColor";
            this.colTextColor.HeaderText = "Text Color";
            this.colTextColor.Name = "colTextColor";
            this.colTextColor.ReadOnly = true;
            // 
            // colColorSample
            // 
            this.colColorSample.DataPropertyName = "SampleText";
            this.colColorSample.HeaderText = "Sample Text";
            this.colColorSample.Name = "colColorSample";
            this.colColorSample.ReadOnly = true;
            this.colColorSample.Width = 120;
            // 
            // pnlButtons
            // 
            this.pnlButtons.ColumnCount = 11;
            this.pnlButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.pnlButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.pnlButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pnlButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.pnlButtons.Controls.Add(this.btnApply, 10, 0);
            this.pnlButtons.Controls.Add(this.ddRuleAttributes, 0, 0);
            this.pnlButtons.Controls.Add(this.lnkMoveRuleUp, 4, 0);
            this.pnlButtons.Controls.Add(this.lnkMoveRuleDown, 5, 0);
            this.pnlButtons.Controls.Add(this.btnDeleteRules, 2, 0);
            this.pnlButtons.Controls.Add(this.btnAddRule, 1, 0);
            this.pnlButtons.Controls.Add(this.lnkSave, 7, 0);
            this.pnlButtons.Controls.Add(this.lnkLoad, 8, 0);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlButtons.Location = new System.Drawing.Point(0, 0);
            this.pnlButtons.Margin = new System.Windows.Forms.Padding(2);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.RowCount = 1;
            this.pnlButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pnlButtons.Size = new System.Drawing.Size(736, 26);
            this.pnlButtons.TabIndex = 1;
            // 
            // btnApply
            // 
            this.btnApply.AutoSize = true;
            this.btnApply.Location = new System.Drawing.Point(628, 2);
            this.btnApply.Margin = new System.Windows.Forms.Padding(2);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(106, 22);
            this.btnApply.TabIndex = 3;
            this.btnApply.Text = "Apply Color Rule(s)";
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // ddRuleAttributes
            // 
            this.ddRuleAttributes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddRuleAttributes.FormattingEnabled = true;
            this.ddRuleAttributes.Location = new System.Drawing.Point(2, 2);
            this.ddRuleAttributes.Margin = new System.Windows.Forms.Padding(2);
            this.ddRuleAttributes.Name = "ddRuleAttributes";
            this.ddRuleAttributes.Size = new System.Drawing.Size(92, 21);
            this.ddRuleAttributes.TabIndex = 4;
            // 
            // lnkMoveRuleUp
            // 
            this.lnkMoveRuleUp.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lnkMoveRuleUp.AutoSize = true;
            this.lnkMoveRuleUp.Location = new System.Drawing.Point(235, 6);
            this.lnkMoveRuleUp.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lnkMoveRuleUp.Name = "lnkMoveRuleUp";
            this.lnkMoveRuleUp.Size = new System.Drawing.Size(51, 13);
            this.lnkMoveRuleUp.TabIndex = 5;
            this.lnkMoveRuleUp.TabStop = true;
            this.lnkMoveRuleUp.Text = "Move Up";
            this.lnkMoveRuleUp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkMoveRuleUp_LinkClicked);
            // 
            // lnkMoveRuleDown
            // 
            this.lnkMoveRuleDown.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lnkMoveRuleDown.AutoSize = true;
            this.lnkMoveRuleDown.Location = new System.Drawing.Point(290, 6);
            this.lnkMoveRuleDown.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lnkMoveRuleDown.Name = "lnkMoveRuleDown";
            this.lnkMoveRuleDown.Size = new System.Drawing.Size(65, 13);
            this.lnkMoveRuleDown.TabIndex = 6;
            this.lnkMoveRuleDown.TabStop = true;
            this.lnkMoveRuleDown.Text = "Move Down";
            this.lnkMoveRuleDown.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkMoveRuleDown_LinkClicked);
            // 
            // btnDeleteRules
            // 
            this.btnDeleteRules.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnDeleteRules.AutoSize = true;
            this.btnDeleteRules.Location = new System.Drawing.Point(153, 6);
            this.btnDeleteRules.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.btnDeleteRules.Name = "btnDeleteRules";
            this.btnDeleteRules.Size = new System.Drawing.Size(63, 13);
            this.btnDeleteRules.TabIndex = 1;
            this.btnDeleteRules.TabStop = true;
            this.btnDeleteRules.Text = "Delete Rule";
            this.btnDeleteRules.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.btnDeleteRules_LinkClicked);
            // 
            // btnAddRule
            // 
            this.btnAddRule.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnAddRule.AutoSize = true;
            this.btnAddRule.Location = new System.Drawing.Point(98, 6);
            this.btnAddRule.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.btnAddRule.Name = "btnAddRule";
            this.btnAddRule.Size = new System.Drawing.Size(51, 13);
            this.btnAddRule.TabIndex = 0;
            this.btnAddRule.TabStop = true;
            this.btnAddRule.Text = "Add Rule";
            this.btnAddRule.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.btnAddRule_LinkClicked);
            // 
            // lnkSave
            // 
            this.lnkSave.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lnkSave.AutoSize = true;
            this.lnkSave.Location = new System.Drawing.Point(379, 6);
            this.lnkSave.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lnkSave.Name = "lnkSave";
            this.lnkSave.Size = new System.Drawing.Size(32, 13);
            this.lnkSave.TabIndex = 8;
            this.lnkSave.TabStop = true;
            this.lnkSave.Text = "Save";
            this.lnkSave.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkSave_LinkClicked);
            // 
            // lnkLoad
            // 
            this.lnkLoad.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lnkLoad.AutoSize = true;
            this.lnkLoad.Location = new System.Drawing.Point(415, 6);
            this.lnkLoad.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lnkLoad.Name = "lnkLoad";
            this.lnkLoad.Size = new System.Drawing.Size(31, 13);
            this.lnkLoad.TabIndex = 7;
            this.lnkLoad.TabStop = true;
            this.lnkLoad.Text = "Load";
            this.lnkLoad.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkLoad_LinkClicked);
            // 
            // openFileDialogXml
            // 
            this.openFileDialogXml.DefaultExt = "xml";
            this.openFileDialogXml.Filter = "XML files|*.xml|All files|*.*";
            // 
            // saveFileDialogXml
            // 
            this.saveFileDialogXml.DefaultExt = "xml";
            this.saveFileDialogXml.Filter = "XML files|*.xml|All files|*.*";
            // 
            // FrmColorRules
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(736, 292);
            this.Controls.Add(this.cdgv);
            this.Controls.Add(this.pnlButtons);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FrmColorRules";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Color Rules";
            ((System.ComponentModel.ISupportInitialize)(this.cdgv)).EndInit();
            this.pnlButtons.ResumeLayout(false);
            this.pnlButtons.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView cdgv;
        private System.Windows.Forms.TableLayoutPanel pnlButtons;
        private System.Windows.Forms.LinkLabel btnAddRule;
        private System.Windows.Forms.LinkLabel btnDeleteRules;
        private System.Windows.Forms.Button btnApply;
        private ComboBox ddRuleAttributes;
        private ColorDialog cellColorDialog;
        private DataGridViewTextBoxColumn colRuleAttribute;
        private DataGridViewComboBoxColumn colOperator;
        private DataGridViewTextBoxColumn colValue;
        private DataGridViewTextBoxColumn colBackColor;
        private DataGridViewTextBoxColumn colTextColor;
        private DataGridViewTextBoxColumn colColorSample;
        private LinkLabel lnkMoveRuleUp;
        private LinkLabel lnkMoveRuleDown;
        private LinkLabel lnkSave;
        private LinkLabel lnkLoad;
        private OpenFileDialog openFileDialogXml;
        private SaveFileDialog saveFileDialogXml;
    }
}