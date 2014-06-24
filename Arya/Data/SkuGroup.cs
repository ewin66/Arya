using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arya.Data
{
    public partial class SkuGroup
    {

        partial void OnCreated()
        {
            SkuDataDbDataContext.DefaultTableValues(this);
        }
    }
}
