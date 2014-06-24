namespace Arya.HelperForms
{
    partial class FrmProjectManager
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmProjectManager));
            this.epUser = new System.Windows.Forms.ErrorProvider(this.components);
            this.btnSave = new System.Windows.Forms.Button();
            this.gbProjectUrlSettings = new System.Windows.Forms.GroupBox();
            this.tlpUrlsAndFilters = new System.Windows.Forms.TableLayoutPanel();
            this.lblProductSearchString = new System.Windows.Forms.Label();
            this.tbProductSearchString = new System.Windows.Forms.TextBox();
            this.lblFilters = new System.Windows.Forms.Label();
            this.tbSchemaFillRateFilters = new System.Windows.Forms.TextBox();
            this.tlpEntityFields = new System.Windows.Forms.TableLayoutPanel();
            this.gbEntityField4 = new System.Windows.Forms.GroupBox();
            this.flpEntityField4 = new System.Windows.Forms.FlowLayoutPanel();
            this.tbEntityField4Name = new System.Windows.Forms.TextBox();
            this.cbEntityField4ReadOnly = new System.Windows.Forms.CheckBox();
            this.gbEntityField5 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.cbEntityField5ReadOnly = new System.Windows.Forms.CheckBox();
            this.cbEntityField5IsStatus = new System.Windows.Forms.CheckBox();
            this.tbEntityField5Name = new System.Windows.Forms.TextBox();
            this.gbEntityField1 = new System.Windows.Forms.GroupBox();
            this.flptEntityField1 = new System.Windows.Forms.FlowLayoutPanel();
            this.tbEntityField1Name = new System.Windows.Forms.TextBox();
            this.cbEntityField1ReadOnly = new System.Windows.Forms.CheckBox();
            this.gbEntityField2 = new System.Windows.Forms.GroupBox();
            this.flpEntityField2 = new System.Windows.Forms.FlowLayoutPanel();
            this.tbEntityField2Name = new System.Windows.Forms.TextBox();
            this.cbEntityField2ReadOnly = new System.Windows.Forms.CheckBox();
            this.gnEntityField3 = new System.Windows.Forms.GroupBox();
            this.flpEntityField3 = new System.Windows.Forms.FlowLayoutPanel();
            this.tbEntityField3Name = new System.Windows.Forms.TextBox();
            this.cbEntityField3ReadOnly = new System.Windows.Forms.CheckBox();
            this.gbListSeparator = new System.Windows.Forms.GroupBox();
            this.lblReturnSeparator = new System.Windows.Forms.Label();
            this.tbReturnSeparator = new System.Windows.Forms.TextBox();
            this.lblListSeparator = new System.Windows.Forms.Label();
            this.tbListSeparator = new System.Windows.Forms.TextBox();
            this.gbProjectDetails = new System.Windows.Forms.GroupBox();
            this.tlpProjectDetails = new System.Windows.Forms.TableLayoutPanel();
            this.lblDatabaseName = new System.Windows.Forms.Label();
            this.lblClientDescription = new System.Windows.Forms.Label();
            this.tbDatabaseName = new System.Windows.Forms.TextBox();
            this.tbClientDescription = new System.Windows.Forms.TextBox();
            this.lblProjectName = new System.Windows.Forms.Label();
            this.tbProjectName = new System.Windows.Forms.TextBox();
            this.lblSetName = new System.Windows.Forms.Label();
            this.tbSetName = new System.Windows.Forms.TextBox();
            this.tbAryaCodeBaseVersion = new System.Windows.Forms.TextBox();
            this.miniToolStrip = new System.Windows.Forms.ToolStrip();
            this.tlpProjectManager = new System.Windows.Forms.TableLayoutPanel();
            this.tsGroups = new System.Windows.Forms.ToolStrip();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.epUser)).BeginInit();
            this.gbProjectUrlSettings.SuspendLayout();
            this.tlpUrlsAndFilters.SuspendLayout();
            this.tlpEntityFields.SuspendLayout();
            this.gbEntityField4.SuspendLayout();
            this.flpEntityField4.SuspendLayout();
            this.gbEntityField5.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.gbEntityField1.SuspendLayout();
            this.flptEntityField1.SuspendLayout();
            this.gbEntityField2.SuspendLayout();
            this.flpEntityField2.SuspendLayout();
            this.gnEntityField3.SuspendLayout();
            this.flpEntityField3.SuspendLayout();
            this.gbListSeparator.SuspendLayout();
            this.gbProjectDetails.SuspendLayout();
            this.tlpProjectDetails.SuspendLayout();
            this.tlpProjectManager.SuspendLayout();
            this.SuspendLayout();
            // 
            // epUser
            // 
            this.epUser.ContainerControl = this;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Image = global::Arya.Properties.Resources.Save;
            this.btnSave.Location = new System.Drawing.Point(541, 520);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(114, 37);
            this.btnSave.TabIndex = 18;
            this.btnSave.Text = " Save Changes";
            this.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // gbProjectUrlSettings
            // 
            this.gbProjectUrlSettings.Controls.Add(this.tlpUrlsAndFilters);
            this.gbProjectUrlSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbProjectUrlSettings.Location = new System.Drawing.Point(3, 272);
            this.gbProjectUrlSettings.Name = "gbProjectUrlSettings";
            this.gbProjectUrlSettings.Size = new System.Drawing.Size(652, 242);
            this.gbProjectUrlSettings.TabIndex = 5;
            this.gbProjectUrlSettings.TabStop = false;
            this.gbProjectUrlSettings.Text = "Urls && Filters";
            // 
            // tlpUrlsAndFilters
            // 
            this.tlpUrlsAndFilters.ColumnCount = 2;
            this.tlpUrlsAndFilters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tlpUrlsAndFilters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpUrlsAndFilters.Controls.Add(this.lblProductSearchString, 0, 0);
            this.tlpUrlsAndFilters.Controls.Add(this.tbProductSearchString, 1, 0);
            this.tlpUrlsAndFilters.Controls.Add(this.lblFilters, 0, 1);
            this.tlpUrlsAndFilters.Controls.Add(this.tbSchemaFillRateFilters, 1, 1);
            this.tlpUrlsAndFilters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpUrlsAndFilters.Location = new System.Drawing.Point(3, 16);
            this.tlpUrlsAndFilters.Name = "tlpUrlsAndFilters";
            this.tlpUrlsAndFilters.RowCount = 2;
            this.tlpUrlsAndFilters.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlpUrlsAndFilters.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlpUrlsAndFilters.Size = new System.Drawing.Size(646, 223);
            this.tlpUrlsAndFilters.TabIndex = 0;
            // 
            // lblProductSearchString
            // 
            this.lblProductSearchString.AutoSize = true;
            this.lblProductSearchString.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblProductSearchString.Location = new System.Drawing.Point(3, 0);
            this.lblProductSearchString.Name = "lblProductSearchString";
            this.lblProductSearchString.Size = new System.Drawing.Size(94, 25);
            this.lblProductSearchString.TabIndex = 0;
            this.lblProductSearchString.Text = "Product URL";
            this.lblProductSearchString.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbProductSearchString
            // 
            this.tbProductSearchString.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbProductSearchString.Location = new System.Drawing.Point(103, 3);
            this.tbProductSearchString.Name = "tbProductSearchString";
            this.tbProductSearchString.Size = new System.Drawing.Size(540, 20);
            this.tbProductSearchString.TabIndex = 16;
            // 
            // lblFilters
            // 
            this.lblFilters.AutoSize = true;
            this.lblFilters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFilters.Location = new System.Drawing.Point(3, 25);
            this.lblFilters.Name = "lblFilters";
            this.lblFilters.Size = new System.Drawing.Size(94, 198);
            this.lblFilters.TabIndex = 2;
            this.lblFilters.Text = "FillRate Filters";
            this.lblFilters.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // tbSchemaFillRateFilters
            // 
            this.tbSchemaFillRateFilters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbSchemaFillRateFilters.Location = new System.Drawing.Point(103, 28);
            this.tbSchemaFillRateFilters.Multiline = true;
            this.tbSchemaFillRateFilters.Name = "tbSchemaFillRateFilters";
            this.tbSchemaFillRateFilters.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbSchemaFillRateFilters.Size = new System.Drawing.Size(540, 192);
            this.tbSchemaFillRateFilters.TabIndex = 17;
            // 
            // tlpEntityFields
            // 
            this.tlpEntityFields.ColumnCount = 3;
            this.tlpEntityFields.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33332F));
            this.tlpEntityFields.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tlpEntityFields.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tlpEntityFields.Controls.Add(this.gbEntityField4, 0, 1);
            this.tlpEntityFields.Controls.Add(this.gbEntityField5, 0, 1);
            this.tlpEntityFields.Controls.Add(this.gbEntityField1, 0, 0);
            this.tlpEntityFields.Controls.Add(this.gbEntityField2, 1, 0);
            this.tlpEntityFields.Controls.Add(this.gnEntityField3, 2, 0);
            this.tlpEntityFields.Controls.Add(this.gbListSeparator, 2, 1);
            this.tlpEntityFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpEntityFields.Location = new System.Drawing.Point(3, 79);
            this.tlpEntityFields.Name = "tlpEntityFields";
            this.tlpEntityFields.RowCount = 2;
            this.tlpEntityFields.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpEntityFields.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpEntityFields.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpEntityFields.Size = new System.Drawing.Size(652, 187);
            this.tlpEntityFields.TabIndex = 4;
            // 
            // gbEntityField4
            // 
            this.gbEntityField4.Controls.Add(this.flpEntityField4);
            this.gbEntityField4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbEntityField4.Location = new System.Drawing.Point(220, 96);
            this.gbEntityField4.Name = "gbEntityField4";
            this.gbEntityField4.Size = new System.Drawing.Size(211, 88);
            this.gbEntityField4.TabIndex = 4;
            this.gbEntityField4.TabStop = false;
            this.gbEntityField4.Text = "Entity Field4";
            // 
            // flpEntityField4
            // 
            this.flpEntityField4.Controls.Add(this.tbEntityField4Name);
            this.flpEntityField4.Controls.Add(this.cbEntityField4ReadOnly);
            this.flpEntityField4.Location = new System.Drawing.Point(3, 16);
            this.flpEntityField4.Name = "flpEntityField4";
            this.flpEntityField4.Padding = new System.Windows.Forms.Padding(5);
            this.flpEntityField4.Size = new System.Drawing.Size(205, 61);
            this.flpEntityField4.TabIndex = 0;
            // 
            // tbEntityField4Name
            // 
            this.tbEntityField4Name.Location = new System.Drawing.Point(8, 8);
            this.tbEntityField4Name.Name = "tbEntityField4Name";
            this.tbEntityField4Name.Size = new System.Drawing.Size(188, 20);
            this.tbEntityField4Name.TabIndex = 11;
            // 
            // cbEntityField4ReadOnly
            // 
            this.cbEntityField4ReadOnly.AutoSize = true;
            this.cbEntityField4ReadOnly.Location = new System.Drawing.Point(8, 34);
            this.cbEntityField4ReadOnly.Name = "cbEntityField4ReadOnly";
            this.cbEntityField4ReadOnly.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cbEntityField4ReadOnly.Size = new System.Drawing.Size(84, 17);
            this.cbEntityField4ReadOnly.TabIndex = 12;
            this.cbEntityField4ReadOnly.Text = "Is ReadOnly";
            this.cbEntityField4ReadOnly.UseVisualStyleBackColor = true;
            // 
            // gbEntityField5
            // 
            this.gbEntityField5.Controls.Add(this.tableLayoutPanel1);
            this.gbEntityField5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbEntityField5.Location = new System.Drawing.Point(3, 96);
            this.gbEntityField5.Name = "gbEntityField5";
            this.gbEntityField5.Size = new System.Drawing.Size(211, 88);
            this.gbEntityField5.TabIndex = 3;
            this.gbEntityField5.TabStop = false;
            this.gbEntityField5.Text = "Entity Field5";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.cbEntityField5ReadOnly, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.cbEntityField5IsStatus, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tbEntityField5Name, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 16);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(202, 61);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // cbEntityField5ReadOnly
            // 
            this.cbEntityField5ReadOnly.AutoSize = true;
            this.cbEntityField5ReadOnly.Location = new System.Drawing.Point(3, 34);
            this.cbEntityField5ReadOnly.Name = "cbEntityField5ReadOnly";
            this.cbEntityField5ReadOnly.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cbEntityField5ReadOnly.Size = new System.Drawing.Size(84, 17);
            this.cbEntityField5ReadOnly.TabIndex = 15;
            this.cbEntityField5ReadOnly.Text = "Is ReadOnly";
            this.cbEntityField5ReadOnly.UseVisualStyleBackColor = true;
            // 
            // cbEntityField5IsStatus
            // 
            this.cbEntityField5IsStatus.AutoSize = true;
            this.cbEntityField5IsStatus.Location = new System.Drawing.Point(104, 34);
            this.cbEntityField5IsStatus.Name = "cbEntityField5IsStatus";
            this.cbEntityField5IsStatus.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cbEntityField5IsStatus.Size = new System.Drawing.Size(67, 17);
            this.cbEntityField5IsStatus.TabIndex = 14;
            this.cbEntityField5IsStatus.Text = "Is Status";
            this.cbEntityField5IsStatus.UseVisualStyleBackColor = true;
            // 
            // tbEntityField5Name
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.tbEntityField5Name, 2);
            this.tbEntityField5Name.Location = new System.Drawing.Point(8, 8);
            this.tbEntityField5Name.Margin = new System.Windows.Forms.Padding(8, 8, 3, 8);
            this.tbEntityField5Name.Name = "tbEntityField5Name";
            this.tbEntityField5Name.Size = new System.Drawing.Size(185, 20);
            this.tbEntityField5Name.TabIndex = 13;
            // 
            // gbEntityField1
            // 
            this.gbEntityField1.Controls.Add(this.flptEntityField1);
            this.gbEntityField1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbEntityField1.Location = new System.Drawing.Point(3, 3);
            this.gbEntityField1.Name = "gbEntityField1";
            this.gbEntityField1.Size = new System.Drawing.Size(211, 87);
            this.gbEntityField1.TabIndex = 0;
            this.gbEntityField1.TabStop = false;
            this.gbEntityField1.Text = "Entity Field1";
            // 
            // flptEntityField1
            // 
            this.flptEntityField1.Controls.Add(this.tbEntityField1Name);
            this.flptEntityField1.Controls.Add(this.cbEntityField1ReadOnly);
            this.flptEntityField1.Location = new System.Drawing.Point(3, 16);
            this.flptEntityField1.Name = "flptEntityField1";
            this.flptEntityField1.Padding = new System.Windows.Forms.Padding(5);
            this.flptEntityField1.Size = new System.Drawing.Size(205, 61);
            this.flptEntityField1.TabIndex = 0;
            // 
            // tbEntityField1Name
            // 
            this.tbEntityField1Name.Location = new System.Drawing.Point(8, 8);
            this.tbEntityField1Name.Name = "tbEntityField1Name";
            this.tbEntityField1Name.Size = new System.Drawing.Size(188, 20);
            this.tbEntityField1Name.TabIndex = 5;
            // 
            // cbEntityField1ReadOnly
            // 
            this.cbEntityField1ReadOnly.AutoSize = true;
            this.cbEntityField1ReadOnly.Location = new System.Drawing.Point(8, 34);
            this.cbEntityField1ReadOnly.Name = "cbEntityField1ReadOnly";
            this.cbEntityField1ReadOnly.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cbEntityField1ReadOnly.Size = new System.Drawing.Size(84, 17);
            this.cbEntityField1ReadOnly.TabIndex = 6;
            this.cbEntityField1ReadOnly.Text = "Is ReadOnly";
            this.cbEntityField1ReadOnly.UseVisualStyleBackColor = true;
            // 
            // gbEntityField2
            // 
            this.gbEntityField2.Controls.Add(this.flpEntityField2);
            this.gbEntityField2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbEntityField2.Location = new System.Drawing.Point(220, 3);
            this.gbEntityField2.Name = "gbEntityField2";
            this.gbEntityField2.Size = new System.Drawing.Size(211, 87);
            this.gbEntityField2.TabIndex = 1;
            this.gbEntityField2.TabStop = false;
            this.gbEntityField2.Text = "Entity Field2";
            // 
            // flpEntityField2
            // 
            this.flpEntityField2.Controls.Add(this.tbEntityField2Name);
            this.flpEntityField2.Controls.Add(this.cbEntityField2ReadOnly);
            this.flpEntityField2.Location = new System.Drawing.Point(3, 16);
            this.flpEntityField2.Name = "flpEntityField2";
            this.flpEntityField2.Padding = new System.Windows.Forms.Padding(5);
            this.flpEntityField2.Size = new System.Drawing.Size(205, 61);
            this.flpEntityField2.TabIndex = 0;
            // 
            // tbEntityField2Name
            // 
            this.tbEntityField2Name.Location = new System.Drawing.Point(8, 8);
            this.tbEntityField2Name.Name = "tbEntityField2Name";
            this.tbEntityField2Name.Size = new System.Drawing.Size(188, 20);
            this.tbEntityField2Name.TabIndex = 7;
            // 
            // cbEntityField2ReadOnly
            // 
            this.cbEntityField2ReadOnly.AutoSize = true;
            this.cbEntityField2ReadOnly.Location = new System.Drawing.Point(8, 34);
            this.cbEntityField2ReadOnly.Name = "cbEntityField2ReadOnly";
            this.cbEntityField2ReadOnly.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cbEntityField2ReadOnly.Size = new System.Drawing.Size(84, 17);
            this.cbEntityField2ReadOnly.TabIndex = 8;
            this.cbEntityField2ReadOnly.Text = "Is ReadOnly";
            this.cbEntityField2ReadOnly.UseVisualStyleBackColor = true;
            // 
            // gnEntityField3
            // 
            this.gnEntityField3.Controls.Add(this.flpEntityField3);
            this.gnEntityField3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gnEntityField3.Location = new System.Drawing.Point(437, 3);
            this.gnEntityField3.Name = "gnEntityField3";
            this.gnEntityField3.Size = new System.Drawing.Size(212, 87);
            this.gnEntityField3.TabIndex = 2;
            this.gnEntityField3.TabStop = false;
            this.gnEntityField3.Text = "Entity Field3";
            // 
            // flpEntityField3
            // 
            this.flpEntityField3.Controls.Add(this.tbEntityField3Name);
            this.flpEntityField3.Controls.Add(this.cbEntityField3ReadOnly);
            this.flpEntityField3.Location = new System.Drawing.Point(3, 16);
            this.flpEntityField3.Name = "flpEntityField3";
            this.flpEntityField3.Padding = new System.Windows.Forms.Padding(5);
            this.flpEntityField3.Size = new System.Drawing.Size(205, 61);
            this.flpEntityField3.TabIndex = 0;
            // 
            // tbEntityField3Name
            // 
            this.tbEntityField3Name.Location = new System.Drawing.Point(8, 8);
            this.tbEntityField3Name.Name = "tbEntityField3Name";
            this.tbEntityField3Name.Size = new System.Drawing.Size(188, 20);
            this.tbEntityField3Name.TabIndex = 9;
            // 
            // cbEntityField3ReadOnly
            // 
            this.cbEntityField3ReadOnly.AutoSize = true;
            this.cbEntityField3ReadOnly.Location = new System.Drawing.Point(8, 34);
            this.cbEntityField3ReadOnly.Name = "cbEntityField3ReadOnly";
            this.cbEntityField3ReadOnly.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cbEntityField3ReadOnly.Size = new System.Drawing.Size(84, 17);
            this.cbEntityField3ReadOnly.TabIndex = 10;
            this.cbEntityField3ReadOnly.Text = "Is ReadOnly";
            this.cbEntityField3ReadOnly.UseVisualStyleBackColor = true;
            // 
            // gbListSeparator
            // 
            this.gbListSeparator.Controls.Add(this.lblReturnSeparator);
            this.gbListSeparator.Controls.Add(this.tbReturnSeparator);
            this.gbListSeparator.Controls.Add(this.lblListSeparator);
            this.gbListSeparator.Controls.Add(this.tbListSeparator);
            this.gbListSeparator.Location = new System.Drawing.Point(437, 96);
            this.gbListSeparator.Name = "gbListSeparator";
            this.gbListSeparator.Size = new System.Drawing.Size(212, 88);
            this.gbListSeparator.TabIndex = 5;
            this.gbListSeparator.TabStop = false;
            this.gbListSeparator.Text = "Separators";
            // 
            // lblReturnSeparator
            // 
            this.lblReturnSeparator.AutoSize = true;
            this.lblReturnSeparator.Location = new System.Drawing.Point(8, 53);
            this.lblReturnSeparator.Name = "lblReturnSeparator";
            this.lblReturnSeparator.Size = new System.Drawing.Size(39, 13);
            this.lblReturnSeparator.TabIndex = 3;
            this.lblReturnSeparator.Text = "Return";
            // 
            // tbReturnSeparator
            // 
            this.tbReturnSeparator.Location = new System.Drawing.Point(69, 50);
            this.tbReturnSeparator.Name = "tbReturnSeparator";
            this.tbReturnSeparator.Size = new System.Drawing.Size(119, 20);
            this.tbReturnSeparator.TabIndex = 2;
            this.tbReturnSeparator.TextChanged += new System.EventHandler(this.tbReturnSeparator_TextChanged);
            // 
            // lblListSeparator
            // 
            this.lblListSeparator.AutoSize = true;
            this.lblListSeparator.Location = new System.Drawing.Point(8, 27);
            this.lblListSeparator.Name = "lblListSeparator";
            this.lblListSeparator.Size = new System.Drawing.Size(23, 13);
            this.lblListSeparator.TabIndex = 1;
            this.lblListSeparator.Text = "List";
            // 
            // tbListSeparator
            // 
            this.tbListSeparator.Location = new System.Drawing.Point(69, 24);
            this.tbListSeparator.Name = "tbListSeparator";
            this.tbListSeparator.Size = new System.Drawing.Size(119, 20);
            this.tbListSeparator.TabIndex = 0;
            this.tbListSeparator.TextChanged += new System.EventHandler(this.tbListSeparator_TextChanged);
            // 
            // gbProjectDetails
            // 
            this.gbProjectDetails.Controls.Add(this.tlpProjectDetails);
            this.gbProjectDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbProjectDetails.Location = new System.Drawing.Point(3, 3);
            this.gbProjectDetails.Name = "gbProjectDetails";
            this.gbProjectDetails.Size = new System.Drawing.Size(652, 70);
            this.gbProjectDetails.TabIndex = 3;
            this.gbProjectDetails.TabStop = false;
            this.gbProjectDetails.Text = "Project Details";
            // 
            // tlpProjectDetails
            // 
            this.tlpProjectDetails.ColumnCount = 5;
            this.tlpProjectDetails.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tlpProjectDetails.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpProjectDetails.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 97F));
            this.tlpProjectDetails.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpProjectDetails.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 41F));
            this.tlpProjectDetails.Controls.Add(this.lblDatabaseName, 0, 1);
            this.tlpProjectDetails.Controls.Add(this.lblClientDescription, 2, 1);
            this.tlpProjectDetails.Controls.Add(this.tbDatabaseName, 1, 1);
            this.tlpProjectDetails.Controls.Add(this.tbClientDescription, 3, 1);
            this.tlpProjectDetails.Controls.Add(this.lblProjectName, 0, 0);
            this.tlpProjectDetails.Controls.Add(this.tbProjectName, 1, 0);
            this.tlpProjectDetails.Controls.Add(this.lblSetName, 2, 0);
            this.tlpProjectDetails.Controls.Add(this.tbSetName, 3, 0);
            this.tlpProjectDetails.Controls.Add(this.tbAryaCodeBaseVersion, 4, 1);
            this.tlpProjectDetails.Controls.Add(this.label1, 4, 0);
            this.tlpProjectDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpProjectDetails.Location = new System.Drawing.Point(3, 16);
            this.tlpProjectDetails.Name = "tlpProjectDetails";
            this.tlpProjectDetails.RowCount = 2;
            this.tlpProjectDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlpProjectDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlpProjectDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpProjectDetails.Size = new System.Drawing.Size(646, 51);
            this.tlpProjectDetails.TabIndex = 0;
            // 
            // lblDatabaseName
            // 
            this.lblDatabaseName.AutoSize = true;
            this.lblDatabaseName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDatabaseName.Location = new System.Drawing.Point(3, 25);
            this.lblDatabaseName.Name = "lblDatabaseName";
            this.lblDatabaseName.Size = new System.Drawing.Size(94, 26);
            this.lblDatabaseName.TabIndex = 1;
            this.lblDatabaseName.Text = "Database Name";
            this.lblDatabaseName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblClientDescription
            // 
            this.lblClientDescription.AutoSize = true;
            this.lblClientDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblClientDescription.Location = new System.Drawing.Point(307, 25);
            this.lblClientDescription.Name = "lblClientDescription";
            this.lblClientDescription.Size = new System.Drawing.Size(91, 26);
            this.lblClientDescription.TabIndex = 3;
            this.lblClientDescription.Text = "Client Description";
            this.lblClientDescription.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbDatabaseName
            // 
            this.tbDatabaseName.Location = new System.Drawing.Point(103, 28);
            this.tbDatabaseName.Name = "tbDatabaseName";
            this.tbDatabaseName.Size = new System.Drawing.Size(196, 20);
            this.tbDatabaseName.TabIndex = 3;
            this.tbDatabaseName.Tag = "";
            // 
            // tbClientDescription
            // 
            this.tbClientDescription.Location = new System.Drawing.Point(404, 28);
            this.tbClientDescription.Name = "tbClientDescription";
            this.tbClientDescription.Size = new System.Drawing.Size(196, 20);
            this.tbClientDescription.TabIndex = 4;
            this.tbClientDescription.Tag = "";
            // 
            // lblProjectName
            // 
            this.lblProjectName.AutoSize = true;
            this.lblProjectName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblProjectName.Location = new System.Drawing.Point(3, 0);
            this.lblProjectName.Name = "lblProjectName";
            this.lblProjectName.Size = new System.Drawing.Size(94, 25);
            this.lblProjectName.TabIndex = 2;
            this.lblProjectName.Text = "Project Name";
            this.lblProjectName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbProjectName
            // 
            this.tbProjectName.Location = new System.Drawing.Point(103, 3);
            this.tbProjectName.Name = "tbProjectName";
            this.tbProjectName.Size = new System.Drawing.Size(196, 20);
            this.tbProjectName.TabIndex = 1;
            this.tbProjectName.Tag = "";
            // 
            // lblSetName
            // 
            this.lblSetName.AutoSize = true;
            this.lblSetName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSetName.Location = new System.Drawing.Point(307, 0);
            this.lblSetName.Name = "lblSetName";
            this.lblSetName.Size = new System.Drawing.Size(91, 25);
            this.lblSetName.TabIndex = 8;
            this.lblSetName.Text = "Set Name";
            this.lblSetName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbSetName
            // 
            this.tbSetName.Location = new System.Drawing.Point(404, 3);
            this.tbSetName.Name = "tbSetName";
            this.tbSetName.Size = new System.Drawing.Size(196, 20);
            this.tbSetName.TabIndex = 2;
            // 
            // tbAryaCodeBaseVersion
            // 
            this.tbAryaCodeBaseVersion.Location = new System.Drawing.Point(608, 28);
            this.tbAryaCodeBaseVersion.Name = "tbAryaCodeBaseVersion";
            this.tbAryaCodeBaseVersion.Size = new System.Drawing.Size(35, 20);
            this.tbAryaCodeBaseVersion.TabIndex = 9;
            // 
            // miniToolStrip
            // 
            this.miniToolStrip.AutoSize = false;
            this.miniToolStrip.CanOverflow = false;
            this.miniToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.miniToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.miniToolStrip.Location = new System.Drawing.Point(0, 3);
            this.miniToolStrip.Name = "miniToolStrip";
            this.miniToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.miniToolStrip.Size = new System.Drawing.Size(102, 25);
            this.miniToolStrip.TabIndex = 2;
            // 
            // tlpProjectManager
            // 
            this.tlpProjectManager.ColumnCount = 1;
            this.tlpProjectManager.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpProjectManager.Controls.Add(this.gbProjectDetails, 0, 0);
            this.tlpProjectManager.Controls.Add(this.tlpEntityFields, 0, 1);
            this.tlpProjectManager.Controls.Add(this.gbProjectUrlSettings, 0, 2);
            this.tlpProjectManager.Controls.Add(this.btnSave, 0, 3);
            this.tlpProjectManager.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpProjectManager.Location = new System.Drawing.Point(0, 0);
            this.tlpProjectManager.Name = "tlpProjectManager";
            this.tlpProjectManager.RowCount = 4;
            this.tlpProjectManager.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 76F));
            this.tlpProjectManager.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 193F));
            this.tlpProjectManager.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 248F));
            this.tlpProjectManager.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tlpProjectManager.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpProjectManager.Size = new System.Drawing.Size(658, 561);
            this.tlpProjectManager.TabIndex = 0;
            // 
            // tsGroups
            // 
            this.tsGroups.Dock = System.Windows.Forms.DockStyle.None;
            this.tsGroups.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsGroups.Location = new System.Drawing.Point(0, 0);
            this.tsGroups.Name = "tsGroups";
            this.tsGroups.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.tsGroups.Size = new System.Drawing.Size(102, 25);
            this.tsGroups.TabIndex = 2;
            this.tsGroups.Text = "toolStrip1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(608, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 25);
            this.label1.TabIndex = 10;
            this.label1.Text = "Ver:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FrmProjectManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(658, 561);
            this.Controls.Add(this.tlpProjectManager);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmProjectManager";
            this.Text = "Project Manager";
            this.Load += new System.EventHandler(this.FrmProjectManager_Load);
            ((System.ComponentModel.ISupportInitialize)(this.epUser)).EndInit();
            this.gbProjectUrlSettings.ResumeLayout(false);
            this.tlpUrlsAndFilters.ResumeLayout(false);
            this.tlpUrlsAndFilters.PerformLayout();
            this.tlpEntityFields.ResumeLayout(false);
            this.gbEntityField4.ResumeLayout(false);
            this.flpEntityField4.ResumeLayout(false);
            this.flpEntityField4.PerformLayout();
            this.gbEntityField5.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.gbEntityField1.ResumeLayout(false);
            this.flptEntityField1.ResumeLayout(false);
            this.flptEntityField1.PerformLayout();
            this.gbEntityField2.ResumeLayout(false);
            this.flpEntityField2.ResumeLayout(false);
            this.flpEntityField2.PerformLayout();
            this.gnEntityField3.ResumeLayout(false);
            this.flpEntityField3.ResumeLayout(false);
            this.flpEntityField3.PerformLayout();
            this.gbListSeparator.ResumeLayout(false);
            this.gbListSeparator.PerformLayout();
            this.gbProjectDetails.ResumeLayout(false);
            this.tlpProjectDetails.ResumeLayout(false);
            this.tlpProjectDetails.PerformLayout();
            this.tlpProjectManager.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ErrorProvider epUser;
        private System.Windows.Forms.TableLayoutPanel tlpProjectManager;
        private System.Windows.Forms.GroupBox gbProjectDetails;
        private System.Windows.Forms.TableLayoutPanel tlpProjectDetails;
        private System.Windows.Forms.Label lblDatabaseName;
        private System.Windows.Forms.Label lblClientDescription;
        private System.Windows.Forms.Label lblProjectName;
        private System.Windows.Forms.TextBox tbDatabaseName;
        private System.Windows.Forms.TextBox tbClientDescription;
        private System.Windows.Forms.TextBox tbProjectName;
        private System.Windows.Forms.TableLayoutPanel tlpEntityFields;
        private System.Windows.Forms.GroupBox gbEntityField4;
        private System.Windows.Forms.FlowLayoutPanel flpEntityField4;
        private System.Windows.Forms.TextBox tbEntityField4Name;
        private System.Windows.Forms.CheckBox cbEntityField4ReadOnly;
        private System.Windows.Forms.GroupBox gbEntityField5;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckBox cbEntityField5ReadOnly;
        private System.Windows.Forms.CheckBox cbEntityField5IsStatus;
        private System.Windows.Forms.TextBox tbEntityField5Name;
        private System.Windows.Forms.GroupBox gbEntityField1;
        private System.Windows.Forms.FlowLayoutPanel flptEntityField1;
        private System.Windows.Forms.TextBox tbEntityField1Name;
        private System.Windows.Forms.CheckBox cbEntityField1ReadOnly;
        private System.Windows.Forms.GroupBox gbEntityField2;
        private System.Windows.Forms.FlowLayoutPanel flpEntityField2;
        private System.Windows.Forms.TextBox tbEntityField2Name;
        private System.Windows.Forms.CheckBox cbEntityField2ReadOnly;
        private System.Windows.Forms.GroupBox gnEntityField3;
        private System.Windows.Forms.FlowLayoutPanel flpEntityField3;
        private System.Windows.Forms.TextBox tbEntityField3Name;
        private System.Windows.Forms.CheckBox cbEntityField3ReadOnly;
        private System.Windows.Forms.GroupBox gbProjectUrlSettings;
        private System.Windows.Forms.TableLayoutPanel tlpUrlsAndFilters;
        private System.Windows.Forms.Label lblProductSearchString;
        private System.Windows.Forms.TextBox tbProductSearchString;
        private System.Windows.Forms.Label lblFilters;
        private System.Windows.Forms.TextBox tbSchemaFillRateFilters;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ToolStrip miniToolStrip;
        private System.Windows.Forms.ToolStrip tsGroups;
        private System.Windows.Forms.Label lblSetName;
        private System.Windows.Forms.TextBox tbSetName;
        private System.Windows.Forms.GroupBox gbListSeparator;
        private System.Windows.Forms.TextBox tbListSeparator;
        private System.Windows.Forms.Label lblReturnSeparator;
        private System.Windows.Forms.TextBox tbReturnSeparator;
        private System.Windows.Forms.Label lblListSeparator;
        private System.Windows.Forms.TextBox tbAryaCodeBaseVersion;
        private System.Windows.Forms.Label label1;

    }
}