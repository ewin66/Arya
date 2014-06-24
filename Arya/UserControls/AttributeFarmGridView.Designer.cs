namespace Arya.UserControls
{
    partial class AttributeFarmGridView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tblFilter = new System.Windows.Forms.TableLayoutPanel();
            this.txtAttributeNameFilter = new System.Windows.Forms.TextBox();
            this.ddAttributeTypeFilter = new System.Windows.Forms.ComboBox();
            this.ddAttributeGroupFilter = new System.Windows.Forms.ComboBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.dgvAttributes = new System.Windows.Forms.DataGridView();
            this.colSelected = new Arya.Framework.GUI.UserControls.DataGridViewImageCheckBoxColumn();
            this.colAttributeName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAttributeType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAttributeGroup = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tmrUpdateDgv = new System.Windows.Forms.Timer(this.components);
            this.dataGridViewImageCheckBoxColumn1 = new Arya.Framework.GUI.UserControls.DataGridViewImageCheckBoxColumn();
            this.sfdAttributeData = new System.Windows.Forms.SaveFileDialog();
            this.tblFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAttributes)).BeginInit();
            this.SuspendLayout();
            // 
            // tblFilter
            // 
            this.tblFilter.AutoSize = true;
            this.tblFilter.ColumnCount = 4;
            this.tblFilter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblFilter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblFilter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblFilter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblFilter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblFilter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblFilter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblFilter.Controls.Add(this.txtAttributeNameFilter, 1, 0);
            this.tblFilter.Controls.Add(this.ddAttributeTypeFilter, 2, 0);
            this.tblFilter.Controls.Add(this.ddAttributeGroupFilter, 3, 0);
            this.tblFilter.Controls.Add(this.btnSave, 0, 0);
            this.tblFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.tblFilter.Location = new System.Drawing.Point(0, 0);
            this.tblFilter.Name = "tblFilter";
            this.tblFilter.RowCount = 1;
            this.tblFilter.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblFilter.Size = new System.Drawing.Size(371, 28);
            this.tblFilter.TabIndex = 2;
            // 
            // txtAttributeNameFilter
            // 
            this.txtAttributeNameFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAttributeNameFilter.Location = new System.Drawing.Point(31, 3);
            this.txtAttributeNameFilter.Name = "txtAttributeNameFilter";
            this.txtAttributeNameFilter.Size = new System.Drawing.Size(83, 20);
            this.txtAttributeNameFilter.TabIndex = 0;
            this.txtAttributeNameFilter.TextChanged += new System.EventHandler(this.FilterUpdated);
            // 
            // ddAttributeTypeFilter
            // 
            this.ddAttributeTypeFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddAttributeTypeFilter.FormattingEnabled = true;
            this.ddAttributeTypeFilter.Location = new System.Drawing.Point(120, 3);
            this.ddAttributeTypeFilter.Name = "ddAttributeTypeFilter";
            this.ddAttributeTypeFilter.Size = new System.Drawing.Size(121, 21);
            this.ddAttributeTypeFilter.TabIndex = 5;
            this.ddAttributeTypeFilter.SelectedIndexChanged += new System.EventHandler(this.FilterUpdated);
            // 
            // ddAttributeGroupFilter
            // 
            this.ddAttributeGroupFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddAttributeGroupFilter.FormattingEnabled = true;
            this.ddAttributeGroupFilter.Location = new System.Drawing.Point(247, 3);
            this.ddAttributeGroupFilter.Name = "ddAttributeGroupFilter";
            this.ddAttributeGroupFilter.Size = new System.Drawing.Size(121, 21);
            this.ddAttributeGroupFilter.TabIndex = 6;
            this.ddAttributeGroupFilter.SelectedIndexChanged += new System.EventHandler(this.FilterUpdated);
            // 
            // btnSave
            // 
            this.btnSave.AutoSize = true;
            this.btnSave.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSave.Image = global::Arya.Properties.Resources.Save;
            this.btnSave.Location = new System.Drawing.Point(3, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(22, 22);
            this.btnSave.TabIndex = 7;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // dgvAttributes
            // 
            this.dgvAttributes.AllowUserToDeleteRows = false;
            this.dgvAttributes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAttributes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSelected,
            this.colAttributeName,
            this.colAttributeType,
            this.colAttributeGroup});
            this.dgvAttributes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvAttributes.Location = new System.Drawing.Point(0, 28);
            this.dgvAttributes.Name = "dgvAttributes";
            this.dgvAttributes.RowTemplate.Height = 24;
            this.dgvAttributes.Size = new System.Drawing.Size(371, 448);
            this.dgvAttributes.TabIndex = 3;
            this.dgvAttributes.VirtualMode = true;
            this.dgvAttributes.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgvAllAttributes_CellBeginEdit);
            this.dgvAttributes.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.adgv_CellMouseClick);
            this.dgvAttributes.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dgvAllAttributes_CellValueNeeded);
            this.dgvAttributes.CellValuePushed += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dgvAllAttributes_CellValuePushed);
            this.dgvAttributes.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvAllAttributes_ColumnHeaderMouseClick);
            this.dgvAttributes.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.adgv_EditingControlShowing);
            // 
            // colSelected
            // 
            this.colSelected.CheckedImage = global::Arya.Properties.Resources.bullet_red;
            this.colSelected.HeaderText = "";
            this.colSelected.Name = "colSelected";
            this.colSelected.ReadOnly = true;
            this.colSelected.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colSelected.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colSelected.UnCheckedImage = global::Arya.Properties.Resources.bullet_green;
            this.colSelected.Visible = false;
            this.colSelected.Width = 25;
            // 
            // colAttributeName
            // 
            this.colAttributeName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.colAttributeName.DataPropertyName = "AttributeName";
            this.colAttributeName.HeaderText = "Attribute Name";
            this.colAttributeName.Name = "colAttributeName";
            this.colAttributeName.Width = 94;
            // 
            // colAttributeType
            // 
            this.colAttributeType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.colAttributeType.DataPropertyName = "AttributeType";
            this.colAttributeType.HeaderText = "Attribute Type";
            this.colAttributeType.Name = "colAttributeType";
            // 
            // colAttributeGroup
            // 
            this.colAttributeGroup.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.colAttributeGroup.DataPropertyName = "AttributeGroup";
            this.colAttributeGroup.HeaderText = "Attribute Group";
            this.colAttributeGroup.Name = "colAttributeGroup";
            this.colAttributeGroup.Width = 120;
            // 
            // tmrUpdateDgv
            // 
            this.tmrUpdateDgv.Interval = 1000;
            this.tmrUpdateDgv.Tick += new System.EventHandler(this.tmrUpdateDgv_Tick);
            // 
            // dataGridViewImageCheckBoxColumn1
            // 
            this.dataGridViewImageCheckBoxColumn1.CheckedImage = global::Arya.Properties.Resources.bullet_red;
            this.dataGridViewImageCheckBoxColumn1.HeaderText = "";
            this.dataGridViewImageCheckBoxColumn1.Name = "dataGridViewImageCheckBoxColumn1";
            this.dataGridViewImageCheckBoxColumn1.ReadOnly = true;
            this.dataGridViewImageCheckBoxColumn1.UnCheckedImage = global::Arya.Properties.Resources.bullet_green;
            this.dataGridViewImageCheckBoxColumn1.Visible = false;
            this.dataGridViewImageCheckBoxColumn1.Width = 25;
            // 
            // sfdAttributeData
            // 
            this.sfdAttributeData.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            // 
            // AttributeFarmGridView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgvAttributes);
            this.Controls.Add(this.tblFilter);
            this.Name = "AttributeFarmGridView";
            this.Size = new System.Drawing.Size(371, 476);
            this.tblFilter.ResumeLayout(false);
            this.tblFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAttributes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tblFilter;
        private System.Windows.Forms.TextBox txtAttributeNameFilter;
        private System.Windows.Forms.ComboBox ddAttributeTypeFilter;
        private System.Windows.Forms.ComboBox ddAttributeGroupFilter;
        private System.Windows.Forms.DataGridView dgvAttributes;
        private System.Windows.Forms.Timer tmrUpdateDgv;
        private Framework.GUI.UserControls.DataGridViewImageCheckBoxColumn dataGridViewImageCheckBoxColumn1;
        private Framework.GUI.UserControls.DataGridViewImageCheckBoxColumn colSelected;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAttributeName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAttributeType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAttributeGroup;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.SaveFileDialog sfdAttributeData;
    }
}
