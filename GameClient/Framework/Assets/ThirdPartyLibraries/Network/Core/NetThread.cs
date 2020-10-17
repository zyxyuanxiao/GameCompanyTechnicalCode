using System;
using System.Threading;

namespace Network
{

    public static class NetThread
    {
        private static Thread _thread;
        private static Action _action;


        public static void Run(Action action)
        {
            if (_thread != null)
            {
                UnityEngine.Debug.Log("已经创建了一个常驻线程,不需要再创建一次常驻线程了");
                return;
            }

            if (action == null)
            {
                UnityEngine.Debug.Log("传入的委托方法为空,无法创建线程");
                return;
            }
            _action = action;

            if (_thread == null)
            {
                _thread = new Thread(new ThreadStart(Update));
                _thread.IsBackground = false;//主线程的结束,会被此线程阻止,需要等待前台线程结束
                _thread.Start();
            }
        }

        public static bool IsAlive()
        {
            return _thread.IsAlive;
        }

        private static void Update()
        {
            while (true)
            {
                Thread.Sleep(33);//按照 1秒30帧 1帧33毫秒 来进行模拟
                _action();
            }
        }

        public static void Abort()
        {
            if (_thread != null)
            {
                _thread.Abort();
            }
        }
    }

}
