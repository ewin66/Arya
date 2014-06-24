using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows.Forms;
using Arya.Framework.Common.ComponentModel;

namespace Arya.Framework.Common.Extensions
{
    public static class LinqExtensions
    {
        #region Methods

        public static void AddOrReplace<T>(this IList<T> target, IEnumerable<T> source, IEqualityComparer<T> comparer)
        {
            target.AddOrReplaceWhere(source, comparer.Equals);
        }

        public static void AddOrReplaceWhere<TTarget, TSource>(this IList<TTarget> target, IEnumerable<TSource> source,
            Func<TSource, TTarget, bool> compareClause) where TSource : TTarget
        {
            if (source == null)
                return;
            var replacedIndexes = (from eachTarget in target
                from eachSource in source
                where compareClause(eachSource, eachTarget)
                select new {Index = target.IndexOf(eachTarget), NewItem = eachSource}).ToList();

            foreach (var item in replacedIndexes)
                target[item.Index] = item.NewItem;

            foreach (var newItem in source.Except(replacedIndexes.Select(p => p.NewItem)))
                target.Add(newItem);
        }

        public static void AddRange<T>(this ICollection<T> target, IEnumerable<T> source)
        {
            if (source == null)
                return;

            foreach (var item in source)
                target.Add(item);
        }

        public static IEnumerable<T> AsEnumerable<T>(this T item) { yield return item; }

        public static T CloneEntity<T>(this T originalEntity) where T : class
        {
            var entityType = typeof (T);

            var ser = new DataContractSerializer(entityType);

            using (var ms = new MemoryStream())
            {
                ser.WriteObject(ms, originalEntity);
                ms.Position = 0;
                return (T) ser.ReadObject(ms);
            }
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> items, T item) { return items.Concat(new[] {item}); }

        public static IEnumerable<TSource> Descendants<TSource>(this IEnumerable<TSource> source,
            Func<TSource, IEnumerable<TSource>> childSelector, int maxDepth) where TSource : class
        {
            if (maxDepth > 0)
            {
                foreach (var subChild in source.SelectMany(child => child.Descendants(childSelector, maxDepth)))
                    yield return subChild;
            }
        }

        public static IEnumerable<TSource> Descendants<TSource>(this TSource source,
            Func<TSource, IEnumerable<TSource>> childSelector, int maxDepth) where TSource : class
        {
            if (maxDepth > 0)
            {
                foreach (
                    var subChild in
                        childSelector(source).SelectMany(child => child.DescendantsAndSelf(childSelector, maxDepth - 1))
                    )
                    yield return subChild;
            }
        }

        public static IEnumerable<TSource> DescendantsAndSelf<TSource>(this IEnumerable<TSource> source,
            Func<TSource, IEnumerable<TSource>> childSelector, int maxDepth) where TSource : class
        {
            if (maxDepth > 0)
            {
                foreach (var subChild in source.SelectMany(child => child.DescendantsAndSelf(childSelector, maxDepth)))
                    yield return subChild;
            }
        }

        public static IEnumerable<TSource> DescendantsAndSelf<TSource>(this TSource source,
            Func<TSource, IEnumerable<TSource>> childSelector, int maxDepth) where TSource : class
        {
            if (maxDepth > 0)
            {
                yield return source;
                foreach (
                    var subChild in
                        childSelector(source).SelectMany(child => child.DescendantsAndSelf(childSelector, maxDepth - 1))
                    )
                    yield return subChild;
            }
        }

        public static int ElementIndex<T>(this IEnumerable<T> source, T element) where T : class
        {
            if (source == null)
                return -1;
            if (element == null)
                return -1;
            var i = 0;
            foreach (var item in source)
            {
                if (item.Equals(element))
                    return i;
                i++;
            }
            return -1;
        }

        public static IEnumerable<int> ElementIndexes<T>(this IList<T> source, IEnumerable<T> elements) where T : class
        {
            if (source == null)
                yield break;
            if (elements == null)
                yield break;
            // TODO: Use a sorted list to improve indexing performance
            foreach (var element in elements)
                yield return source.IndexOf(element);
        }

