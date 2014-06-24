namespace Arya.Framework.GUI.Forms
{
    using System.Windows.Forms;

    public partial class FilterTextEditor : Form
    {
        public FilterTextEditor()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterParent;
        }

        private string oldFilterText = string.Empty;

        public string FilterText
        {
            set
            {
                tbFilter.Text = value;
                oldFilterText = value;
            }
            get
            {
                return tbFilter.Text;
            }
        }

        public string[] FilterLines
        {
            get { return tbFilter.Lines; }
        }

        private void tbnOk_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            tbFilter.Text = oldFilterText;
            Close();
        }
    }
}
