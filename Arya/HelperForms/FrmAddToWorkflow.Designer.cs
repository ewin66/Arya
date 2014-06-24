namespace Arya.HelperForms
{
    partial class FrmAddToWorkflow
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.cboWorkflows = new System.Windows.Forms.ComboBox();
            this.workflowBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.lblSelection = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.workflowBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(136, 76);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(217, 76);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 1;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // cboWorkflows
            // 
            this.cboWorkflows.DataSource = this.workflowBindingSource;
            this.cboWorkflows.DisplayMember = "WorkflowName";
            this.cboWorkflows.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboWorkflows.FormattingEnabled = true;
            this.cboWorkflows.Location = new System.Drawing.Point(15, 49);
            this.cboWorkflows.Name = "cboWorkflows";
            this.cboWorkflows.Size = new System.Drawing.Size(277, 21);
            this.cboWorkflows.TabIndex = 2;
            this.cboWorkflows.ValueMember = "ID";
            this.cboWorkflows.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // workflowBindingSource
            // 
            this.workflowBindingSource.DataSource = typeof(Arya.Data.Workflow);
            // 
            // lblSelection
            // 
            this.lblSelection.AutoSize = true;
            this.lblSelection.Location = new System.Drawing.Point(12, 9);
            this.lblSelection.Name = "lblSelection";
            this.lblSelection.Size = new System.Drawing.Size(51, 13);
            this.lblSelection.TabIndex = 3;
            this.lblSelection.Text = "Selection";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Select Workflow:";
            // 
            // FrmAddToWorkflow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(304, 111);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblSelection);
            this.Controls.Add(this.cboWorkflows);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FrmAddToWorkflow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Workflow";
            this.Load += new System.EventHandler(this.FrmAddToWorkflow_Load);
            ((System.ComponentModel.ISupportInitialize)(this.workflowBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.ComboBox cboWorkflows;
        private System.Windows.Forms.Label lblSelection;
        private System.Windows.Forms.BindingSource workflowBindingSource;
        private System.Windows.Forms.Label label2;
    }
}