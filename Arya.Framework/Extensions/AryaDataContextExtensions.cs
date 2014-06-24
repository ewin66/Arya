using System;
using System.Linq;
using System.Reflection;
using Arya.Framework.Data;
using Arya.Framework.Data.AryaDb;

namespace Arya.Framework.Extensions
{
    public static class AryaDataContextExtensions
    {
        public static WeakReference GetContext(this BaseEntity entity)
        {
            //var fEvent = GetPropertyChangingField(entity) as MulticastDelegate;
            var fEvent = GetFieldValue(entity, "PropertyChanging") as MulticastDelegate;
            if (fEvent == null) 
                return null;

            var invocationItem = fEvent.GetInvocationList().FirstOrDefault();
            if (invocationItem == null)
                return null;

            var changeTracker = invocationItem.Target;
            if (changeTracker == null)
                return null;

            var services = GetFieldValue(changeTracker, "services");
            return new WeakReference(GetFieldValue(services, "context"));
        }

        private static object GetFieldValue(object instance, string propertyName)
        {
            var field = instance.GetType().GetField(propertyName, BindingFlags.Instance | BindingFlags.NonPublic);
            return field == null ? null : field.GetValue(instance);
        }

        //private static readonly MemberGetter GetPropertyChangingField =
        //    typeof(BaseEntity).DelegateForGetFieldValue("PropertyChanging",
        //                                                 BindingFlags.NonPublic | BindingFlags.Instance);

        //private static readonly MemberGetter GetServicesField = typeof (AryaDbDataContext).DelegateForGetFieldValue(
        //    "services", BindingFlags.NonPublic | BindingFlags.Instance);

        //private static readonly MemberGetter GetContextProperty =
        //    typeof(AryaDbDataContext).DelegateForGetPropertyValue("Context", BindingFlags.NonPublic | BindingFlags.Instance);

        ///// <summary>
        ///// Returns a weak reference to the parent conext of an entity.
        ///// </summary>
        ///// <param name="obj"></param>
        ///// <returns>Returns AryaDbDataContext if it exists, other wise null</returns>
        //public static WeakReference GetContext(this BaseEntity obj)
        //{
        //    var fEvent = GetPropertyChangingField(obj) as MulticastDelegate;

        //    if (fEvent == null) return null;

        //    var services = GetServicesField(fEvent.Target);

        //    if (services == null) return null;

        //    // Get the Context
        //    var context = GetContextProperty(services) as AryaDbDataContext;
        //    return context != null ? new WeakReference(context) : null;
        //}
    }
}
