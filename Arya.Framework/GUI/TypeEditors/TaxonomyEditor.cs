using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace Arya.Framework.GUI.TypeEditors
{
    public class TaxonomyEditor : UITypeEditor
    {
        private string _taxonomyActionObjectName;

        /// <summary>
        /// Constructor for the TaxonomyEditor Attribute.
        /// </summary>
        /// <param name="taxonomyActionObjectName">Provide the Taxonomy action object name for the EditValue to function properly</param>
        public TaxonomyEditor(string taxonomyActionObjectName) { _taxonomyActionObjectName = taxonomyActionObjectName; }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) { return UITypeEditorEditStyle.Modal; }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            //var selectedTaxonomies = (TaxonomyCollection)value;
            //var existingIds = selectedTaxonomies.Cast<ExtendedTaxonomyInfo>().Select(p => p.Taxonomy.ID).ToHashSet();
            //AryaTools.Instance.TreeForm.GetTaxonomy("Select this node", false);
            //AryaTools.Instance.TreeForm.TaxonomySelected += (s, ea) =>
            //{
            //    AryaTools.Instance.TreeForm.Hide();
            //    if (AryaTools.Instance.TreeForm.DialogResult !=
            //        DialogResult.OK)
            //        return;

            //    AryaTools.Instance.TreeForm.taxonomyTree.SelectedTaxonomies.Select(p => new ExtendedTaxonomyInfo(p))
            //        .ToList().ForEach(p =>
            //                              {
            //                                  if(!existingIds.Contains(p.Taxonomy.ID)) selectedTaxonomies.Add(p);
            //                              });
            //};

            //AryaTools.Instance.TreeForm.Show();

            //return selectedTaxonomies;

            throw new NotImplementedException();
        }
    }
}