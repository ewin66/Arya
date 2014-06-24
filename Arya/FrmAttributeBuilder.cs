namespace Arya
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;
    using Data;
    using HelperClasses;
    using HelperForms;
    using LinqKit;

    public sealed partial class FrmAttributeBuilder : Form
    {
		#region Fields (10) 

        private bool _abortWorker;
        private string _attributeExpression;
        private readonly List<string> _attributeNames = new List<string>
                                                            {
                                                                "Special: New Taxonomy",
                                                                "Special: Node Name",
                                                                "Special: Old Taxonomy",
                                                                "Other Navigation Attributes",
                                                                "Other Display Attributes",
                                                                "Other InSchema Attributes",
                                                                "Special: Uom",
                                                                "..."
                                                            };
        private readonly Dictionary<string, string> _constructs = new Dictionary<string, string>
                                                                      {
                                                                          {"\"<1>\"", "Constant"},
                                                                          {"{0} + <1>", "Concatenate"},
                                                                          {"({0})", "Group"},
                                                                          {"(<1> ? <2> : <3>)", "If-then-else"},
                                                                          {"({0} ?? <1>)", "If blank"},
                                                                          {"(<1> == <2>)", "Equal to"},
                                                                          {
                                                                              "(<1>, <2>, [...] : \", \" + [Att Name] + \": \" + [Att Value])"
                                                                              , "Format attributes"
                                                                          },
                                                                          {"{{0}}", "Mathematical Expression"}
                                                                      };
        private object _sampleData;
        private readonly IEnumerable<Sku> _sampleSkus;
        private readonly TaxonomyInfo _sourceTaxonomy;
        private string _workerExpression = string.Empty;
        private Thread _workerThread;
        private const string Accept = "Accept";

		#endregion Fields 

		#region Constructors (2) 

        public FrmAttributeBuilder(string expression, int maxLength, IEnumerable<string> attributes,
                                   IEnumerable<Sku> sampleSkus, TaxonomyInfo sourceTaxonomy, bool isInherited,
                                   string derivedAttributeName = null) : this()
        {
            _sampleSkus = sampleSkus;
            AttributeExpression = expression;
            MaxLength = maxLength;
            numUDExpressionLength.Value = MaxLength;
            attributes.OrderBy(att => att).ForEach(_attributeNames.Add);
            _sourceTaxonomy = sourceTaxonomy;

            var descriptionText = string.Empty;

            if (_sourceTaxonomy != null)
                descriptionText = "Source Taxonomy:" + Environment.NewLine + sourceTaxonomy;

            if (!string.IsNullOrEmpty(expression))
            {
                descriptionText += Enumerable.Repeat(Environment.NewLine, 2).Aggregate((a, b) => a + b) + "Expression:" +
                                   Environment.NewLine + expression + Environment.NewLine;
            }

            if (string.IsNullOrWhiteSpace(descriptionText) || string.IsNullOrWhiteSpace(expression))
            {
                lblExpressionDescription.Visible = false;
                btnAccept.Text = Accept;
                dgvAttributes.Enabled = true;
                dgvConstructs.Enabled = true;
                numUDExpressionLength.Enabled = true;
            }
            else
            {
                lblExpressionDescription.Text = descriptionText;
                numUDExpressionLength.Enabled = false;
            }

            btnEditInherited.Visible = isInherited;

            if (!string.IsNullOrEmpty(derivedAttributeName))
                Text = "Attribute Builder : " + derivedAttributeName.Trim();
            else
                Text = "Attribute Builder";
        }

        private FrmAttributeBuilder()
        {
            InitializeComponent();
            DisplayStyle.SetDefaultFont(this);
            DialogResult = DialogResult.Cancel;
        }

		#endregion Constructors 

		#region Properties (3) 

        public string AttributeExpression
        {
            get { return _attributeExpression; }
            private set { _attributeExpression = string.IsNullOrWhiteSpace(value) ? null : value; }
        }

        public bool IsExpressionInherited { get; private set; }

        public int MaxLength { get; private set; }

		#endregion Properties 

		#region Methods (12) 

		// Private Methods (12) 

        private void btnAccept_Click(object sender, EventArgs e)
        {
            if (btnAccept.Text.Equals(Accept, StringComparison.OrdinalIgnoreCase))
            {
                //AttributeExpression = Regex.Replace(txtExpression.Text, "\\s+", " ");
                AttributeExpression = txtExpression.Text.Trim();
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                lblExpressionDescription.Visible = false;
                btnAccept.Text = Accept;
                dgvAttributes.Enabled = true;
                dgvConstructs.Enabled = true;
                numUDExpressionLength.Enabled = true;
                btnEditInherited.Visible = false;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnEditInherited_Click(object sender, EventArgs e)
        {
            lblExpressionDescription.Visible = false;
            btnAccept.Text = Accept;
            btnEditInherited.Visible = false;
            IsExpressionInherited = true;
            numUDExpressionLength.Enabled = true;
        }

        private void dgvAttributes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var start = txtExpression.SelectionStart;
            var mid = start + txtExpression.SelectionLength;
            var attributeName = dgvAttributes[0, e.RowIndex].Value.ToString();
            var newValue = string.Format("{0}[{1}]{2}", txtExpression.Text.Substring(0, start), attributeName,
                                         txtExpression.Text.Length > mid
                                             ? txtExpression.Text.Substring(mid, txtExpression.Text.Length - mid)
                                             : string.Empty);
            txtExpression.Text = newValue;
            txtExpression.Select(start, attributeName.Length + 2);
            txtExpression.Focus();
        }

        private void dgvConstructs_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var start = txtExpression.SelectionStart;
            var mid = start + txtExpression.SelectionLength;

            var construct = dgvConstructs[0, e.RowIndex].Value.ToString().Replace("{0}", txtExpression.SelectedText);

            var newValue = string.Format("{0} {1} {2}", txtExpression.Text.Substring(0, start).Trim(), construct,
                                         txtExpression.Text.Length > mid
                                             ? txtExpression.Text.Substring(mid, txtExpression.Text.Length - mid).Trim()
                                             : string.Empty);
            txtExpression.Text = newValue;

            if (!SelectNextParameter())
                txtExpression.Select(start, construct.Length + 2);
            txtExpression.Focus();
        }

        private void FrmAttributeBuilder_FormClosing(object sender, FormClosingEventArgs e)
        {
            _abortWorker = true;
            if (_workerThread == null || !_workerThread.IsAlive)
                return;

            var waitkey = FrmWaitScreen.ShowMessage("Please wait...");
            _workerThread.Join();
            FrmWaitScreen.HideMessage(waitkey);
        }

        private void FrmAttributeBuilder_Load(object sender, EventArgs e)
        {
            txtExpression.Text = AttributeExpression;

            dgvAttributes.Rows.Add(_attributeNames.Count);
            for (var i = 0; i < _attributeNames.Count; i++)
                dgvAttributes[0, i].Value = _attributeNames[i];

            dgvConstructs.Rows.Add(_constructs.Count);
            var constructCtr = 0;
            foreach (var construct in _constructs)
            {
                dgvConstructs[0, constructCtr].Value = construct.Key;
                dgvConstructs[1, constructCtr].Value = construct.Value;
                constructCtr++;
            }

            txtExpression.SelectAll();
            txtExpression.Focus();
        }

        private void GenerateSampleValues()
        {
            var expression = string.Empty;
            while (!_abortWorker)
            {
                if (expression.Equals(_workerExpression))
                {
                    Thread.Sleep(500);
                    continue;
                }

                expression = _workerExpression;
                var maxResultLength = MaxLength;
                _sampleData = (from sku in _sampleSkus
                               let dValue =
                                   new TaxAttValueCalculator(sku).ProcessCalculatedAttribute(expression, maxResultLength)
                               select new {ItemId = sku.ItemID, Value = dValue, dValue.Value.Length}).ToList();
            }
        }

        private void numUDExpressionLength_ValueChanged(object sender, EventArgs e)
        {
            MaxLength = (int) numUDExpressionLength.Value;
        }

        private bool SelectNextParameter()
        {
            for (var i = 1; i < 10; i++)
            {
                var parameter = string.Format("<{0}>", i);
                var index = txtExpression.Text.IndexOf(parameter, StringComparison.Ordinal);
                if (index == -1)
                    continue;

                txtExpression.Select(index, parameter.Length);
                return true;
            }
            return false;
        }

        private void tmrGenerateSamples_Tick(object sender, EventArgs e)
        {
            if (_sampleData != null &&
                (dgvSampleData.DataSource == null || !dgvSampleData.DataSource.Equals(_sampleData)))
                dgvSampleData.DataSource = _sampleData;

            if (_workerExpression.Equals(txtExpression.Text))
                return;

            _workerExpression = txtExpression.Text;
            if (_workerThread == null || !_workerThread.IsAlive)
            {
                _workerThread = new Thread(GenerateSampleValues) {IsBackground = true};
                _workerThread.Start();
            }
        }

        private void txtExpression_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab && SelectNextParameter())
                e.Handled = true;
        }

		#endregion Methods 
    }
}