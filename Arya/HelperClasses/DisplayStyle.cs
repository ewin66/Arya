using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Arya.Data;
using System.Drawing.Text;
using System.IO;
using Arya.Properties;

namespace Arya.HelperClasses
{
    public static class DisplayStyle
    {
        //only column style should carry the font
        //internal const string FontName = "Lucida Sans Unicode";
        private const float FontSize = 8.75f;
        public static PrivateFontCollection pfc;
        public static Font DefaultRegularFont;
        public static Font DefaultItalicFont;
        public static Font DefaultBoldFont;
        public static FontFamily DefaultFontName;
        private static readonly Color CustomGrey = Color.FromArgb(232, 232, 232);

        #region SchemaStyle

        public static readonly DataGridViewCellStyle CellStyleItalicColumn = new DataGridViewCellStyle
                                                                                 {
                                                                                     Font = DefaultItalicFont,
                                                                                 };


        public static readonly DataGridViewCellStyle CellStyleFirstRow = new DataGridViewCellStyle
        {
            BackColor = Color.Gainsboro,
            ForeColor = Color.Black,
            Font = DefaultBoldFont,
            Alignment =
                DataGridViewContentAlignment.
                MiddleLeft
        };

        public static readonly DataGridViewCellStyle CellStyleHeaderAttribute = new DataGridViewCellStyle
                                                                                    {
                                                                                        BackColor = Color.FromArgb(240, 240, 240),
                                                                                        ForeColor = Color.Black,
                                                                                        Font = DefaultBoldFont,
                                                                                        Alignment =
                                                                                            DataGridViewContentAlignment.
                                                                                            MiddleLeft
                                                                                    };

        public static readonly DataGridViewCellStyle CellStyleHeaderSchematus = new DataGridViewCellStyle
        {
            Font = DefaultBoldFont,
            BackColor = Color.RosyBrown,
            Alignment =
                DataGridViewContentAlignment
                .
                MiddleLeft
        };

        public static readonly DataGridViewCellStyle CellStyleHeaderTaxonomy = new DataGridViewCellStyle
        {
            Font = DefaultBoldFont,
            BackColor = Color.DarkKhaki
        };

        public static readonly DataGridViewCellStyle CellStyleMeta = new DataGridViewCellStyle
                                                                         {
                                                                             BackColor = Color.LemonChiffon,
                                                                             WrapMode = DataGridViewTriState.True
                                                                         };

        public static readonly DataGridViewCellStyle CellStyleSchemaBoolean = new DataGridViewCellStyle
                                                                                  {
                                                                                      BackColor = CustomGrey,
                                                                                      Alignment =
                                                                                          DataGridViewContentAlignment.
                                                                                          MiddleLeft,
                                                                                      FormatProvider =
                                                                                          new BoolFormatter(),
                                                                                      Format = "y-",
                                                                                      Font = DefaultItalicFont,
                                                                                  };


        public static readonly DataGridViewCellStyle CellStyleSchemaMetaBoolean = new DataGridViewCellStyle
                                                                                      {
                                                                                          BackColor = Color.LightGoldenrodYellow,
                                                                                          Alignment =
                                                                                              DataGridViewContentAlignment
                                                                                              .MiddleLeft,
                                                                                          FormatProvider =
                                                                                              new BoolFormatter(),
                                                                                          Format = "y-",
                                                                                          Font = DefaultItalicFont,
                                                                                      };
       

        public static readonly DataGridViewCellStyle CellStyleSchemaDecimal = new DataGridViewCellStyle
                                                                                  {
                                                                                      BackColor = CustomGrey,
                                                                                      Alignment =
                                                                                          DataGridViewContentAlignment.
                                                                                          MiddleLeft,
                                                                                      Format = "###.###;-###.###;—",
                                                                                      Font = DefaultItalicFont,
                                                                                  };
        public static readonly DataGridViewCellStyle CellStyleSchemaDecimalAttributeTab = new DataGridViewCellStyle
        {

            Alignment =
                DataGridViewContentAlignment.
                MiddleLeft,
            Format = "###.###;-###.###;—",
            Font = DefaultItalicFont,
        };

