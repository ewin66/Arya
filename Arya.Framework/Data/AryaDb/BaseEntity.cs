using System;
using Arya.Framework.Extensions;

namespace Arya.Framework.Data.AryaDb
{
    public class BaseEntity
    {
        protected bool Initialize = true;

        private WeakReference _parentContextRef;

        public AryaDbDataContext ParentContext
        {
            get
            {
                if (_parentContextRef == null)
                    _parentContextRef = this.GetContext();

                if (_parentContextRef == null)
                    return null;

                if (_parentContextRef.Target == null || _parentContextRef.IsAlive == false)
                    return null;
                return (AryaDbDataContext)_parentContextRef.Target;
            }
            set
            {
                if (value != null)
                    _parentContextRef = new WeakReference(value);
            }
        }

        protected void InitEntity()
        {
            var parentContext = ParentContext;
            if (Initialize && parentContext != null)
            {
                AryaDbDataContext.DefaultInsertedTableValues(this, parentContext.CurrentUser.ID,
                    parentContext.CurrentProject.ID);
            }
        }
    }
}