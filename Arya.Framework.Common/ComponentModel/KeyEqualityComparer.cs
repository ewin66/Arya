using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Arya.Framework.Common.ComponentModel
{
    public class KeyEqualityComparer<T> : IEqualityComparer<T>
    {
        #region Fields (1) 

        private readonly Func<T, object> _keyExtractor;

        #endregion Fields 

        #region Constructors (1) 

        public KeyEqualityComparer(Func<T, object> keyExtractor)
        {
            _keyExtractor = keyExtractor;
        }

        #endregion Constructors 



        #region IEqualityComparer<T> Members

        public bool Equals(T x, T y)
        {
            return _keyExtractor(x).Equals(_keyExtractor(y));
        }

        public int GetHashCode(T obj)
        {
            return _keyExtractor(obj).GetHashCode();
        }

        #endregion
    }

    public class GenericEqualityComparer<T> : IEqualityComparer<T>
    {
        readonly List<PropertyInfo> _properties=new List<PropertyInfo>();

        public GenericEqualityComparer()
        {
            _properties.AddRange(typeof(T).GetProperties());
        }

        public bool Equals(T x, T y)
        {
            foreach (var pi in _properties)
            {
                var xValue = pi.GetValue(x);
                var yValue = pi.GetValue(y);
                if (xValue == null && yValue == null)
                    continue;
                if (xValue == null || yValue == null)
                    return false;
                if(xValue!=yValue)
                    return false;
            }
            return true;
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }
}