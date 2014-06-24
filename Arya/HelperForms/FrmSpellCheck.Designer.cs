namespace Arya.HelperForms
{
    partial class FrmSpellCheck
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
            this.btnIgnore = new System.Windows.Forms.Button();
            this.btnIgnoreAll = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.listBox = new System.Windows.Forms.ListBox();
            this.labelCurrentWord = new System.Windows.Forms.Label();
            this.lblLocation = new System.Windows.Forms.Label();
            this.btnChange = new System.Windows.Forms.Button();
            this.btnChangeAll = new System.Windows.Forms.Button();
            this.richTextValue = new System.Windows.Forms.RichTextBox();
            this.btnSkip = new System.Windows.Forms.Button();
            this.lblCurrentContext = new System.Windows.Forms.Label();
            this.toolTipIgnoreAll = new System.Windows.Forms.ToolTip(this.components);
            this.toolTipChangeAll = new System.Windows.Forms.ToolTip(this.components);
            this.grpValue = new System.Windows.Forms.GroupBox();
            this.grpWord = new System.Windows.Forms.GroupBox();
            this.btnReplace = new System.Windows.Forms.Button();
            this.richTextBoxMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ignoreThisWordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ignoreAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToDictionaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tmrWait = new System.Windows.Forms.Timer(this.components);
            this.grpValue.SuspendLayout();
            this.grpWord.SuspendLayout();
            this.richTextBoxMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnIgnore
            // 
            this.btnIgnore.Location = new System.Drawing.Point(227, 47);
            this.btnIgnore.Name = "btnIgnore";
            this.btnIgnore.Size = new System.Drawing.Size(114, 23);
            this.btnIgnore.TabIndex = 1;
            this.btnIgnore.Text = "Ignore this word";
            this.btnIgnore.UseVisualStyleBackColor = true;
            this.btnIgnore.Click += new System.EventHandler(this.btnIgnore_Click);
            // 
            // btnIgnoreAll
            // 
            this.btnIgnoreAll.Location = new System.Drawing.Point(227, 76);
            this.btnIgnoreAll.Name = "btnIgnoreAll";
            this.btnIgnoreAll.Size = new System.Drawing.Size(114, 23);
            this.btnIgnoreAll.TabIndex = 2;
            this.btnIgnoreAll.Text = "Ignore All";
            this.btnIgnoreAll.UseVisualStyleBackColor = true;
            this.btnIgnoreAll.Click += new System.EventHandler(this.btnIgnoreAll_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(227, 105);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(114, 23);
            this.btnAdd.TabIndex = 3;
            this.btnAdd.Text = "Add to Dictionary";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // listBox
            // 
            this.listBox.FormattingEnabled = true;
            this.listBox.Location = new System.Drawing.Point(9, 46);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(212, 82);
            this.listBox.TabIndex = 4;
            this.listBox.Click += new System.EventHandler(this.listBox_Click);
            this.listBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBox_MouseDoubleClick);
            // 
            // labelCurrentWord
            // 
            this.labelCurrentWord.AutoSize = true;
            this.labelCurrentWord.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCurrentWord.Location = new System.Drawing.Point(6, 18);
            this.labelCurrentWord.Name = "labelCurrentWord";
            this.labelCurrentWord.Size = new System.Drawing.Size(82, 13);
            this.labelCurrentWord.TabIndex = 7;
            this.labelCurrentWord.Text = "Current Word";
            this.labelCurrentWord.UseMnemonic = false;
            // 
            // lblLocation
            // 
            this.lblLocation.AutoSize = true;
            this.lblLocation.Location = new System.Drawing.Point(6, 16);
            this.lblLocation.Name = "lblLocation";
            this.lblLocation.Size = new System.Drawing.Size(59, 13);
            this.lblLocation.TabIndex = 9;
            this.lblLocation.Text = "Locationtxt";
            // 
            // btnChange
            // 
            this.btnChange.Location = new System.Drawing.Point(359, 95);
            this.btnChange.Name = "btnChange";
            this.btnChange.Size = new System.Drawing.Size(75, 23);
            this.btnChange.TabIndex = 10;
            this.btnChange.Text = "Change";
            this.btnChange.UseVisualStyleBackColor = true;
            this.btnChange.Click += new System.EventHandler(this.btnChange_Click);
            // 
            // btnChangeAll
            // 
            this.btnChangeAll.Location = new System.Drawing.Point(359, 124);
            this.btnChangeAll.Name = "btnChangeAll";
            this.btnChangeAll.Size = new System.Drawing.Size(75, 23);
            this.btnChangeAll.TabIndex = 11;
            this.btnChangeAll.Text = "Change All";
            this.btnChangeAll.UseVisualStyleBackColor = true;
            this.btnChangeAll.Click += new System.EventHandler(this.btnChangeAll_Click);
            // 
            // richTextValue
            // 
            this.richTextValue.Location = new System.Drawing.Point(6, 32);
            this.richTextValue.Name = "richTextValue";
            this.richTextValue.Size = new System.Drawing.Size(347, 115);
            this.richTextValue.TabIndex = 12;
            this.richTextValue.Text = "";
            this.richTextValue.SelectionChanged += new System.EventHandler(this.richTextValue_SelectionChanged);
            this.richTextValue.Click += new System.EventHandler(this.richTextValue_Click);
            this.richTextValue.TextChanged += new System.EventHandler(this.richTextValue_TextChanged);
            // 
            // btnSkip
            // 
            this.btnSkip.Location = new System.Drawing.Point(359, 32);
            this.btnSkip.Name = "btnSkip";
            this.btnSkip.Size = new System.Drawing.Size(75, 23);
            this.btnSkip.TabIndex = 13;
            this.btnSkip.Text = "Skip";
            this.btnSkip.UseVisualStyleBackColor = true;
            this.btnSkip.Click += new System.EventHandler(this.btnSkip_Click);
            // 
            // lblCurrentContext
            // 
            this.lblCurrentContext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCurrentContext.AutoSize = true;
            this.lblCurrentContext.Location = new System.Drawing.Point(375, 9);
            this.lblCurrentContext.Name = "lblCurrentContext";
            this.lblCurrentContext.Size = new System.Drawing.Size(77, 13);
            this.lblCurrentContext.TabIndex = 15;
            this.lblCurrentContext.Text = "CurrentContext";
            // 
            // grpValue
            // 
            this.grpValue.Controls.Add(this.grpWord);
            this.grpValue.Controls.Add(this.richTextValue);
            this.grpValue.Controls.Add(this.lblLocation);
            this.grpValue.Controls.Add(this.btnChangeAll);
            this.grpValue.Controls.Add(this.btnSkip);
            this.grpValue.Controls.Add(this.btnChange);
            this.grpValue.Location = new System.Drawing.Point(12, 25);
            this.grpValue.Name = "grpValue";
            this.grpValue.Size = new System.Drawing.Size(440, 293);
            this.grpValue.TabIndex = 16;
            this.grpValue.TabStop = false;
            this.grpValue.Text = "Value";
            // 
            // grpWord
            // 
            this.grpWord.Controls.Add(this.btnReplace);
            this.grpWord.Controls.Add(this.labelCurrentWord);
            this.grpWord.Controls.Add(this.listBox);
            this.grpWord.Controls.Add(this.btnIgnore);
            this.grpWord.Controls.Add(this.btnIgnoreAll);
            this.grpWord.Controls.Add(this.btnAdd);
            this.grpWord.Location = new System.Drawing.Point(6, 153);
            this.grpWord.Name = "grpWord";
            this.grpWord.Size = new System.Drawing.Size(347, 134);
            this.grpWord.TabIndex = 14;
            this.grpWord.TabStop = false;
            this.grpWord.Text = "Word";
            // 
            // btnReplace
            // 
            this.btnReplace.Enabled = false;
            this.btnReplace.Location = new System.Drawing.Point(227, 20);
            this.btnReplace.Name = "btnReplace";
            this.btnReplace.Size = new System.Drawing.Size(114, 23);
            this.btnReplace.TabIndex = 8;
            this.btnReplace.Text = "Replace";
            this.btnReplace.UseVisualStyleBackColor = true;
            this.btnReplace.Click += new System.EventHandler(this.btnReplace_Click);
            // 
            // richTextBoxMenu
            // 
            this.richTextBoxMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ignoreThisWordToolStripMenuItem,
            this.ignoreAllToolStripMenuItem,
            this.addToDictionaryToolStripMenuItem});
            this.richTextBoxMenu.Name = "richTextBoxMenu";
            this.richTextBoxMenu.Size = new System.Drawing.Size(168, 70);
            // 
            // ignoreThisWordToolStripMenuItem
            // 
            this.ignoreThisWordToolStripMenuItem.Name = "ignoreThisWordToolStripMenuItem";
            this.ignoreThisWordToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.ignoreThisWordToolStripMenuItem.Text = "Ignore this Word";
            // 
            // ignoreAllToolStripMenuItem
            // 
            this.ignoreAllToolStripMenuItem.Name = "ignoreAllToolStripMenuItem";
            this.ignoreAllToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.ignoreAllToolStripMenuItem.Text = "Ignore All";
            // 
            // addToDictionaryToolStripMenuItem
            // 
            this.addToDictionaryToolStripMenuItem.Name = "addToDictionaryToolStripMenuItem";
            this.addToDictionaryToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.addToDictionaryToolStripMenuItem.Text = "Add to Dictionary";
            // 
            // tmrWait
            // 
            this.tmrWait.Interval = 500;
            this.tmrWait.Tick += new System.EventHandler(this.tmrWait_Tick);
            // 
            // FrmSpellCheck
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 321);
            this.Controls.Add(this.grpValue);
            this.Controls.Add(this.lblCurrentContext);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmSpellCheck";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Spell Check";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmSpellCheck_FormClosing);
            this.grpValue.ResumeLayout(false);
            this.grpValue.PerformLayout();
            this.grpWord.ResumeLayout(false);
            this.grpWord.PerformLayout();
            this.richTextBoxMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnIgnore;
        private System.Windows.Forms.Button btnIgnoreAll;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.ListBox listBox;
        private System.Windows.Forms.Label labelCurrentWord;
        private System.Windows.Forms.Label lblLocation;
        private System.Windows.Forms.Button btnChange;
        private System.Windows.Forms.Button btnChangeAll;
        private System.Windows.Forms.RichTextBox richTextValue;
        private System.Windows.Forms.Button btnSkip;
        private System.Windows.Forms.Label lblCurrentContext;
        private System.Windows.Forms.ToolTip toolTipIgnoreAll;
        private System.Windows.Forms.ToolTip toolTipChangeAll;
        private System.Windows.Forms.GroupBox grpValue;
        private System.Windows.Forms.GroupBox grpWord;
        private System.Windows.Forms.Button btnReplace;
        private System.Windows.Forms.ContextMenuStrip richTextBoxMenu;
        private System.Windows.Forms.ToolStripMenuItem ignoreThisWordToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ignoreAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addToDictionaryToolStripMenuItem;
        private System.Windows.Forms.Timer tmrWait;
    }
}