namespace Arya.Framework.GUI.Forms
{
    partial class ImportFieldMapper
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
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportFieldMapper));
            this.Required = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.FieldName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.MapTo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lstMapping = new System.Windows.Forms.ListView();
            this.lblMapTo = new System.Windows.Forms.Label();
            this.lstFileFields = new System.Windows.Forms.ListBox();
            this.lblAvailableFields = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Required
            // 
            this.Required.Text = "Required";
            this.Required.Width = 59;
            // 
            // FieldName
            // 
            this.FieldName.Text = "Field Name";
            this.FieldName.Width = 120;
            // 
            // MapTo
            // 
            this.MapTo.Text = "Mapped to";
            this.MapTo.Width = 120;
            // 
            // lstMapping
            // 
            this.lstMapping.AllowDrop = true;
            this.lstMapping.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Required,
            this.FieldName,
            this.MapTo});
            this.lstMapping.FullRowSelect = true;
            this.lstMapping.GridLines = true;
            this.lstMapping.HideSelection = false;
            this.lstMapping.LabelWrap = false;
            this.lstMapping.Location = new System.Drawing.Point(164, 25);
            this.lstMapping.MultiSelect = false;
            this.lstMapping.Name = "lstMapping";
            this.lstMapping.ShowGroups = false;
            this.lstMapping.Size = new System.Drawing.Size(312, 303);
            this.lstMapping.TabIndex = 13;
            this.lstMapping.UseCompatibleStateImageBehavior = false;
            this.lstMapping.View = System.Windows.Forms.View.Details;
            this.lstMapping.DragDrop += new System.Windows.Forms.DragEventHandler(this.lstMapping_DragDrop);
            this.lstMapping.DragOver += new System.Windows.Forms.DragEventHandler(this.lstMapping_DragOver);
            this.lstMapping.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lstMapping_KeyUp);
            // 
            // lblMapTo
            // 
            this.lblMapTo.AutoSize = true;
            this.lblMapTo.Location = new System.Drawing.Point(161, 9);
            this.lblMapTo.Name = "lblMapTo";
            this.lblMapTo.Size = new System.Drawing.Size(70, 13);
            this.lblMapTo.TabIndex = 12;
            this.lblMapTo.Text = "Map fields to:";
            // 
            // lstFileFields
            // 
            this.lstFileFields.FormattingEnabled = true;
            this.lstFileFields.Location = new System.Drawing.Point(12, 25);
            this.lstFileFields.Name = "lstFileFields";
            this.lstFileFields.Size = new System.Drawing.Size(146, 303);
            this.lstFileFields.TabIndex = 11;
            this.lstFileFields.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lstFileFields_MouseDown);
            // 
            // lblAvailableFields
            // 
            this.lblAvailableFields.AutoSize = true;
            this.lblAvailableFields.Location = new System.Drawing.Point(9, 9);
            this.lblAvailableFields.Name = "lblAvailableFields";
            this.lblAvailableFields.Size = new System.Drawing.Size(125, 13);
            this.lblAvailableFields.TabIndex = 10;
            this.lblAvailableFields.Text = "Available fields in the file:";
            // 
            // ImportFieldMapper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(488, 336);
            this.Controls.Add(this.lblAvailableFields);
            this.Controls.Add(this.lstFileFields);
            this.Controls.Add(this.lblMapTo);
            this.Controls.Add(this.lstMapping);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImportFieldMapper";
            this.Text = "Mapping Editor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ColumnHeader Required;
        private System.Windows.Forms.ColumnHeader FieldName;
        private System.Windows.Forms.ColumnHeader MapTo;
        private System.Windows.Forms.ListView lstMapping;
        private System.Windows.Forms.Label lblMapTo;
        private System.Windows.Forms.ListBox lstFileFields;
        private System.Windows.Forms.Label lblAvailableFields;


    }
}