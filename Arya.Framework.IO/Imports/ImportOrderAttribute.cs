using System;

namespace Arya.Framework.IO.Imports
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ImportOrderAttribute : Attribute, IComparable<ImportOrderAttribute>
    {
        private readonly int _order;

        public ImportOrderAttribute(int order)
        {
            _order = order;
        }

        public int Order
        {
            get { return _order; }
        }

        public int CompareTo(ImportOrderAttribute other)
        {
            if (Order == other.Order) return 0;
            if (Order < other.Order) return -1;
            return 1;
        }
    }
}
