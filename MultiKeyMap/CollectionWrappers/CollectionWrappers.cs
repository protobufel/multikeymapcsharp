/*
Copyright 2017 David Tesler

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections;

namespace GitHub.Protobufel.MultiKeyMap.CollectionWrappers
{
    internal static partial class EnumerableExtensions
    {
        internal static IList<T> ToListWrapper<T>(this IEnumerable<T> source)
        {
            switch (source)
            {
                case null:
                    throw new ArgumentNullException("source");
                case IList<T> list:
                    return list;
                default:
                    return new VirtualList<T>(source);
            }
        }

        public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> source)
        {
            return new ReadOnlyCollection<T>(source.ToListWrapper());
        }

        public static ReadOnlyCollection<TResult> ToReadOnlyCollection<T, TResult>(this IEnumerable<T> source, Func<T, TResult> selector)
        {
            return new ReadOnlyCollection<TResult>(source.Select(selector).ToListWrapper());
        }

        public static ReadOnlyCollection<TResult> ToReadOnlyCollection<T, TResult>(this IEnumerable<T> source, Func<T, int, TResult> selector)
        {
            return new ReadOnlyCollection<TResult>(source.Select(selector).ToListWrapper());
        }
    }

    [Serializable]
    internal class VirtualList<T> : IList<T>
    {
        protected IEnumerable<T> source;

        public VirtualList(IEnumerable<T> source)
        {
            this.source = source;
        }

        public virtual T this[int index] { get => source.ElementAt(index); set => throw new NotImplementedException(); }

        public virtual int Count => source.Count();

        public virtual bool IsReadOnly => true;

        public virtual void Add(T item)
        {
            throw new NotImplementedException();
        }

        public virtual bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public virtual void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public virtual void Clear()
        {
            throw new NotImplementedException();
        }

        public virtual bool Contains(T item)
        {
            return source.Contains(item);
        }

        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            foreach (var item in source)
            {
                array[arrayIndex++] = item;
            }
        }

        public virtual int IndexOf(T item)
        {
            return source.Count(x => EqualityComparer<T>.Default.Equals(x, item));
        }

        public virtual void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in source)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
