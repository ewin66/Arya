using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arya.Data
{
    public partial class IgnoreWord
    {
        partial void OnCreated()
        {
            SkuDataDbDataContext.DefaultTableValues(this);
        }
    }
}
