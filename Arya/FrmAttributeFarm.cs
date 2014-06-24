using System.Windows.Forms;
using Arya.HelperClasses;
using Arya.Properties;

namespace Arya
{
	public partial class FrmAttributeFarm : Form
    {

		#region Constructors (1) 

		public FrmAttributeFarm()
		{
			InitializeComponent(); 
			Icon = Resources.AryaLogoIcon;
		}

		#endregion Constructors 

        private void FrmAttributeFarm_Load(object sender, System.EventArgs e)
        {
            attributeFarmGridView1.PrepareData();
        }

        private void FrmAttributeFarm_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Control)
                return;

            switch (e.KeyCode)
            {
                case Keys.S:
                    AryaTools.Instance.SaveChangesIfNecessary(false, true);
                    break;
            }
        }

	}
}
