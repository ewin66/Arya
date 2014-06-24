using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Arya.Framework.IO
{
    public class EnumerableTextReader : StreamReader
    {
        private readonly char _delimiter;
        private readonly int _numberOfColumns;
        public List<LineData> BadLines { get; private set; }
        private int _lineNumber;

        public EnumerableTextReader(string filePath, char delimiter, int numberOfColumns):base(filePath)
        {
            _delimiter = delimiter;
            _numberOfColumns = numberOfColumns;
            BadLines=new List<LineData>();
        }

        public override string ReadLine()
        {
            bool isRecordValid = false;
            string line = null;

            do
            {
                line = base.ReadLine();
                _lineNumber++;

                if (line == null)
                    return null;

                var parts = line.Split(_delimiter);
                if (parts.Count() != _numberOfColumns)
                {
                    BadLines.Add(new LineData {LineNumber = _lineNumber, LineText = line});
                    continue;
                }

                line = parts.Aggregate((current, part) =>
                                       current + _delimiter +
                                       (part.Contains("\"")
                                            ? string.Format("\"{0}\"", part.Replace("\"", "\"\""))
                                            : part)
                    );
                isRecordValid = true;
            } while (!isRecordValid);

            return line;
        }
    }
    
    //public class FileReader
    //{
    //    private List<LineData> _invalidRecords;
    //    #region Constant variables

    //    //private const string TargetEncoding = "UTF8"; 

    //    #endregion

    //    #region Constructors
    //    #endregion

    //    private CsvConfiguration currentConfiguration;

    //    public FileReader(CsvConfiguration currentConfiguration)
    //    {
    //        this.currentConfiguration = currentConfiguration;
    //    }

    //    #region Properties
    //    //public public TYPE Type { get; set; }
    //    public List<LineData> InvalidRecords
    //    {
    //        get
    //        {
    //            if (_invalidRecords == null)
    //            {
    //                _invalidRecords = new List<LineData>();
    //            }
    //            return _invalidRecords;
    //        }

    //    }

    //    #endregion

    //    MemoryStream stream = new MemoryStream();

    //    public IEnumerable<T> GetRecords<T>()
    //    {
    //        var stream = new Stream();
    //        var sr = new StreamReader()
    //        var csvReader = new CsvReader()
    //    }

    //    public IEnumerable<T> GetRecords<T>(string filePath, Delimiter fileDelimiter, int fieldMappingCount, CsvConfiguration currentConfiguration) where T : InterchangeRecord
    //    {
    //        List<string> validrecords = new List<string>();
    //        List<T> records;
    //        using (StreamReader sr = new StreamReader(filePath))
    //        {
    //            String line = sr.ReadLine();//ignore for header
    //            line = sr.ReadLine();
    //            int lineNumber = 2; //counting the headed    
    //            while (line != null)
    //            {

    //                var parts = line.Split((char)fileDelimiter.GetValue());
    //                if (parts.Count() != fieldMappingCount)//check for number of column in the row
    //                {
    //                    InvalidRecords.Add(new LineData() { LineNumber = lineNumber, LineText = line });
    //                    line = sr.ReadLine();
    //                    continue;
    //                }
    //                for (int i = 0; i < parts.Count(); i++)
    //                {
    //                    if (parts[i].Contains("\""))
    //                    {
    //                        parts[i] = "\"" + parts[i].Replace("\"", "\"\"") + "\"";
    //                    }
    //                }
    //                validrecords.Add(string.Join(fileDelimiter.GetValue().ToString(), parts));

    //                line = sr.ReadLine();

    //                lineNumber++;
    //            }
    //        }
    //        using (var csvReader = new CsvReader(new StreamReader(string.Join("\n", validrecords)),
    //                                                  currentConfiguration))
    //        {
    //            records = csvReader.GetRecordsWithNulls<T>().Select(p => p).ToList();
    //        }
    //        return (IEnumerable<T>)records.AsEnumerable();


    //    }


    //    public void ConvertToUtf(string filePath)
    //    {
    //        if (System.String.CompareOrdinal(FileHelper.GetFileEncoding(filePath).ToString(), Encoding.Unicode.ToString()) != 0)
    //        {
    //            string currentFileName = Path.GetFileName(filePath);
    //            string tempOutputFilePath = Path.Combine(Path.GetDirectoryName(filePath), "_temp" + currentFileName);
    //            using (var sre = new StreamReader(filePath))
    //            {
    //                //not the correct type
    //                using (var sw = new StreamWriter(tempOutputFilePath, false, Encoding.UTF8))
    //                {
    //                    sw.Write(sre.ReadToEnd());
    //                }
    //            }

    //            //sre.Close();
    //            //File.Delete(filePath);
    //            File.Replace(tempOutputFilePath, filePath, "_backup" + currentFileName);
    //        }
    //    }


    //}
}
