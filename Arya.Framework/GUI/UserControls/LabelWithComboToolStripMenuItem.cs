using System.Drawing;
using System.Windows.Forms;

namespace Arya.CustomControls
{
    public class LabelWithComboToolStripMenuItem : ToolStripControlHost
    {
		#region Constructors (1) 

        public LabelWithComboToolStripMenuItem()
            : base(new FlowLayoutPanel {AutoSize = true, BackColor = Color.Transparent, Margin=new Padding{All=0}})
        {
            Label = new Label { Anchor = AnchorStyles.Left, AutoSize = true, Margin = new Padding { All = 0 } };
            ComboBox = new ComboBox { Anchor = AnchorStyles.Left, AutoSize = true, Margin = new Padding { All = 0 } };
            Panel.Controls.AddRange(new Control[] {Label, ComboBox});
            ComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            ComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
        }

		#endregion Constructors 

		#region Properties (6) 

        public ComboBox ComboBox { get; private set; }

        public ComboBox.ObjectCollection ComboItems
        {
            get { return ComboBox.Items; }
        }

        public Label Label { get; private set; }

        public string LabelText
        {
            get { return Label.Text; }
            set { Label.Text = value; }
        }

        private FlowLayoutPanel Panel
        {
            get { return Control as FlowLayoutPanel; }
        }

        public override string Text
        {
            get { return ComboBox.Text; }
            set { ComboBox.Text = value; }
        }

		#endregion Properties 
    }
}