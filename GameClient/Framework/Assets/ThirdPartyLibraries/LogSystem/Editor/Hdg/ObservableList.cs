using System;
using System.Collections.Generic;

namespace LogSystem
{
    public class ObservableList<T> : List<T>
    {
        public event Action<ObservableList<T>> ListChanged = _param1 => { };

        public new void Add(T item)
        {
            base.Add(item);
            this.ListChanged(this);
        }

        new public void Remove(T item)
        {
            base.Remove(item);
            this.ListChanged(this);
        }

        public new void AddRange(IEnumerable<T> collection)
        {
            base.AddRange(collection);
            this.ListChanged(this);
        }

        public new void RemoveRange(int index, int count)
        {
            base.RemoveRange(index, count);
            this.ListChanged(this);
        }

        public void ReplaceAll(T item)
        {
            base.Clear();
            base.Add(item);
            this.ListChanged(this);
        }

        public void ReplaceAll(IEnumerable<T> collection)
        {
            base.Clear();
            base.AddRange(collection);
            this.ListChanged(this);
        }

        public new void Clear()
        {
            base.Clear();
            this.ListChanged(this);
        }

        public new void Insert(int index, T item)
        {
            base.Insert(index, item);
            this.ListChanged(this);
        }

        public new void InsertRange(int index, IEnumerable<T> collection)
        {
            base.InsertRange(index, collection);
            this.ListChanged(this);
        }

        new public void RemoveAll(Predicate<T> match)
        {
            base.RemoveAll(match);
            this.ListChanged(this);
        }

        public new T this[int index]
        {
            get
            {
                return base[index];
            }
            set
            {
                base[index] = value;
                this.ListChanged(this);
            }
        }
    }
}
