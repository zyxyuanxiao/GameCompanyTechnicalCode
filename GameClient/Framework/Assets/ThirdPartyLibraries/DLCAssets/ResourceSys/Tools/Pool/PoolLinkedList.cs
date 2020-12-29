using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledLinkedList<T> : LinkedList<T>
{
    static Stack<LinkedListNode<T>> s_pool = new
    Stack<LinkedListNode<T>>(10);
    public PooledLinkedList()
    {
    }
    private LinkedListNode<T> Create(T t)
    {
        LinkedListNode<T> node = null;
        if (s_pool.Count > 0)
        {
            node = s_pool.Pop();
            node.Value = t;
        }
        else
        {
            node = new LinkedListNode<T>(t);
        }
        return node;
    }
    public new void AddLast(T t)
    {
        base.AddLast(Create(t));
    }
    public new void AddFirst(T t)
    {
        base.AddFirst(Create(t));
    }
    public new void AddAfter(LinkedListNode<T> n, T t)
    {
        base.AddAfter(n, Create(t));
    }
    public new void AddBefore(LinkedListNode<T> n, T t)
    {
        base.AddBefore(n, Create(t));
    }
    public new void Remove(LinkedListNode<T> n)
    {
        int count = Count;
        base.Remove(n);
        if (count != Count)
        {
            s_pool.Push(n);
        }
    }
    public new void Remove(T value)
    {        
        LinkedListNode<T> n = Create(value);
        Remove(n);
    }
    public new void RemoveFirst()
    {
        if (Count <= 0)
            return;
        s_pool.Push(this.First);
        base.RemoveFirst();
    }
    public new void RemoveLast()
    {
        if (Count <= 0)
            return;
        s_pool.Push(this.Last);
        base.RemoveLast();
    }
    public new void Clear()
    {
        if (Count <= 0)
            return;
        for (var cur = First; cur != null; cur = cur.Next)
        {
            s_pool.Push(cur);
        }
        base.Clear();
    }
}