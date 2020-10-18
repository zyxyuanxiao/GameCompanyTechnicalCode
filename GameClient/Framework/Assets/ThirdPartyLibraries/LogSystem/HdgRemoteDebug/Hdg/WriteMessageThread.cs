using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace LogSystem
{
    public class WriteMessageThread
    {
        private Queue<rdtTcpMessage> m_messageQueue = new Queue<rdtTcpMessage>();
        private AutoResetEvent m_event = new AutoResetEvent(false);
        private Stream m_stream;
        private BinaryWriter m_writer;
        private bool m_run;
        private Action[] m_stateDelegates;
        private WriteMessageThread.State m_state;
        private rdtTcpMessage m_currentMessage;
        private Thread m_thread;
        private string m_name;

        public bool IsConnected
        {
            get
            {
                return this.m_state != WriteMessageThread.State.LostConnection;
            }
        }

        public WriteMessageThread(Stream stream, string name)
        {
            this.m_name = name;
            this.m_stateDelegates = new Action[3];
            this.m_stateDelegates[0] = new Action(this.OnIdle);
            this.m_stateDelegates[1] = new Action(this.OnWriting);
            this.m_stateDelegates[2] = new Action(this.OnLostConnection);
            this.m_stream = stream;
            this.m_writer = new BinaryWriter(this.m_stream);
            this.m_run = true;
            this.m_thread = new Thread(new ThreadStart(this.ThreadFunc));
            this.m_thread.Name = this.m_name + " rdtWriteMessageThread";
            this.m_thread.Start();
        }

        public void Stop()
        {
            this.m_run = false;
            this.m_event.Set();
            this.m_thread.Join();
        }

        public void EnqueueMessage(rdtTcpMessage message)
        {
            lock (this.m_messageQueue)
                this.m_messageQueue.Enqueue(message);
            this.m_event.Set();
        }

        private void ThreadFunc()
        {
            while (this.m_run)
            {
                if (this.m_stateDelegates[(int)this.m_state] != null)
                    this.m_stateDelegates[(int)this.m_state]();
            }
            rdtDebug.Debug((object)this, "Exited");
        }

        private void OnIdle()
        {
            this.m_currentMessage = (rdtTcpMessage)null;
            lock (this.m_messageQueue)
            {
                if (this.m_messageQueue.Count > 0)
                    this.m_currentMessage = this.m_messageQueue.Dequeue();
            }
            if (this.m_currentMessage != null)
                this.m_state = WriteMessageThread.State.Writing;
            else
                this.m_event.WaitOne();
        }

        private void OnWriting()
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (BinaryWriter w = new BinaryWriter((Stream)memoryStream))
                    {
                        string fullName = this.m_currentMessage.GetType().FullName;
                        w.Write(fullName);
                        this.m_currentMessage.Write(w);
                        byte[] array = memoryStream.ToArray();
                        this.m_writer.Write(array.Length);
                        this.m_writer.Write(array);
                        this.m_writer.Flush();
                    }
                }
                this.m_state = WriteMessageThread.State.Idle;
            }
            catch (IOException ex)
            {
                rdtDebug.Log((object)this, (Exception)ex, rdtDebug.LogLevel.Debug, "{0} lost connection", (object)this.m_name);
                this.m_state = WriteMessageThread.State.LostConnection;
            }
            catch (ObjectDisposedException ex)
            {
                rdtDebug.Debug((object)this, "{0} object disposed, lost connection {1}", (object)this.m_name, ex.ToString());
                this.m_state = WriteMessageThread.State.LostConnection;
            }
            catch (Exception ex)
            {
                rdtDebug.Error((object)this, ex, "{0} Unknown exception", (object)this.m_name);
                this.m_state = WriteMessageThread.State.LostConnection;
            }
        }

        private void OnLostConnection()
        {
            this.m_run = false;
        }

        private enum State
        {
            Idle,
            Writing,
            LostConnection,
            Max,
        }
    }
}
