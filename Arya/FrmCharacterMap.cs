using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Serialization;
using Arya.Data;
using Arya.HelperClasses;
using Arya.Properties;

namespace Arya
{
    using Framework.Extensions;

    public partial class FrmCharacterMap : Form
    {
		#region Fields (5) 

        private readonly Font _bigFont = new Font(DisplayStyle.DefaultFontName, 14F);
        private CharacterMapInformation _characterMapData;
        private const string ColumnDescription = "Description";
        private const string ColumnSymbol = "Symbol";
        private const string BrowserText="<html><head><style type=\"text/css\">.style1{{font-family: Arial, Helvetica, sans-serif;}}.style2{{font-family: Arial, Helvetica, sans-serif;font-size: xx-large;text-align: center;}}.style3{{font-family: \"Times New Roman\" , Times, serif;}}.style4{{font-family: \"Times New Roman\" , Times, serif;font-size: xx-large;text-align: center;}}.style5{{font-family: \"Courier New\" , Courier, monospace;}}.style6{{font-family: \"Courier New\" , Courier, monospace;font-size: xx-large;text-align: center;}}</style></head><body><table cellpadding=\"5\" align=\"center\"><tr><th>Font Family</th><th>Description</th><th>Symbol</th></tr><tr><td class=\"style1\">Arial, Helvetica, sans-serif:</td><td class=\"style1\">{0}</td><td class=\"style2\">{1}</td></tr><tr><td class=\"style3\">Times New Roman, Times, serif:</td><td class=\"style3\">{0}</td><td class=\"style4\">{1}</td></tr><tr><td class=\"style5\">'Courier New', Courier, monospace:</td><td class=\"style5\">{0}</td><td class=\"style6\">{1}</td></tr></table></body></html>";

        #endregion Fields 

		#region Constructors (1) 

        public FrmCharacterMap()
        {
            InitializeComponent(); DisplayStyle.SetDefaultFont(this);
            Icon = Resources.AryaLogoIcon;
            
            Init();
            DoFind();
        }

		#endregion Constructors 

		#region Methods (8) 

		// Public Methods (1) 

        // Private Methods (7) 

        private void dgvCharacterMaps_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            lblStatus.Text = @"Ready";
            if (dgvCharacterMaps.CurrentCell.OwningColumn.HeaderText.Equals(ColumnSymbol))
            {
                Clipboard.SetText(dgvCharacterMaps.CurrentCell.Value.ToString());
                lblStatus.Text = string.Format("{0}: {1} copied to clipboard",dgvCharacterMaps.CurrentCell.OwningRow.Cells[ColumnDescription].Value, dgvCharacterMaps.CurrentCell.Value);
                pnlStatus.Visible = true;
            }
        }

        private void DoFind()
        {
            string block = txtBlock.Text.ToLower();
            string description = txtDescription.Text.ToLower();
            string symbol = txtSymbol.Text.ToLower();

            IEnumerable<Character> data = _characterMapData.Characters;
            if(symbol.Length>0)
                data = data.Where(d => d.Symbol!=null && d.Symbol.Contains(symbol));

            if (block.Length > 0)
                data = data.Where(d => d.BlockLower.Contains(block));

            if (description.Length > 0)
                data = data.Where(d => d.DescriptionLower.Contains(description));

            dgvCharacterMaps.DataSource = data.Select(d => new {d.Block, d.Description, d.Symbol}).ToList();
            dgvCharacterMaps.Columns[ColumnSymbol].DefaultCellStyle.Font = _bigFont;
            dgvCharacterMaps.DefaultCellStyle = DisplayStyle.CellStyleGreyRegular;

            for (int i = 0; i < dgvCharacterMaps.RowCount; i++)
            {
                if (i%2 != 0)
                    dgvCharacterMaps.Rows[i].DefaultCellStyle = DisplayStyle.CellStyleOddRow;

            }
        }

        private void Init()
        {
            var charMapSerializer = new XmlSerializer(typeof (CharacterMapInformation));

            //var charMapFileStream = new FileStream("Plugins\\CharacterMap.xml", FileMode.Open);
            using (var charMapFileStream=Assembly.GetExecutingAssembly().GetManifestResourceStream("Arya.Resources.CharacterMap.xml"))
            {
                if (charMapFileStream != null)
                    _characterMapData = (CharacterMapInformation) charMapSerializer.Deserialize(charMapFileStream);
            }
            //charMapFileStream.Close();

            txtBlock.SetAutoComplete(_characterMapData.Characters.Select(d => d.Block).Distinct());
            txtDescription.SetAutoComplete(_characterMapData.Characters.Select(d => d.Description).Distinct());
        }

        private void lnkBrowserTest_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var row = dgvCharacterMaps.CurrentCell.OwningRow;
            AryaTools.Instance.Forms.BrowserForm.SetDocumentText(
                string.Format(BrowserText, row.Cells[ColumnDescription].Value, row.Cells[ColumnSymbol].Value));
            AryaTools.Instance.Forms.BrowserForm.BringToFront();
        }

        private void txtBlock_TextChanged(object sender, EventArgs e)
        {
            DoFind();
        }

        private void txtDescription_TextChanged(object sender, EventArgs e)
        {
            DoFind();
        }

        private void txtSymbol_TextChanged(object sender, EventArgs e)
        {
            DoFind();
        }

		#endregion Methods 

        private void dgvCharacterMaps_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex > -1 && e.RowIndex > -1)
            {
                e.AdvancedBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;
                e.AdvancedBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None;
            }
        }
    }
}
