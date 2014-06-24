namespace Arya.Framework.GUI.Forms
{
    partial class FilterTextEditor
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
            this.tlpFilterForm = new System.Windows.Forms.TableLayoutPanel();
            this.tbFilter = new System.Windows.Forms.TextBox();
            this.tbnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblDescription = new System.Windows.Forms.Label();
            this.tlpFilterForm.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpFilterForm
            // 
            this.tlpFilterForm.ColumnCount = 3;
            this.tlpFilterForm.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpFilterForm.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tlpFilterForm.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tlpFilterForm.Controls.Add(this.tbFilter, 0, 1);
            this.tlpFilterForm.Controls.Add(this.tbnOk, 1, 2);
            this.tlpFilterForm.Controls.Add(this.btnCancel, 2, 2);
            this.tlpFilterForm.Controls.Add(this.lblDescription, 0, 0);
            this.tlpFilterForm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpFilterForm.Location = new System.Drawing.Point(0, 0);
            this.tlpFilterForm.Name = "tlpFilterForm";
            this.tlpFilterForm.RowCount = 3;
            this.tlpFilterForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tlpFilterForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpFilterForm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tlpFilterForm.Size = new System.Drawing.Size(438, 461);
            this.tlpFilterForm.TabIndex = 0;
            // 
            // tbFilter
            // 
            this.tlpFilterForm.SetColumnSpan(this.tbFilter, 3);
            this.tbFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbFilter.Location = new System.Drawing.Point(3, 33);
            this.tbFilter.Multiline = true;
            this.tbFilter.Name = "tbFilter";
            this.tbFilter.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbFilter.Size = new System.Drawing.Size(432, 385);
            this.tbFilter.TabIndex = 0;
            // 
            // tbnOk
            // 
            this.tbnOk.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbnOk.Location = new System.Drawing.Point(241, 424);
            this.tbnOk.Name = "tbnOk";
            this.tbnOk.Size = new System.Drawing.Size(94, 34);
            this.tbnOk.TabIndex = 1;
            this.tbnOk.Text = "Ok";
            this.tbnOk.UseVisualStyleBackColor = true;
            this.tbnOk.Click += new System.EventHandler(this.tbnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCancel.Location = new System.Drawing.Point(341, 424);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(94, 34);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.tlpFilterForm.SetColumnSpan(this.lblDescription, 3);
            this.lblDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDescription.Location = new System.Drawing.Point(3, 0);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(432, 30);
            this.lblDescription.TabIndex = 3;
            // 
            // FilterTextEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(438, 461);
            this.Controls.Add(this.tlpFilterForm);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FilterTextEditor";
            this.Text = "Filter Text Editor";
            this.tlpFilterForm.ResumeLayout(false);
            this.tlpFilterForm.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpFilterForm;
        private System.Windows.Forms.TextBox tbFilter;
        private System.Windows.Forms.Button tbnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblDescription;

    }
}