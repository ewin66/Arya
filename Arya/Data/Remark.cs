using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arya.Data
{
    public partial class Remark
    {

        partial void OnCreated()
        {
            SkuDataDbDataContext.DefaultTableValues(this);
        }

    }
}
