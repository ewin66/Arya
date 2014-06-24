using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Natalie.Data
{
    public partial class SkuGroupData
    {
        partial void OnCreated()
        {
            SkuDataDbDataContext.DefaultTableValues(this);
        }
    }
}
