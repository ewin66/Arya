using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arya.Data
{
    partial class EntityDataNote
    {
        partial void OnCreated()
        {
            SkuDataDbDataContext.DefaultTableValues(this);
        }
    }
}