        public static readonly DataGridViewCellStyle CellStyleSchemaLeft = new DataGridViewCellStyle
                                                                               {
                                                                                   //BackColor = Color.PeachPuff,
                                                                                   WrapMode = DataGridViewTriState.True,
                                                                                   Alignment =
                                                                                       DataGridViewContentAlignment.
                                                                                       MiddleLeft
                                                                               };

        public static readonly DataGridViewCellStyle CellStyleSpecialCenter = new DataGridViewCellStyle
                                                                                  {
                                                                                      //BackColor =  Color.LightGoldenrodYellow,
                                                                                      Alignment =
                                                                                          DataGridViewContentAlignment.
                                                                                          MiddleLeft
                                                                                  };

        #endregion SchemaStyle

        #region cellStyles

        public static readonly DataGridViewCellStyle CellStyleDefaultCell = new DataGridViewCellStyle
                                                                       {
                                                                           BackColor = Color.Yellow,
                                                                           Alignment =
                                                                               DataGridViewContentAlignment.
                                                                               MiddleLeft,
                                                                           Font = DefaultRegularFont
                                                                       };


        public static readonly DataGridViewCellStyle CellStyleDefaultRankColumn = new DataGridViewCellStyle
                                                                                      {
                                                                                          Alignment =
                                                                                              DataGridViewContentAlignment
                                                                                              .MiddleLeft,
                                                                                          Font = DefaultItalicFont,
                                                                                          FormatProvider =
                                                                                              new RankFormatter()
                                                                                      };

        public static readonly DataGridViewCellStyle CellStyleDefaultRegularColumn = new DataGridViewCellStyle
                                                                                {
                                                                                    Alignment =
                                                                                        DataGridViewContentAlignment.
                                                                                        MiddleLeft,
                                                                                    Font = DefaultRegularFont
                                                                                };


        public static readonly DataGridViewCellStyle CellStyleAttributeHeaderColumn = new DataGridViewCellStyle
                                                                                          {
                                                                                              BackColor = Color.White,
                                                                                              Alignment =
                                                                                                  DataGridViewContentAlignment
                                                                                                  .
                                                                                                  MiddleLeft,
                                                                                              Font = DefaultItalicFont
                                                                                          };


        public static readonly DataGridViewCellStyle CellStyleEvenRow = new DataGridViewCellStyle
                                                                            {
                                                                                BackColor =
                                                                                    Color.FromArgb(233, 242, 249),
                                                                                Alignment =
                                                                                    DataGridViewContentAlignment.
                                                                                    MiddleLeft
                                                                            };

        public static readonly DataGridViewCellStyle CellStyleOddRow = new DataGridViewCellStyle
                                                                           {
                                                                               BackColor = Color.GhostWhite,
                                                                               // BackColor = Color.Red,
                                                                               //BackColor = Color.FromArgb(234, 234, 255)
                                                                           };


        public static readonly DataGridViewCellStyle CellStyleOddRowSkuGroup = new DataGridViewCellStyle
        {
            BackColor = Color.GhostWhite,
            //BackColor = Color.FromArgb(234, 234, 255)
        };


        public static readonly DataGridViewCellStyle CellStyleGreyItalic = new DataGridViewCellStyle
                                                                               {
                                                                                   Alignment =
                                                                                       DataGridViewContentAlignment.
                                                                                       MiddleLeft,
                                                                                       WrapMode = DataGridViewTriState.True,
                                                                                   Font = DefaultItalicFont,
                                                                                   BackColor = CustomGrey
                                                                               };


        public static readonly DataGridViewCellStyle CellStyleGreyMetaRegular = new DataGridViewCellStyle
                                                                                    {
                                                                                        WrapMode =
                                                                                            DataGridViewTriState.True,
                                                                                        Alignment =
                                                                                            DataGridViewContentAlignment
                                                                                            .
                                                                                            MiddleLeft,
                                                                                        Font = DefaultRegularFont,
                                                                                        BackColor = CustomGrey
                                                                                    };

        public static readonly DataGridViewCellStyle CellStyleGreyLovRegular = new DataGridViewCellStyle
        {
            BackColor = CustomGrey,
            WrapMode =
                DataGridViewTriState.True,
            Alignment =
                DataGridViewContentAlignment.MiddleLeft,
            Font = DefaultRegularFont,
            FormatProvider = new LovFormatter(),
            Format = "",

        };

