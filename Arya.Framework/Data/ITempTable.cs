using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arya.Framework.Data
{
    public interface ITempTable
    {
        string GetCreateIndexString(string databaseName, string tableName);
    }
}
