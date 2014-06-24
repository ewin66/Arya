using System;
using System.Drawing;
using System.Xml.Serialization;

namespace Arya.HelperClasses
{
	public class XmlColor
	{
		#region Fields (1) 

		private Color color_ = Color.Black;

		#endregion Fields 

		#region Constructors (2) 

		public XmlColor(Color c) { color_ = c; }

		public XmlColor() {}

		#endregion Constructors 

		#region Properties (2) 

		[XmlAttribute]
		public byte Alpha
		{
			get { return color_.A; }
			set { 
				if (value != color_.A) // avoid hammering named color if no alpha change
					color_ = Color.FromArgb(value, color_); 
			}
		}

		[XmlAttribute]
		public string Web
		{
			get { return ColorTranslator.ToHtml(color_); }
			set {
				try
				{
					color_ = Alpha == 0xFF ? ColorTranslator.FromHtml(value) : Color.FromArgb(Alpha, ColorTranslator.FromHtml(value));
				}
				catch(Exception)
				{
					color_ = Color.Black;
				}
			}
		}

		#endregion Properties 

		#region Methods (5) 

		// Public Methods (5) 

		public static implicit operator Color(XmlColor x)
		{
			return x.ToColor();
		}

		public void FromColor(Color c)
		{
			color_ = c;
		}

		public bool ShouldSerializeAlpha() { return Alpha < 0xFF; }

		public Color ToColor()
		{
			return color_;
		}

		public static implicit operator XmlColor(Color c)
		{
			return new XmlColor(c);
		}

		#endregion Methods 
	}
}