        public static IEnumerable<int> ElementIndexes<T>(this IEnumerable<T> source, IEnumerable<T> elements)
            where T : class
        {
            var i = 0;
            // TODO: Use a sorted list to improve indexing performance
            foreach (var element in elements)
            {
                foreach (var item in source)
                {
                    if (item.Equals(element))
                        yield return i;
                    i++;
                }
            }
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> items, T item)
        {
            return items.Where(p => !item.Equals(p));
        }

        public static IEnumerable<T[]> GetBlocks<T>(this IEnumerable<T> source, int blockSize)
        {
            var list = new List<T>(blockSize);
            foreach (var item in source)
            {
                list.Add(item);
                if (list.Count == blockSize)
                {
                    yield return list.ToArray();
                    list.Clear();
                }
            }
            if (list.Count > 0)
                yield return list.ToArray();
        }

        // Public Methods (26)
        public static IEnumerable<T> InRange<T, TValue>(this IQueryable<T> source, Expression<Func<T, TValue>> selector,
            int blockSize, IEnumerable<TValue> values)
        {
            var method = (from tmp in typeof (Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static)
                where tmp.Name == "Contains" && tmp.IsGenericMethodDefinition && tmp.GetParameters().Length == 2
                select tmp.MakeGenericMethod(typeof (TValue))).FirstOrDefault();

            if (method == null)
                throw new InvalidOperationException("Unable to locate Contains");
            return from block in values.GetBlocks(blockSize)
                let row = Expression.Parameter(typeof (T), "row")
                let member = Expression.Invoke(selector, row)
                let keys = Expression.Constant(block, typeof (TValue[]))
                let predicate = Expression.Call(method, keys, member)
                select Expression.Lambda<Func<T, bool>>(predicate, row)
                into lambda
                from record in source.Where(lambda)
                select record;
        }

        public static void InvokeEx<T>(this T @this, Action<T> action) where T : ISynchronizeInvoke
        {
            if (@this.InvokeRequired)
                @this.Invoke(action, new object[] {@this});
            else
                action(@this);
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source) where T : class
        {
            if (source == null)
                return true;
            return !source.Any();
        }

        public static bool OnlyContains<T>(this IEnumerable<T> items, IEnumerable<T> containsItems)
        {
            return !items.Except(containsItems).Any();
        }

        public static bool OnlyContains<T>(this IEnumerable<T> items, T containsItem)
        {
            return !items.Except(containsItem).Any();
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string property)
        {
            return ApplyOrder(source, property, "OrderBy");
        }

        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string property)
        {
            return ApplyOrder(source, property, "OrderByDescending");
        }

