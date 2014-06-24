using System;
using System.Data;
using System.IO;
using System.Text;
using OfficeOpenXml;

namespace Arya.Framework.Utility
{
    public static class FileHelper
    {
        #region Methods

        public static Encoding GetFileEncoding(string filePath)
        {
            Encoding enc;
            var file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            if (file.CanSeek)
            {
                var bom = new byte[4]; // Get the byte-order mark, if there is one
                file.Read(bom, 0, 4);
                if ((bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) || // utf-8
                    (bom[0] == 0xff && bom[1] == 0xfe) || // ucs-2le, ucs-4le, and ucs-16le
                    (bom[0] == 0xfe && bom[1] == 0xff) || // utf-16 and ucs-2
                    (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff)) // ucs-4
                    enc = Encoding.Unicode;
                else
                    enc = Encoding.ASCII;

                // Now reposition the file cursor back to the start of the file
                file.Seek(0, SeekOrigin.Begin);
            }
            else
            {
                // The file cannot be randomly accessed, so you need to decide what to set the default to
                // based on the data provided. If you're expecting data from a lot of older applications,
                // default your encoding to Encoding.ASCII. If you're expecting data from a lot of newer
                // applications, default your encoding to Encoding.Unicode. Also, since binary files are
                // single byte-based, so you will want to use Encoding.ASCII, even though you'll probably
                // never need to use the encoding then since the Encoding classes are really meant to get
                // strings from the byte array that is the file.
                enc = Encoding.ASCII;
            }

            file.Close();
            file.Dispose();

            return enc;
        }

        public static void SaveXmlFile(this DataTable dataTable, string filePath)
        {
            dataTable.WriteXml(filePath);
        }

        public static void SaveExcelFile(this DataTable dataTable, string filePath)
        {
            var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add(dataTable.TableName);

            var row = 1;
            for (var col = 0; col < dataTable.Columns.Count && col < 16384; col++)
                ws.Cells[row, col + 1].Value = dataTable.Columns[col].ColumnName;

            foreach (DataRow dataRow in dataTable.Rows)
            {
                row++;
                if (row > 1048576)
                    break;

                for (var col = 0; col < dataTable.Columns.Count && col < 16384; col++)
                    ws.Cells[row, col + 1].Value = dataRow[col].ToString();
            }

            pck.SaveAs(new FileInfo(filePath));
        }

        public static void SaveTextFile(this DataTable dataTable, string filePath, string delimiter = "\t")
        {
            using (TextWriter file = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                var line = String.Empty;
                for (var i = 0; i < dataTable.Columns.Count; i++)
                {
                    line += (String.IsNullOrWhiteSpace(line) ? String.Empty : delimiter)
                            + dataTable.Columns[i].ColumnName;
                }
                file.WriteLine(line);

                for (var row = 0; row < dataTable.Rows.Count; row++)
                {
                    line = String.Empty;
                    for (var col = 0; col < dataTable.Columns.Count; col++)
                        line += (String.IsNullOrWhiteSpace(line) ? String.Empty : delimiter) + dataTable.Rows[row][col];
                    file.WriteLine(line);
                }
            }
        }

        public static void TextToExcel(string file, string delimiter, bool deleteSourceFile = true)
        {
            using (TextReader rdr = new StreamReader(file))
            {
                var pck = new ExcelPackage();
                var ws = pck.Workbook.Worksheets.Add("Data");

                var row = 1;
                string line;
                while ((line = rdr.ReadLine()) != null)
                {
                    var parts = line.Split(new[] {delimiter}, StringSplitOptions.None);
                    for (var col = 0; col < parts.Length; col++)
                    {
                        if (col > 16384)
                        {
                            Console.WriteLine("Line " + row
                                              + ": Too many columns in text file. Not all columns exported.");
                            break;
                        }

                        ws.Cells[row, col + 1].Value = parts[col];
                    }

                    row++;
                    if (row > 1048576)
                    {
                        Console.WriteLine("Too many lines in text file. Not all records exported.");
                        break;
                    }
                }

                pck.SaveAs(new FileInfo(Path.ChangeExtension(file, "xlsx")));

                if (deleteSourceFile)
                    File.Delete(file);
            }
        }

        #endregion Methods
    }
}