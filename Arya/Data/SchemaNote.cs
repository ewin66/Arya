using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arya.Data
{
    partial class SchemaNote
    {
        partial void OnCreated()
        {
            SkuDataDbDataContext.DefaultTableValues(this);
        }
    }
}
