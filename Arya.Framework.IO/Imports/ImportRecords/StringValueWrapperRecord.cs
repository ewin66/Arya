using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Natalie.Framework.Data;

namespace Natalie.Framework.IO.Imports.ImportRecords
{
    public class StringValueWrapperRecord: ITempTable
    {

        [Column(DbType = "VarChar(4000)"), Category(WorkerBase.CAPTION_REQUIRED)]
        public string StringValue { get; set; }




        public string GetCreateIndexString(string databaseName, string tableName)
        {
            return string.Empty;
        }
    }
}
