using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Arya.Framework.Extensions;

namespace Arya.CustomControls
{
    public sealed partial class CheckedImageListBox : ListBox
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckedImageListBox"/> class.
        /// </summary>
        public CheckedImageListBox()
        {
            this.InitializeComponent();
            this.DrawMode = DrawMode.OwnerDrawVariable;
            this.checkedIndices = new HashSet<int>();
            this.textBrush = new SolidBrush(ForeColor);
        }

        /// <summary>
        /// The text brush.
        /// </summary>
        private SolidBrush textBrush;

        /// <summary>
        /// The checked indices.
        /// </summary>
        private HashSet<int> checkedIndices;

        /// <summary>
        /// The on fore color changed.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e);
            this.textBrush = new SolidBrush(ForeColor);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public HashSet<int> CheckedIndices
        {
            get
            {
                return checkedIndices;
            }
            set
            {
                checkedIndices = value;
                Invalidate();
            }
        }

        public List<object> CheckedItems
        {
            get
            {
                return checkedIndices.Select(checkedIndex => Items[checkedIndex]).ToList();
            }
        }

        [Description("Image representing Checked State")]
        [Category("Appearance")]
        [EditorAttribute(typeof(System.Drawing.Design.ImageEditor), typeof(System.Drawing.Design.BitmapEditor))]
        [DefaultValueAttribute(typeof(Image), "null"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Image CheckedImage { get; set; }

        [Description("Image representing UnChecked State")]
        [Category("Appearance")]
        [EditorAttribute(typeof(System.Drawing.Design.ImageEditor), typeof(System.Drawing.Design.BitmapEditor))]
        [DefaultValueAttribute(typeof(Image), "null"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Image UnCheckedImage { get; set; }

        public void SetItemCheckState(int index,CheckState checkState)
        {
            if( index < 0 || index >= Items.Count )
            {
                throw new IndexOutOfRangeException(string.Format("Index {0} is out of range ",index));
            }

            switch (checkState)
            {
                case CheckState.Checked:
                    if(!checkedIndices.Contains(index))
                    {
                        checkedIndices.Add(index);
                        Invalidate(GetItemRectangle(index));
                    }
                    break;
                case CheckState.Unchecked:
                    if(checkedIndices.Contains(index))
                    {
                        checkedIndices.Remove(index);
                        Invalidate(GetItemRectangle(index));
                    }
                    break;
            }
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            // Make sure we're not trying to draw something that isn't there.
            if (e.Index >= Items.Count || e.Index <= -1)
                return;

            // Get the item object.
            object item = Items[e.Index];
            if (item == null)
                return;

            //Get the DataBound DisplayMember
            //Exception will not happen at run-time.
// ReSharper disable PossibleNullReferenceException
            string displayText = string.IsNullOrEmpty(DisplayMember) ? item.ToString() : FilterItemOnProperty(item, DisplayMember).ToString().ToCamelCase();
// ReSharper restore PossibleNullReferenceException

            var currentImage = checkedIndices.Contains(e.Index) ? CheckedImage : UnCheckedImage;

            if(currentImage == null) return;

            // Draw the background color depending on 
            // if the item is selected or not.
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                // The item is selected.
                e.Graphics.FillRectangle(new SolidBrush(SystemColors.Highlight), e.Bounds);
            }
            else
            {
                // The item is NOT selected.
                e.Graphics.FillRectangle(new SolidBrush(Color.White), e.Bounds);
            }

            System.Drawing.Drawing2D.GraphicsContainer container =
                e.Graphics.BeginContainer();

            e.Graphics.SetClip(e.Bounds);
            var imageRectangle = new Rectangle
                           {
                               Location = new Point(0, e.Bounds.Y),
                               Size = new Size(currentImage.Width, currentImage.Height)
                           };

            var stringFormat = new StringFormat
                                   {
                                       Alignment = StringAlignment.Near,
                                       LineAlignment = StringAlignment.Center
                                   };
            e.Graphics.DrawImage(currentImage, imageRectangle);
            e.Graphics.DrawString(displayText, Font, textBrush,
                                  new RectangleF(currentImage.Width, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height),
                                  stringFormat);
            e.Graphics.EndContainer(container);
        }

        protected override void OnMeasureItem(MeasureItemEventArgs e)
        {
            base.OnMeasureItem(e);
            if(CheckedImage == null || UnCheckedImage == null) return;
            e.ItemHeight = checkedIndices.Contains(e.Index) ? CheckedImage.Height : UnCheckedImage.Height;
        }

        private int previousSelectedIndex = -1;

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            if(checkedIndices.Contains(SelectedIndex))
            {
                checkedIndices.Remove(SelectedIndex);
            }
            else if(SelectedIndex > -1 && SelectedIndex < Items.Count)
            {
                checkedIndices.Add(SelectedIndex);
            }

            if (previousSelectedIndex >= 0)
                Invalidate(GetItemRectangle(previousSelectedIndex));
            if (SelectedIndex >= 0)
                Invalidate(GetItemRectangle(SelectedIndex));

            previousSelectedIndex = SelectedIndex;
        }

    }
}
