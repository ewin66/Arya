using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Arya.Framework.Collections.Generic
{
    public class DoubleKeyDictionary<TK1, TK2, TV> : IEnumerable<DoubleKeyPairValue<TK1, TK2, TV>>,
                                                IEquatable<DoubleKeyDictionary<TK1, TK2, TV>>
    {
        #region Fields (1)

        private Dictionary<TK2, TV> _mInnerDictionary;

        #endregion Fields

        #region Constructors (1)

        public DoubleKeyDictionary()
        {
            OuterDictionary = new Dictionary<TK1, Dictionary<TK2, TV>>();
        }

        #endregion Constructors

        #region Properties (4)

        public IEnumerable<TK1> Key1S
        {
            get { return OuterDictionary.Keys; }
        }

        private Dictionary<TK1, Dictionary<TK2, TV>> OuterDictionary { get; set; }

        public TV this[TK1 index1, TK2 index2]
        {
            get { return OuterDictionary[index1][index2]; }
            set { Add(index1, index2, value); }
        }

        public Dictionary<TK2, TV> this[TK1 index]
        {
            get { return OuterDictionary[index]; }
        }

        #endregion Properties

        #region Methods (3)

        public void Clear()
        {
            _mInnerDictionary.Clear();
            OuterDictionary.Clear();
        }

        public void Remove(TK1 key1, TK2 key2)
        {
            if (!OuterDictionary.ContainsKey(key1))
                return;

            var inner = OuterDictionary[key1];

            if (!inner.ContainsKey(key2))
                return;

            inner.Remove(key2);
            if (!inner.Any())
                OuterDictionary.Remove(key1);
        }

        // Public Methods (3) 

        public void Add(TK1 key1, TK2 key2, TV value)
        {
            if (OuterDictionary.ContainsKey(key1))
            {
                _mInnerDictionary = OuterDictionary[key1];

                if (_mInnerDictionary.ContainsKey(key2))
                    OuterDictionary[key1][key2] = value;
                else
                {
                    _mInnerDictionary.Add(key2, value);
                    OuterDictionary[key1] = _mInnerDictionary;
                }
            }
            else
            {
                _mInnerDictionary = new Dictionary<TK2, TV>();
                _mInnerDictionary[key2] = value;
                OuterDictionary.Add(key1, _mInnerDictionary);
            }
        }

        public bool ContainsKey(TK1 key1)
        {
            return OuterDictionary.ContainsKey(key1);
        }

        public bool ContainsKeys(TK1 key1, TK2 key2)
        {
            return OuterDictionary.ContainsKey(key1) && OuterDictionary[key1].ContainsKey(key2);
        }

        #endregion Methods



        #region IEnumerable<DoubleKeyPairValue<K,T,V>> Members

        public IEnumerator<DoubleKeyPairValue<TK1, TK2, TV>> GetEnumerator()
        {
            return (from outer in OuterDictionary from inner in outer.Value select new DoubleKeyPairValue<TK1, TK2, TV>(outer.Key, inner.Key, inner.Value)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IEquatable<DoubleKeyDictionary<K,T,V>> Members

        public bool Equals(DoubleKeyDictionary<TK1, TK2, TV> other)
        {
            if (OuterDictionary.Keys.Count != other.OuterDictionary.Keys.Count)
                return false;

            bool isEqual = true;

            foreach (var innerItems in OuterDictionary)
            {
                if (!other.OuterDictionary.ContainsKey(innerItems.Key))
                    isEqual = false;

                if (!isEqual)
                    break;

                // here we can be sure that the key is in both lists, 
                // but we need to check the contents of the inner dictionary
                Dictionary<TK2, TV> otherInnerDictionary = other.OuterDictionary[innerItems.Key];
                foreach (var innerValue in innerItems.Value)
                {
                    if (!otherInnerDictionary.ContainsValue(innerValue.Value))
                        isEqual = false;
                    if (!otherInnerDictionary.ContainsKey(innerValue.Key))
                        isEqual = false;
                }

                if (!isEqual)
                    break;
            }

            return isEqual;
        }

        #endregion
    }
}
