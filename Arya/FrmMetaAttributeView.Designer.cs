namespace Arya
{
    partial class FrmMetaAttributeView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMetaAttributeView));
            this.tabMetaAttributes = new System.Windows.Forms.TabControl();
            this.tabSchema = new System.Windows.Forms.TabPage();
            this.cntSchema = new Arya.UserControls.MetaAttributeDataGridView();
            this.tabTaxonomy = new System.Windows.Forms.TabPage();
            this.cntTaxonomy = new Arya.UserControls.MetaAttributeDataGridView();
            this.tabMetaAttributes.SuspendLayout();
            this.tabSchema.SuspendLayout();
            this.tabTaxonomy.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabMetaAttributes
            // 
            this.tabMetaAttributes.Controls.Add(this.tabSchema);
            this.tabMetaAttributes.Controls.Add(this.tabTaxonomy);
            this.tabMetaAttributes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMetaAttributes.Location = new System.Drawing.Point(0, 0);
            this.tabMetaAttributes.Name = "tabMetaAttributes";
            this.tabMetaAttributes.SelectedIndex = 0;
            this.tabMetaAttributes.Size = new System.Drawing.Size(772, 517);
            this.tabMetaAttributes.TabIndex = 0;
            // 
            // tabSchema
            // 
            this.tabSchema.Controls.Add(this.cntSchema);
            this.tabSchema.Location = new System.Drawing.Point(4, 22);
            this.tabSchema.Name = "tabSchema";
            this.tabSchema.Padding = new System.Windows.Forms.Padding(3);
            this.tabSchema.Size = new System.Drawing.Size(764, 491);
            this.tabSchema.TabIndex = 0;
            this.tabSchema.Text = "Schema";
            this.tabSchema.UseVisualStyleBackColor = true;
            // 
            // cntSchema
            // 
            this.cntSchema.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cntSchema.Location = new System.Drawing.Point(3, 3);
            this.cntSchema.MetaType = Arya.UserControls.MetaTypeEnum.Schema;
            this.cntSchema.Name = "cntSchema";
            this.cntSchema.Size = new System.Drawing.Size(758, 485);
            this.cntSchema.TabIndex = 0;
            // 
            // tabTaxonomy
            // 
            this.tabTaxonomy.Controls.Add(this.cntTaxonomy);
            this.tabTaxonomy.Location = new System.Drawing.Point(4, 22);
            this.tabTaxonomy.Name = "tabTaxonomy";
            this.tabTaxonomy.Padding = new System.Windows.Forms.Padding(3);
            this.tabTaxonomy.Size = new System.Drawing.Size(764, 491);
            this.tabTaxonomy.TabIndex = 1;
            this.tabTaxonomy.Text = "Taxonomy";
            this.tabTaxonomy.UseVisualStyleBackColor = true;
            // 
            // cntTaxonomy
            // 
            this.cntTaxonomy.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cntTaxonomy.Location = new System.Drawing.Point(3, 3);
            this.cntTaxonomy.MetaType = Arya.UserControls.MetaTypeEnum.Schema;
            this.cntTaxonomy.Name = "cntTaxonomy";
            this.cntTaxonomy.Size = new System.Drawing.Size(758, 485);
            this.cntTaxonomy.TabIndex = 0;
            // 
            // FrmMetaAttributeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(772, 517);
            this.Controls.Add(this.tabMetaAttributes);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmMetaAttributeView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Project Meta-Attributes";
            this.tabMetaAttributes.ResumeLayout(false);
            this.tabSchema.ResumeLayout(false);
            this.tabTaxonomy.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabMetaAttributes;
        private System.Windows.Forms.TabPage tabSchema;
        private System.Windows.Forms.TabPage tabTaxonomy;
        private UserControls.MetaAttributeDataGridView cntSchema;
        private UserControls.MetaAttributeDataGridView cntTaxonomy;
    }
}