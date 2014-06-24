using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Arya.Framework.Collections.Generic;

namespace Arya.Framework.Collections.Concurrent
{
    public class ConcurrentDoubleKeyDictonary<TK1, TK2, TV> : IEnumerable<DoubleKeyPairValue<TK1, TK2, TV>>,
                                                IEquatable<ConcurrentDoubleKeyDictonary<TK1, TK2, TV>>
    {
        readonly ReaderWriterLockSlim _readerWriterLock = new ReaderWriterLockSlim();
        private Dictionary<TK2, TV> _mInnerDictionary;

        public ConcurrentDoubleKeyDictonary()
        {
            OuterDictionary = new Dictionary<TK1, Dictionary<TK2, TV>>();
        }

        public IEnumerable<TK1> Key1S
        {
            get 
            {
                try
                {
                    _readerWriterLock.EnterReadLock();
                    return OuterDictionary.Keys; 
                }
                finally
                {
                    _readerWriterLock.ExitReadLock();
                }
            }
        }

        private Dictionary<TK1, Dictionary<TK2, TV>> OuterDictionary { get; set; }

        public TV this[TK1 index1, TK2 index2]
        {
            get
            {
                try
                {
                    _readerWriterLock.EnterReadLock();
                    return OuterDictionary[index1][index2];
                }
                finally
                {
                    _readerWriterLock.ExitReadLock();
                }
            }
            set
            {
                Add(index1, index2, value);
            }
        }

        public Dictionary<TK2, TV> this[TK1 index]
        {
            get
            {
                _readerWriterLock.EnterReadLock();
                try
                {
                    return OuterDictionary[index];
                }
                finally
                {
                    _readerWriterLock.ExitReadLock();
                }
            }
        }

        public void Add(TK1 key1, TK2 key2, TV value)
        {
            _readerWriterLock.EnterUpgradeableReadLock();
            try
            {
                if (OuterDictionary.ContainsKey(key1))
                {
                    _mInnerDictionary = OuterDictionary[key1];

                    if (_mInnerDictionary.ContainsKey(key2))
                        OuterDictionary[key1][key2] = value;
                    else
                    {
                        _readerWriterLock.EnterWriteLock();
                        try
                        {
                            _mInnerDictionary.Add(key2, value);
                        }
                        finally
                        {
                            _readerWriterLock.ExitWriteLock();
                        }
                        OuterDictionary[key1] = _mInnerDictionary;
                    }
                }
                else
                {
                    _mInnerDictionary = new Dictionary<TK2, TV>();
                    _mInnerDictionary[key2] = value;
                    _readerWriterLock.EnterWriteLock();
                    try
                    {
                        OuterDictionary.Add(key1, _mInnerDictionary);
                    }
                    finally
                    {
                        _readerWriterLock.ExitWriteLock();
                    }
                    
                }
            }
            finally
            {
                _readerWriterLock.ExitReadLock();
            }
            
        }

        public bool ContainsKey(TK1 key1)
        {
            _readerWriterLock.EnterReadLock();
            try
            {
                return OuterDictionary.ContainsKey(key1);
            }
            finally
            {
                _readerWriterLock.ExitReadLock();
            }
        }

        public bool ContainsKeys(TK1 key1, TK2 key2)
        {
            _readerWriterLock.EnterReadLock();
            try
            {
                return OuterDictionary.ContainsKey(key1) && OuterDictionary[key1].ContainsKey(key2);
            }
            finally
            {
                _readerWriterLock.ExitReadLock();
            }
        }

        public IEnumerator<DoubleKeyPairValue<TK1, TK2, TV>> GetEnumerator()
        {
            _readerWriterLock.EnterReadLock();
            try
            {
                return (from outer in OuterDictionary from inner in outer.Value select new DoubleKeyPairValue<TK1, TK2, TV>(outer.Key, inner.Key, inner.Value)).GetEnumerator();
            }
            finally
            {
                _readerWriterLock.ExitReadLock();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Equals(ConcurrentDoubleKeyDictonary<TK1, TK2, TV> other)
        {
            _readerWriterLock.EnterReadLock();

            try
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

            finally
            {
                _readerWriterLock.ExitReadLock();
            }         
        }
    }
}
