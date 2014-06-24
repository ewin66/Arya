using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using System.Reflection;
using PropertyAttributes = System.Data.PropertyAttributes;

namespace Natalie.Framework.IO.Imports.ImportRecords
{
    public class SKUAttributeValueImportRecord : ImportRecord
    {
        [Column(DbType = "VarChar(255)", CanBeNull = false), Category(WorkerBase.CAPTION_REQUIRED)]
        public string ItemID { get; set; }

        [Column(DbType = "NVarChar(255)", CanBeNull = false), Category(WorkerBase.CAPTION_REQUIRED)]
        public string AttributeName { get; set; }

        [Column(DbType = "NVarChar(255)"), Category(WorkerBase.CAPTION_OPTIONAL)]
        public string Uom { get; set; }

        [Column(DbType = "NVarChar(4000)",CanBeNull = false), Category(WorkerBase.CAPTION_REQUIRED), ]
        public string Value { get; set; }

        [Column(DbType = "VarChar(255)"), Category(WorkerBase.CAPTION_OPTIONAL)]
        public string Field1 { get; set; }

        [Column(DbType = "VarChar(255)"), Category(WorkerBase.CAPTION_OPTIONAL)]
        public string Field2 { get; set; }

        [Column(DbType = "VarChar(255)"), Category(WorkerBase.CAPTION_OPTIONAL)]
        public string Field3 { get; set; }

        [Column(DbType = "VarChar(255)"), Category(WorkerBase.CAPTION_OPTIONAL)]
        public string Field4 { get; set; }

        [Column(DbType = "VarChar(255)"), Category(WorkerBase.CAPTION_OPTIONAL)]
        public string Field5 { get; set; }

        [Category(WorkerBase.CAPTION_OPTIONAL)]
        public bool BeforeEntity { get; set; }

        [Column(DbType = "Uniqueidentifier"), Category(WorkerBase.CAPTION_OPTIONAL)]
        public Guid? EntityID { get; set; }

        public override string GetCreateIndexString(string databaseName, string tableName)
        {
            return string.Empty;
        }
    }
}