        public static readonly DataGridViewCellStyle CellStyleGreyLovRegularAttributeTab = new DataGridViewCellStyle
        {

            WrapMode =
                DataGridViewTriState.True,
            Alignment =
                DataGridViewContentAlignment.MiddleLeft,
            Font = DefaultRegularFont,
            FormatProvider = new LovFormatter(),
            Format = "",

        };


        public static readonly DataGridViewCellStyle CellStyleGreyRegular = new DataGridViewCellStyle
                                                                                {
                                                                                    Alignment =
                                                                                        DataGridViewContentAlignment.
                                                                                        MiddleLeft,
                                                                                    Font = DefaultRegularFont,
                                                                                    BackColor = CustomGrey
                                                                                };

        public static readonly DataGridViewCellStyle CellStyleGreyRegularSkuGroup = new DataGridViewCellStyle
        {
            Alignment =
                DataGridViewContentAlignment.
                MiddleLeft,
            Font = DefaultRegularFont,
            BackColor = CustomGrey
        };

        public static readonly DataGridViewCellStyle CellStyleItemEvenColumn = new DataGridViewCellStyle
                                                                                   {
                                                                                       BackColor =
                                                                                           Color.FromArgb(233, 242, 249),
                                                                                       // light blue
                                                                                       Alignment =
                                                                                           DataGridViewContentAlignment.
                                                                                           MiddleLeft,
                                                                                       Font = DefaultRegularFont
                                                                                   };


        public static readonly DataGridViewCellStyle CellStyleSchemaMeta = new DataGridViewCellStyle
                                                                               {
                                                                                   WrapMode = DataGridViewTriState.True,
                                                                                   BackColor =
                                                                                       Color.FromArgb(233, 242, 249),
                                                                                   // light blue
                                                                                   Alignment =
                                                                                       DataGridViewContentAlignment.
                                                                                       MiddleLeft,
                                                                                   Font = DefaultRegularFont
                                                                               };


        public static readonly DataGridViewCellStyle CellStyleItemOddColumn = new DataGridViewCellStyle
                                                                                  {
                                                                                      BackColor =
                                                                                          Color.FromArgb(225, 237, 247),
                                                                                      //Dark Blue
                                                                                      Alignment =
                                                                                          DataGridViewContentAlignment.
                                                                                          MiddleLeft,
                                                                                      Font = DefaultRegularFont
                                                                                  };


        public static readonly DataGridViewCellStyle CellStyleItemIDColumn = new DataGridViewCellStyle
                                                                                 {
                                                                                     BackColor = CustomGrey,
                                                                                     ForeColor = Color.SaddleBrown,
                                                                                     Font = DefaultBoldFont,
                                                                                     Alignment =
                                                                                         DataGridViewContentAlignment.
                                                                                         MiddleLeft
                                                                                 };


        public static readonly DataGridViewCellStyle CellStyleAttributeColumn = new DataGridViewCellStyle
                                                                                    {
                                                                                        BackColor = CustomGrey,
                                                                                        ForeColor = Color.SaddleBrown,
                                                                                        Font = DefaultRegularFont,
                                                                                        Alignment =
                                                                                            DataGridViewContentAlignment
                                                                                            .
                                                                                            MiddleLeft
                                                                                    };

        public static readonly DataGridViewCellStyle CellStyleFirstRowItemIDHeader = new DataGridViewCellStyle
                                                                                         {
                                                                                             BackColor = Color.Gainsboro,
                                                                                             ForeColor = Color.Black,
                                                                                             Font = DefaultBoldFont,
                                                                                             Alignment =
                                                                                                 DataGridViewContentAlignment
                                                                                                 .
                                                                                                 MiddleLeft
                                                                                         };

        public static readonly DataGridViewCellStyle CellStyleSecondRowItemFields = new DataGridViewCellStyle
                                                                                        {
                                                                                            BackColor =
                                                                                                Color.FromArgb(244, 244,
                                                                                                               244),
                                                                                            Font = DefaultRegularFont,
                                                                                            WrapMode =
                                                                                                DataGridViewTriState.
                                                                                                True,
                                                                                            Alignment =
                                                                                                DataGridViewContentAlignment
                                                                                                .MiddleLeft,
                                                                                        };

