using System.ComponentModel;
using System.Data.Linq.Mapping;
using Arya.Framework.Common;
using Arya.Framework.Data;

namespace Arya.Framework.IO.InterchangeRecords
{
    public class StringValueWrapperRecord : ITempTable
    {
        [Column(DbType = "VarChar(4000)", CanBeNull = false), Category(WorkerBase.CaptionRequired)]
        public string StringValue { get; set; }

        #region ITempTable Members

        public string GetCreateIndexString(string databaseName, string tableName) { return string.Empty; }

        #endregion
    }
}