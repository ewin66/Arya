using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using Natalie.Framework.Collections;
using Natalie.Framework.ComponentModel;
using Natalie.Framework.Extensions;
using Natalie.Framework.IO;
using Natalie.HelperClasses;

namespace Natalie.Framework.UI.TypeEditors
{
    public class TaxonomyEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var selectedTaxonomies = (TaxonomyCollection)value;
            var existingIds = selectedTaxonomies.Cast<ExtendedTaxonomyInfo>().Select(p => p.Taxonomy.ID).ToHashSet();
            NatalieTools.Instance.TreeForm.GetTaxonomy("Select this node", false);
            NatalieTools.Instance.TreeForm.TaxonomySelected += (s, ea) =>
            {
                if (NatalieTools.Instance.TreeForm.DialogResult !=
                    DialogResult.OK)
                    return;

                NatalieTools.Instance.TreeForm.taxonomyTree.SelectedTaxonomies.Select(p => new ExtendedTaxonomyInfo(p))
                    .ToList().ForEach(p =>
                                          {
                                              if(!existingIds.Contains(p.Taxonomy.ID)) selectedTaxonomies.Add(p);
                                          });
            };

            //NatalieTools.Instance.TreeForm.Show();

            return selectedTaxonomies;
        }
    }
}
