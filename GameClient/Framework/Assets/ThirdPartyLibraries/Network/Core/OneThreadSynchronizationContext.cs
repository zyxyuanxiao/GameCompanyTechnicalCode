using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Network
{
    public class OneThreadSynchronizationContext : SynchronizationContext
    {
        public static OneThreadSynchronizationContext Instance { get; } = new OneThreadSynchronizationContext();

        private static readonly int mainThreadId = Thread.CurrentThread.ManagedThreadId;

        // 线程同步队列,发送接收socket回调都放到该队列,由poll线程统一执行
        private readonly ConcurrentQueue<Action> queue = new ConcurrentQueue<Action>();

        private Action a;

        //子线程的方法,使用线程安全的queue存起来,并在主线程中执行
        public void Update()
        {
            while (true)
            {
                if (!this.queue.TryDequeue(out a))
                {
                    return;//必须要有,不然死循环
                }
                a();
            }
        }

        public override void Post(SendOrPostCallback callback, object state)
        {
            if (Thread.CurrentThread.ManagedThreadId == mainThreadId)
            {
                callback(state);//如果是主线程则直接执行
                return;
            }
            //子线程的方法,使用线程安全的queue存起来,并在主线程中执行
            this.queue.Enqueue(() => { callback(state); });
        }
    }
}
