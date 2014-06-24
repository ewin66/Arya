namespace Arya.Framework.GUI.UserControls
{
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    [DataGridViewColumnDesignTimeVisible(true)]
    public class DataGridViewImageCheckBoxColumn : DataGridViewImageColumn
    {
        public DataGridViewImageCheckBoxColumn()
        {
            this.CellTemplate = new DataGridViewImageCheckBoxCell();
        }

        [Description("Image representing Checked State")]
        [Category("Appearance")]
        [Editor(typeof(System.Drawing.Design.ImageEditor), typeof(System.Drawing.Design.BitmapEditor))]
        [DefaultValue(typeof(Image), "null"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Image CheckedImage { get; set; }

        public override object Clone()
        {
            var cloneObj = base.Clone() as DataGridViewImageCheckBoxColumn;
            if (cloneObj != null)
            {
                cloneObj.CheckedImage = this.CheckedImage;
                cloneObj.UnCheckedImage = this.UnCheckedImage;
            }
            return cloneObj;
        }

        [Description("Image representing UnChecked State")]
        [Category("Appearance")]
        [Editor(typeof(System.Drawing.Design.ImageEditor), typeof(System.Drawing.Design.BitmapEditor))]
        [DefaultValue(typeof(Image), "null"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Image UnCheckedImage { get; set; }

        public override sealed DataGridViewCell CellTemplate
        {
            get { return base.CellTemplate; }
            set { base.CellTemplate = value; }
        }
    }
}