        #endregion cellStyles


        static DisplayStyle()
        {
            pfc = new PrivateFontCollection();
            pfc.AddFontFile(Path.GetDirectoryName(Application.ExecutablePath) + "\\Resources\\ArialUnicode.ttf");

            DefaultFontName = pfc.Families[0];
            //DefaultFontName = FontFamily.GenericSansSerif;

            DefaultRegularFont = new Font(DefaultFontName, FontSize, FontStyle.Regular);
            DefaultItalicFont = new Font(DefaultFontName, FontSize, FontStyle.Italic);
            DefaultBoldFont = new Font(DefaultFontName, FontSize, FontStyle.Bold);
        }

        public static void SetDefaultFont(Control form)
        {
            form.Font = DefaultRegularFont;
            foreach (Control ctrl in form.Controls)
            {
                if (ctrl.Font != DefaultBoldFont && ctrl.Font != DefaultItalicFont)
                    ctrl.Font = DefaultRegularFont;
            }
        }
    }

    //Used for Navigation and Display Ranks
    public class RankFormatter : IFormatProvider, ICustomFormatter
    {
        public object GetFormat(Type formatType)
        {
            return formatType == typeof(ICustomFormatter) ? this : null;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider) { return arg == null ? string.Empty : (arg.ToString() == "0" ? "—" : arg.ToString()); }
    }

    public class BoolFormatter : IFormatProvider, ICustomFormatter
    {
        #region ICustomFormatter Members

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            Type x = arg.GetType();
            if (arg == null || arg.GetType() != typeof(bool))
                return HandleOtherFormats(format, arg);

            if (string.IsNullOrEmpty(format))
                return arg.ToString();

            if ((bool)arg)
                return format.StartsWith("y") ? "Yes" : arg.ToString();

            return format.Equals("yn") ? "No" : format.Equals("y-") ? "—" : string.Empty;
        }

        #endregion

        #region IFormatProvider Members

        public object GetFormat(Type formatType)
        {
            return formatType == typeof(ICustomFormatter) ? this : null;
        }

        #endregion

        private static string HandleOtherFormats(string format, object arg)
        {
            if (arg is SchemaMetaData)
            {
                if (format.Equals("y-"))
                {
                    if (arg.ToString().ToLower().StartsWith("y"))
                    {
                        return "Yes";
                    }
                    else
                        return "—";
                }
            }

            if (arg is IFormattable)
                return ((IFormattable)arg).ToString(format, CultureInfo.CurrentCulture);
            if (arg != null)
                return arg.ToString();
            return String.Empty;
        }
    }

    public class LovFormatter : IFormatProvider, ICustomFormatter
    {
        #region ICustomFormatter Members

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (arg.GetType() != typeof(string))
                return HandleOtherFormats(format, arg);

            string listSep = AryaTools.Instance.InstanceData.CurrentProject.ProjectPreferences.ListSeparator ?? ";";
            int charCount = 150;
            string result;
            var firstOrDefault = AryaTools.Instance.InstanceData.CurrentProject.UserProjects.FirstOrDefault(up => up.User == AryaTools.Instance.InstanceData.CurrentUser);
            if (firstOrDefault != null)
            {
                charCount = firstOrDefault.UserProjectPreferences.LovDisplayMax;
            }


            string argString = arg.ToString();
            if (argString.Length > charCount)
            {
                string cutString = argString.Substring(0, charCount);
                var currentValue = cutString.LastIndexOf(listSep, StringComparison.CurrentCulture);
                if (currentValue == -1)
                    result = cutString + "...";
                else
                    result = cutString.Substring(0, currentValue) + "...";
            }
            else
            {
                result = argString;
            }
            return result;

        }

        #endregion

        #region IFormatProvider Members

        public object GetFormat(Type formatType)
        {
            return formatType == typeof(ICustomFormatter) ? this : null;
        }

        #endregion

        private static string HandleOtherFormats(string format, object arg)
        {
            if (arg is IFormattable)
                return ((IFormattable)arg).ToString(format, CultureInfo.CurrentCulture);
            if (arg != null)
                return arg.ToString();
            return String.Empty;
        }
    }
}