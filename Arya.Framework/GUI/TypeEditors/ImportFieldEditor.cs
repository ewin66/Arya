using System.ComponentModel;
using System.Drawing.Design;

namespace Arya.Framework.GUI.TypeEditors
{
    class ImportFieldEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        //public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        //{

        //}
    }
}
