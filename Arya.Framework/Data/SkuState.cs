using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Natalie.Framework.Data
{
    partial class SkuState
    {
        partial void OnCreated()
        {
            SkuDataDbDataContext.DefaultTableValues(this);
        }
    }
}
