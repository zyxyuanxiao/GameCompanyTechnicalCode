using System;
using System.Threading;

namespace LogSystem
{
    /// <summary>
    /// 取自 <<NET CLR via C#>> 第 21 章托管堆和垃圾回收,垃圾回收触发条件
    /// </summary>
    public static class GCTools
    {
        //通知运行时在安排垃圾回收时应考虑分配大量的非托管内存。
        //GC.AddMemoryPressure(10*1024*1024);//分配 10MB 非托管内存
        //GC.RemoveMemoryPressure(10*1024*1024);//销毁 10MB 非托管内存
        //强制对所有代进行即时垃圾回收
        //GC.Collect();
        //强制对零代到指定代进行即时垃圾回收。
        //GC.Collect(0);//对 0 代进行垃圾回收,最多 3 代
        //强制在 GCCollectionMode 值所指定的时间对零代到指定代进行垃圾回收
        //GC.Collect(0,System.GCCollectionMode.Default);//0代默认回收
        //返回已经对对象的指定代进行的垃圾回收次数。
        // GC.CollectionCount(0);//对 0 代进行 GC 的次数
        //返回指定对象的当前代数。
        //GC.GetGeneration(obj);//obj 属于哪一代?
        //返回指定弱引用的目标的当前代数。
        //GC.GetGeneration(WeakReference);//弱引用对象属于哪一代
        //检索当前认为要分配的字节数。 一个参数，指示此方法是否可以等待较短间隔再返回，以便系统回收垃圾和终结对象。
        //GC.GetTotalMemory(true);单位是 byte
        //引用指定对象，使其从当前例程开始到调用此方法的那一刻为止均不符合进行垃圾回收的条件。
        //GC.KeepAlive(obj);不让其在 GC 阶段回收内存.
        //指定当条件支持完整垃圾回收以及回收完成时，应引发垃圾回收通知。
        //GC.RegisterForFullGCNotification(1-99, 1-99);// http://www.csref.cn/vs100/method/System-GC-RegisterForFullGCNotification.html
        //请求系统调用指定对象的终结器，此前已为该对象调用 SuppressFinalize。
        //GC.ReRegisterForFinalize(this);
        //请求系统不要调用指定对象的终结器。
        //GC.SuppressFinalize(obj);
        //返回已注册通知的状态，用于确定公共语言运行时是否即将引发完整垃圾回收。
        //GC.WaitForFullGCApproach();
        //挂起当前线程，直到处理终结器队列的线程清空该队列为止。
        //GC.WaitForPendingFinalizers();
        //GCHanadle 手动监视和控制对象的生存期
        //GCSettings默认是客户端模式的 GC,消耗小,可以设置为服务器模式的 GC,消耗大,垃圾回收模式
        
        private static Action<Int32> s_gcDone = null;

        public static event Action<Int32> GCDone
        {
            add
            {
                //如果之前没有登记的委托,就开始报告通知
                if (s_gcDone == null)
                {
                    new GenObject(0);
                    new GenObject(2);
                }

                s_gcDone += value;
            }
            remove { s_gcDone -= value; }
        }

        private sealed class GenObject
        {
            private Int32 m_generation;

            public GenObject(Int32 generation)
            {
                m_generation = generation;
            }

            ~GenObject() //这个是 Finalize 方法
            {
                //如果这个对象在我们希望的(或更高的)代中. 3>2>1>0,GC 是先将 0 代中的对象清除,再 1 代,再 2 代.最多 3 代.
                //就通知委托一次 GC 刚刚完成
                int g = GC.GetGeneration(this);
                if (g >= m_generation)
                {
                    Action<Int32> temp = Volatile.Read(ref s_gcDone);
                    //回调的次数越多,说明 GC 的次数越多,总内存越小,说明可分配的内存块越小,正常情况下 6-10 秒左右回调一次,大小7-15MB
                    temp(g);
                }

                //如果至少还有一个已登记的委托,而且 AppDomain 并非正在卸载
                //而且进程并非正在关闭,就继续报告通知
                if ((s_gcDone != null) &&
                    !AppDomain.CurrentDomain.IsFinalizingForUnload() &&
                    !Environment.HasShutdownStarted)
                {
                    //对于第 0 代,创建一个新对象;对于第二代,复活对象,
                    //使第 2 代在下次回收时,GC 会再次调用 Finalize
                    if (m_generation == 0)
                    {
                        new GenObject(0);
                    }
                    else
                    {
                        GC.ReRegisterForFinalize(this);
                    }
                }
            }
        }

    }
}