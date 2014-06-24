namespace Arya.HelperForms
{
    partial class FrmCreateSkuGroup
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.textBoxGroupDescription = new System.Windows.Forms.TextBox();
            this.textBoxGroupName = new System.Windows.Forms.TextBox();
            this.btnShowConflicts = new System.Windows.Forms.Button();
            this.lblGroupDescription = new System.Windows.Forms.Label();
            this.lblGroupName = new System.Windows.Forms.Label();
            this.lblConflict = new System.Windows.Forms.Label();
            this.checkBoxOverwrite = new System.Windows.Forms.CheckBox();
            this.btnCreate = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.93939F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 36.9697F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 29.39394F));
            this.tableLayoutPanel1.Controls.Add(this.textBoxGroupDescription, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBoxGroupName, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnShowConflicts, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblGroupDescription, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblGroupName, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblConflict, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxOverwrite, 2, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(330, 97);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // textBoxGroupDescription
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.textBoxGroupDescription, 2);
            this.textBoxGroupDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxGroupDescription.Location = new System.Drawing.Point(114, 29);
            this.textBoxGroupDescription.Name = "textBoxGroupDescription";
            this.textBoxGroupDescription.Size = new System.Drawing.Size(213, 20);
            this.textBoxGroupDescription.TabIndex = 2;
            // 
            // textBoxGroupName
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.textBoxGroupName, 2);
            this.textBoxGroupName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxGroupName.Location = new System.Drawing.Point(114, 3);
            this.textBoxGroupName.Name = "textBoxGroupName";
            this.textBoxGroupName.Size = new System.Drawing.Size(213, 20);
            this.textBoxGroupName.TabIndex = 1;
            // 
            // btnShowConflicts
            // 
            this.btnShowConflicts.Location = new System.Drawing.Point(114, 55);
            this.btnShowConflicts.Name = "btnShowConflicts";
            this.btnShowConflicts.Size = new System.Drawing.Size(113, 22);
            this.btnShowConflicts.TabIndex = 0;
            this.btnShowConflicts.Text = "Show Groups";
            this.btnShowConflicts.UseVisualStyleBackColor = true;
            this.btnShowConflicts.Click += new System.EventHandler(this.btnShowConflicts_Click);
            // 
            // lblGroupDescription
            // 
            this.lblGroupDescription.AutoSize = true;
            this.lblGroupDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblGroupDescription.Location = new System.Drawing.Point(3, 26);
            this.lblGroupDescription.Name = "lblGroupDescription";
            this.lblGroupDescription.Size = new System.Drawing.Size(105, 26);
            this.lblGroupDescription.TabIndex = 3;
            this.lblGroupDescription.Text = "Group Description :";
            this.lblGroupDescription.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblGroupName
            // 
            this.lblGroupName.AutoSize = true;
            this.lblGroupName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblGroupName.Location = new System.Drawing.Point(3, 0);
            this.lblGroupName.Name = "lblGroupName";
            this.lblGroupName.Size = new System.Drawing.Size(105, 26);
            this.lblGroupName.TabIndex = 4;
            this.lblGroupName.Text = "Group Name :";
            this.lblGroupName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblConflict
            // 
            this.lblConflict.AutoSize = true;
            this.lblConflict.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblConflict.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblConflict.Location = new System.Drawing.Point(3, 52);
            this.lblConflict.Name = "lblConflict";
            this.lblConflict.Size = new System.Drawing.Size(105, 45);
            this.lblConflict.TabIndex = 5;
            this.lblConflict.Text = "One or more SKUs are already linked to existing SKU Groups";
            // 
            // checkBoxOverwrite
            // 
            this.checkBoxOverwrite.AutoSize = true;
            this.checkBoxOverwrite.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkBoxOverwrite.Location = new System.Drawing.Point(235, 55);
            this.checkBoxOverwrite.Name = "checkBoxOverwrite";
            this.checkBoxOverwrite.Size = new System.Drawing.Size(92, 39);
            this.checkBoxOverwrite.TabIndex = 6;
            this.checkBoxOverwrite.Text = "Overwrite";
            this.checkBoxOverwrite.UseVisualStyleBackColor = true;
            // 
            // btnCreate
            // 
            this.btnCreate.Location = new System.Drawing.Point(262, 108);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(80, 22);
            this.btnCreate.TabIndex = 4;
            this.btnCreate.Text = "Create";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // FrmCreateSkuGroup
            // 
            this.AcceptButton = this.btnCreate;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(353, 135);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmCreateSkuGroup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Create Sku Group";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox textBoxGroupDescription;
        private System.Windows.Forms.TextBox textBoxGroupName;
        private System.Windows.Forms.Button btnShowConflicts;
        private System.Windows.Forms.Label lblGroupDescription;
        private System.Windows.Forms.Label lblGroupName;
        private System.Windows.Forms.Label lblConflict;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.CheckBox checkBoxOverwrite;

    }
}