        public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> source, int size)
        {
            T[] array = null;
            var count = 0;
            foreach (var item in source)
            {
                if (array == null)
                    array = new T[size];
                array[count] = item;
                count++;
                if (count == size)
                {
                    yield return new ReadOnlyCollection<T>(array);
                    array = null;
                    count = 0;
                }
            }
            if (array != null)
            {
                Array.Resize(ref array, count);
                yield return new ReadOnlyCollection<T>(array);
            }
        }

        public static void RemoveAll<T>(this ICollection<T> target, IEnumerable<T> source)
        {
            if (source == null)
                return;

            var list = source.ToArray();
            foreach (var item in list)
                target.Remove(item);
        }

        public static void SafeInvoke(this Control uiElement, Action updater, bool forceSynchronous)
        {
            if (uiElement == null)
                throw new ArgumentNullException("uiElement");

            if (uiElement.InvokeRequired)
            {
                if (forceSynchronous)
                    uiElement.Invoke((Action) (() => SafeInvoke(uiElement, updater, true)));
                else
                    uiElement.BeginInvoke((Action) (() => SafeInvoke(uiElement, updater, false)));
            }
            else
            {
                if (uiElement.IsDisposed)
                    throw new ObjectDisposedException("Control is already disposed.");

                updater();
            }
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> items, int chunksize)
        {
            return Split(new List<T>(items), chunksize);
        }

        // perferred option
        public static IEnumerable<IEnumerable<T>> Split<T>(this ICollection<T> items, int chunksize)
        {
            if (chunksize <= 0)
                throw new ArgumentOutOfRangeException("chunksize");
            var array = new T[chunksize];
            var index = 0;
            foreach (var item in items)
            {
                array[index++] = item;
                if (index == chunksize)
                {
                    yield return new ReadOnlyCollection<T>(array);
                    index = 0;
                }
            }

            Array.Resize(ref array, index);
            yield return new ReadOnlyCollection<T>(array);
        }

        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string property)
        {
            return ApplyOrder(source, property, "ThenBy");
        }

        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string property)
        {
            return ApplyOrder(source, property, "ThenByDescending");
        }

        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            var props = TypeDescriptor.GetProperties(typeof (T));
            var table = new DataTable();
            for (var i = 0; i < props.Count; i++)
            {
                var prop = props[i];
                //table.Columns.Add(prop.Name, prop.PropertyType);
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            var values = new object[props.Count];
            foreach (var item in data)
            {
                for (var i = 0; i < values.Length; i++)
                    values[i] = props[i].GetValue(item) ?? DBNull.Value;
                table.Rows.Add(values);
            }
            return table;
        }

        public static DataTable ToDataTable<T>(this IEnumerable<T> data)
        {
            var props = TypeDescriptor.GetProperties(typeof (T));
            var table = new DataTable();
            for (var i = 0; i < props.Count; i++)
            {
                var prop = props[i];
                //table.Columns.Add(prop.Name, prop.PropertyType);
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            var values = new object[props.Count];
            foreach (var item in data)
            {
                for (var i = 0; i < values.Length; i++)
                    values[i] = props[i].GetValue(item) ?? DBNull.Value;
                table.Rows.Add(values);
            }
            return table;
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source) { return new HashSet<T>(source); }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer)
        {
            return new HashSet<T>(source, comparer);
        }

        public static SortableBindingList<T> ToSortableBindingList<T>(this IEnumerable<T> source) where T : class
        {
            return new SortableBindingList<T>(source);
        }

        /// <summary>
        /// Recursively Traverses the specified source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="fnRecurse">The fn recurse.</param>
        /// <returns></returns>
        public static IEnumerable<T> Traverse<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> fnRecurse)
        {
            if (source == null)
                yield break;

            foreach (var item in source)
            {
                yield return item;

                var seqRecurse = fnRecurse(item);

                if (seqRecurse == null)
                    continue;

                foreach (var itemRecurse in Traverse(seqRecurse, fnRecurse))
                    yield return itemRecurse;
            }
        }

        public static IEnumerable<T> Union<T>(this IEnumerable<T> items, T item) { return items.Union(new[] {item}); }

        public static IEnumerable<TSource> Update<TSource>(this IEnumerable<TSource> source, Action<TSource> update)
            where TSource : class
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (update == null)
                throw new ArgumentNullException("update");

            foreach (var element in source)
            {
                update(element);
                yield return element;
            }
        }

        public static IEnumerable<TTarget> UpdateWith<TTarget, TUpdateWith>(this IEnumerable<TTarget> target,
            IEnumerable<TUpdateWith> updateWith, Action<TTarget, TUpdateWith> update) where TTarget : class
        {
            if (target == null)
                throw new ArgumentNullException("target");
            if (update == null)
                throw new ArgumentNullException("update");

            var updates = updateWith.GetEnumerator();
            foreach (var element in target.TakeWhile(element => updates.MoveNext()))
            {
                //TUpdateWith updateValue = updateWith.Skip(counter).FirstOrDefault();
                update(element, updates.Current);
                yield return element;
            }
        }

        public static IOrderedQueryable<T> ApplyOrder<T>(IEnumerable<T> source, string property, string methodName)
        {
            return ApplyOrder(source.AsQueryable(), property, methodName);
        }

        public static IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, string property, string methodName)
        {
            var props = property.Split('.');
            var type = typeof (T);
            var arg = Expression.Parameter(type, "x");
            Expression expr = arg;
            foreach (var prop in props)
            {
                // use reflection (not ComponentModel) to mirror LINQ
                var pi = type.GetProperty(prop);
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }
            var delegateType = typeof (Func<,>).MakeGenericType(typeof (T), type);
            var lambda = Expression.Lambda(delegateType, expr, arg);

            var result =
                typeof (Queryable).GetMethods()
                    .Single(
                        method =>
                            method.Name == methodName && method.IsGenericMethodDefinition
                            && method.GetGenericArguments().Length == 2 && method.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof (T), type)
                    .Invoke(null, new object[] {source, lambda});
            return (IOrderedQueryable<T>) result;
        }

        #endregion Methods

       
    }
}