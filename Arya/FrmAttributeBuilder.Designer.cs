namespace Arya
{
    partial class FrmAttributeBuilder
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
            this.tblAttributesConstructs = new System.Windows.Forms.TableLayoutPanel();
            this.dgvConstructs = new System.Windows.Forms.DataGridView();
            this.colConstruct = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblConstructs = new System.Windows.Forms.Label();
            this.dgvAttributes = new System.Windows.Forms.DataGridView();
            this.colAttributeName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblAttributes = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnAccept = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnEditInherited = new System.Windows.Forms.Button();
            this.numUDExpressionLength = new System.Windows.Forms.NumericUpDown();
            this.lblMaxExpressionLength = new System.Windows.Forms.Label();
            this.txtExpression = new System.Windows.Forms.TextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lblExpressionDescription = new System.Windows.Forms.Label();
            this.dgvSampleData = new System.Windows.Forms.DataGridView();
            this.colItemId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLength = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tmrGenerateSamples = new System.Windows.Forms.Timer(this.components);
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tblAttributesConstructs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvConstructs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAttributes)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUDExpressionLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSampleData)).BeginInit();
            this.SuspendLayout();
            // 
            // tblAttributesConstructs
            // 
            this.tblAttributesConstructs.ColumnCount = 1;
            this.tblAttributesConstructs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblAttributesConstructs.Controls.Add(this.dgvConstructs, 0, 1);
            this.tblAttributesConstructs.Controls.Add(this.lblConstructs, 0, 0);
            this.tblAttributesConstructs.Controls.Add(this.dgvAttributes, 0, 3);
            this.tblAttributesConstructs.Controls.Add(this.lblAttributes, 0, 2);
            this.tblAttributesConstructs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblAttributesConstructs.Location = new System.Drawing.Point(0, 0);
            this.tblAttributesConstructs.Name = "tblAttributesConstructs";
            this.tblAttributesConstructs.RowCount = 4;
            this.tblAttributesConstructs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblAttributesConstructs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblAttributesConstructs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblAttributesConstructs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblAttributesConstructs.Size = new System.Drawing.Size(287, 532);
            this.tblAttributesConstructs.TabIndex = 0;
            // 
            // dgvConstructs
            // 
            this.dgvConstructs.AllowUserToAddRows = false;
            this.dgvConstructs.AllowUserToDeleteRows = false;
            this.dgvConstructs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvConstructs.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colConstruct,
            this.colDescription});
            this.dgvConstructs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvConstructs.Enabled = false;
            this.dgvConstructs.Location = new System.Drawing.Point(23, 16);
            this.dgvConstructs.Name = "dgvConstructs";
            this.dgvConstructs.ReadOnly = true;
            this.dgvConstructs.RowHeadersVisible = false;
            this.dgvConstructs.Size = new System.Drawing.Size(256, 247);
            this.dgvConstructs.TabIndex = 6;
            this.dgvConstructs.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvConstructs_CellClick);
            // 
            // colConstruct
            // 
            this.colConstruct.HeaderText = "Construct";
            this.colConstruct.Name = "colConstruct";
            this.colConstruct.ReadOnly = true;
            this.colConstruct.Width = 60;
            // 
            // colDescription
            // 
            this.colDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colDescription.HeaderText = "Description";
            this.colDescription.Name = "colDescription";
            this.colDescription.ReadOnly = true;
            this.colDescription.Width = 85;
            // 
            // lblConstructs
            // 
            this.lblConstructs.AutoSize = true;
            this.lblConstructs.Location = new System.Drawing.Point(23, 0);
            this.lblConstructs.Name = "lblConstructs";
            this.lblConstructs.Size = new System.Drawing.Size(60, 13);
            this.lblConstructs.TabIndex = 2;
            this.lblConstructs.Text = "Constructs:";
            // 
            // dgvAttributes
            // 
            this.dgvAttributes.AllowUserToAddRows = false;
            this.dgvAttributes.AllowUserToDeleteRows = false;
            this.dgvAttributes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvAttributes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAttributes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colAttributeName});
            this.dgvAttributes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvAttributes.Enabled = false;
            this.dgvAttributes.Location = new System.Drawing.Point(23, 282);
            this.dgvAttributes.Name = "dgvAttributes";
            this.dgvAttributes.ReadOnly = true;
            this.dgvAttributes.RowHeadersVisible = false;
            this.dgvAttributes.Size = new System.Drawing.Size(256, 247);
            this.dgvAttributes.TabIndex = 5;
            this.dgvAttributes.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvAttributes_CellClick);
            // 
            // colAttributeName
            // 
            this.colAttributeName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colAttributeName.HeaderText = "Attribute Name";
            this.colAttributeName.Name = "colAttributeName";
            this.colAttributeName.ReadOnly = true;
            this.colAttributeName.Width = 102;
            // 
            // lblAttributes
            // 
            this.lblAttributes.AutoSize = true;
            this.lblAttributes.Location = new System.Drawing.Point(23, 266);
            this.lblAttributes.Name = "lblAttributes";
            this.lblAttributes.Size = new System.Drawing.Size(54, 13);
            this.lblAttributes.TabIndex = 1;
            this.lblAttributes.Text = "Attributes:";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 5;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 190F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.btnAccept, 4, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnCancel, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnEditInherited, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.numUDExpressionLength, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblMaxExpressionLength, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 532);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(784, 29);
            this.tableLayoutPanel2.TabIndex = 4;
            // 
            // btnAccept
            // 
            this.btnAccept.Location = new System.Drawing.Point(706, 3);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(75, 23);
            this.btnAccept.TabIndex = 0;
            this.btnAccept.Text = "Edit";
            this.btnAccept.UseVisualStyleBackColor = true;
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(625, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnEditInherited
            // 
            this.btnEditInherited.Location = new System.Drawing.Point(246, 3);
            this.btnEditInherited.Name = "btnEditInherited";
            this.btnEditInherited.Size = new System.Drawing.Size(90, 23);
            this.btnEditInherited.TabIndex = 2;
            this.btnEditInherited.Text = "Edit Inherited";
            this.btnEditInherited.UseVisualStyleBackColor = true;
            this.btnEditInherited.Click += new System.EventHandler(this.btnEditInherited_Click);
            // 
            // numUDExpressionLength
            // 
            this.numUDExpressionLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.numUDExpressionLength.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numUDExpressionLength.Location = new System.Drawing.Point(193, 6);
            this.numUDExpressionLength.Maximum = new decimal(new int[] {
            4000,
            0,
            0,
            0});
            this.numUDExpressionLength.Name = "numUDExpressionLength";
            this.numUDExpressionLength.Size = new System.Drawing.Size(47, 20);
            this.numUDExpressionLength.TabIndex = 3;
            this.numUDExpressionLength.ValueChanged += new System.EventHandler(this.numUDExpressionLength_ValueChanged);
            // 
            // lblMaxExpressionLength
            // 
            this.lblMaxExpressionLength.AutoSize = true;
            this.lblMaxExpressionLength.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMaxExpressionLength.Location = new System.Drawing.Point(3, 0);
            this.lblMaxExpressionLength.Name = "lblMaxExpressionLength";
            this.lblMaxExpressionLength.Size = new System.Drawing.Size(184, 29);
            this.lblMaxExpressionLength.TabIndex = 4;
            this.lblMaxExpressionLength.Text = "Maximum Value Length (characters)";
            this.lblMaxExpressionLength.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtExpression
            // 
            this.txtExpression.AcceptsReturn = true;
            this.txtExpression.AcceptsTab = true;
            this.txtExpression.BackColor = System.Drawing.SystemColors.Window;
            this.txtExpression.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtExpression.HideSelection = false;
            this.txtExpression.Location = new System.Drawing.Point(0, 0);
            this.txtExpression.Multiline = true;
            this.txtExpression.Name = "txtExpression";
            this.txtExpression.Size = new System.Drawing.Size(493, 382);
            this.txtExpression.TabIndex = 0;
            this.txtExpression.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtExpression_KeyDown);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lblExpressionDescription);
            this.splitContainer1.Panel1.Controls.Add(this.txtExpression);
            this.splitContainer1.Panel1.Controls.Add(this.dgvSampleData);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tblAttributesConstructs);
            this.splitContainer1.Size = new System.Drawing.Size(784, 532);
            this.splitContainer1.SplitterDistance = 493;
            this.splitContainer1.TabIndex = 5;
            // 
            // lblExpressionDescription
            // 
            this.lblExpressionDescription.BackColor = System.Drawing.Color.Transparent;
            this.lblExpressionDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblExpressionDescription.Location = new System.Drawing.Point(0, 0);
            this.lblExpressionDescription.Name = "lblExpressionDescription";
            this.lblExpressionDescription.Size = new System.Drawing.Size(493, 382);
            this.lblExpressionDescription.TabIndex = 2;
            // 
            // dgvSampleData
            // 
            this.dgvSampleData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSampleData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colItemId,
            this.colValue,
            this.colLength});
            this.dgvSampleData.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dgvSampleData.Location = new System.Drawing.Point(0, 382);
            this.dgvSampleData.Name = "dgvSampleData";
            this.dgvSampleData.Size = new System.Drawing.Size(493, 150);
            this.dgvSampleData.TabIndex = 1;
            // 
            // colItemId
            // 
            this.colItemId.DataPropertyName = "ItemId";
            this.colItemId.HeaderText = "Item Id";
            this.colItemId.Name = "colItemId";
            this.colItemId.ReadOnly = true;
            this.colItemId.Width = 75;
            // 
            // colValue
            // 
            this.colValue.DataPropertyName = "Value";
            this.colValue.HeaderText = "Sample Values";
            this.colValue.Name = "colValue";
            this.colValue.ReadOnly = true;
            this.colValue.Width = 300;
            // 
            // colLength
            // 
            this.colLength.DataPropertyName = "Length";
            this.colLength.HeaderText = "Length";
            this.colLength.Name = "colLength";
            this.colLength.ReadOnly = true;
            // 
            // tmrGenerateSamples
            // 
            this.tmrGenerateSamples.Enabled = true;
            this.tmrGenerateSamples.Interval = 1000;
            this.tmrGenerateSamples.Tick += new System.EventHandler(this.tmrGenerateSamples_Tick);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "ItemId";
            this.dataGridViewTextBoxColumn1.HeaderText = "Item Id";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 75;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.DataPropertyName = "Value";
            this.dataGridViewTextBoxColumn2.HeaderText = "Sample Values";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Width = 350;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.DataPropertyName = "Length";
            this.dataGridViewTextBoxColumn3.HeaderText = "Construct";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.Width = 60;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn4.HeaderText = "Description";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn5.HeaderText = "Attribute Name";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn6.HeaderText = "Attribute Name";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            // 
            // FrmAttributeBuilder
            // 
            this.AcceptButton = this.btnAccept;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.tableLayoutPanel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "FrmAttributeBuilder";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Attribute Builder";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmAttributeBuilder_FormClosing);
            this.Load += new System.EventHandler(this.FrmAttributeBuilder_Load);
            this.tblAttributesConstructs.ResumeLayout(false);
            this.tblAttributesConstructs.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvConstructs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAttributes)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUDExpressionLength)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSampleData)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tblAttributesConstructs;
        private System.Windows.Forms.Label lblAttributes;
        private System.Windows.Forms.Label lblConstructs;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TextBox txtExpression;
        private System.Windows.Forms.DataGridView dgvConstructs;
        private System.Windows.Forms.DataGridView dgvAttributes;
        private System.Windows.Forms.DataGridViewTextBoxColumn colConstruct;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAttributeName;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dgvSampleData;
        private System.Windows.Forms.Timer tmrGenerateSamples;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.Label lblExpressionDescription;
        private System.Windows.Forms.Button btnEditInherited;
        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.NumericUpDown numUDExpressionLength;
        private System.Windows.Forms.DataGridViewTextBoxColumn colItemId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLength;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.Label lblMaxExpressionLength;
    }
}