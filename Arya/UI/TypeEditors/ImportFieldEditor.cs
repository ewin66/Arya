using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using Fasterflect;

namespace Natalie.Framework.UI.TypeEditors
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
