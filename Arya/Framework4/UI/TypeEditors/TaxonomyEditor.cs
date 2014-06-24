using Arya.Framework.Common.Extensions;

namespace Arya.Framework4.UI.TypeEditors
{
    using System;
    using System.ComponentModel;
    using System.Drawing.Design;
    using System.Linq;
    using System.Windows.Forms;
    using Collections;
    using ComponentModel;
    using Framework.Extensions;
    using HelperClasses;

    public class TaxonomyEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var selectedTaxonomies = (TaxonomyCollection) value;
            var existingIds = selectedTaxonomies.Cast<ExtendedTaxonomyInfo>().Select(p => p.Taxonomy.ID).ToHashSet();
            AryaTools.Instance.Forms.TreeForm.GetTaxonomy("Select this node", false);
            AryaTools.Instance.Forms.TreeForm.TaxonomySelected += (s, ea) =>
                                                                   {
                                                                       if (
                                                                           AryaTools.Instance.Forms.TreeForm.DialogResult !=
                                                                           DialogResult.OK)
                                                                           return;

                                                                       AryaTools.Instance.Forms.TreeForm.taxonomyTree.
                                                                           SelectedTaxonomies.Select(
                                                                               p => new ExtendedTaxonomyInfo(p)).ToList()
                                                                           .ForEach(p =>
                                                                                        {
                                                                                            if (
                                                                                                !existingIds.Contains(
                                                                                                    p.Taxonomy.ID))
                                                                                                selectedTaxonomies.Add(p);
                                                                                        });
                                                                   };

            //AryaTools.Instance.Forms.TreeForm.Show();

            return selectedTaxonomies;
        }
    }
}