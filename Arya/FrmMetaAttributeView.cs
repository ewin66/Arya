using System.Windows.Forms;
using Arya.UserControls;

namespace Arya
{
    public partial class FrmMetaAttributeView : Form
    {
        public FrmMetaAttributeView()
        {
            InitializeComponent();

            // initialize schema page
            cntSchema.MetaType = MetaTypeEnum.Schema;
            cntSchema.Init();

            // initialize taxonomy page
            cntTaxonomy.MetaType = MetaTypeEnum.Taxonomy;
            cntTaxonomy.Init();
        }
    }
}
