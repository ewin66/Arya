using System;
using System.Collections.Generic;
using System.IO;
using Arya.Framework.Common;

namespace Arya.Framework.IO.Imports
{
    public class ImportArgs:WorkerArguments
    {
        public string InputFilePath { get; set; }
        public ImportOptions CurrentImportOptions { get; set; }
        public int UpdateFrequency { get; set; }
        public Delimiter FieldDelimiter { get; set; }
       // public string JobDescription { get; set; }
        public Dictionary<string, int> FieldMappings { get; set; }

        public static FileInfo GetImportArgumentsDirectoryPath(string folderPath, string fileName = ArgumentsFileName)
        {
            return new FileInfo(folderPath);
        }

        public ImportArgs()
        {
            PortalUrl = "Import.aspx";
        }
    }
}
