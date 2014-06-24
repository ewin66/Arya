using System.ComponentModel;
using System.Drawing.Design;

namespace Arya.Framework.GUI.TypeEditors
{
    public class EmptyEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.None;
        }
    }
}
