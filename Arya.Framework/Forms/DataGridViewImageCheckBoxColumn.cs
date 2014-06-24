using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Natalie.Framework.Forms
{
    [DataGridViewColumnDesignTimeVisible(true)]
    public class DataGridViewImageCheckBoxColumn : DataGridViewImageColumn
    {
        public DataGridViewImageCheckBoxColumn()
        {
            CellTemplate = new DataGridViewImageCheckBoxCell();
        }

        [Description("Image representing Checked State")]
        [Category("Appearance")]
        [EditorAttribute(typeof(System.Drawing.Design.ImageEditor), typeof(System.Drawing.Design.BitmapEditor))]
        [DefaultValueAttribute(typeof(Image), "null"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Image CheckedImage { get; set; }

        public override object Clone()
        {
            var cloneObj = base.Clone() as DataGridViewImageCheckBoxColumn;
            if (cloneObj != null)
            {
                cloneObj.CheckedImage = CheckedImage;
                cloneObj.UnCheckedImage = UnCheckedImage;
            }
            return cloneObj;
        }

        [Description("Image representing UnChecked State")]
        [Category("Appearance")]
        [EditorAttribute(typeof(System.Drawing.Design.ImageEditor), typeof(System.Drawing.Design.BitmapEditor))]
        [DefaultValueAttribute(typeof(Image), "null"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Image UnCheckedImage { get; set; }

        public override sealed DataGridViewCell CellTemplate
        {
            get { return base.CellTemplate; }
            set { base.CellTemplate = value; }
        }
    }
}
