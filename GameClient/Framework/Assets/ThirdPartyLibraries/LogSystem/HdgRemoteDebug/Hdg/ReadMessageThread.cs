using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace LogSystem
{
    public class ReadMessageThread
    {
        private Action[] m_stateDelegates;
        private ReadMessageThread.State m_state;
        private rdtDispatcher m_dispatcher;
        private Stream m_stream;
        private BinaryReader m_reader;
        private Thread m_thread;
        private bool m_run;
        private Action<rdtTcpMessage> m_callback;
        private string m_name;

        public bool IsConnected
        {
            get
            {
                return this.m_state != ReadMessageThread.State.LostConnection;
            }
        }

        public ReadMessageThread(
          Stream stream,
          rdtDispatcher dispatcher,
          Action<rdtTcpMessage> callback,
          string name)
        {
            this.m_name = name;
            this.m_stateDelegates = new Action[2];
            this.m_stateDelegates[0] = new Action(this.OnReading);
            this.m_stateDelegates[1] = new Action(this.OnLostConnection);
            this.m_stream = stream;
            this.m_reader = new BinaryReader(this.m_stream);
            this.m_dispatcher = dispatcher;
            this.m_callback = callback;
            this.m_run = true;
            this.m_thread = new Thread(new ThreadStart(this.ThreadFunc));
            this.m_thread.Name = this.m_name + " rdtReadMessageThread";
            this.m_thread.Start();
        }

        public void Stop()
        {
            this.m_run = false;
            this.m_thread.Join();
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

        private void OnReading()
        {
            try
            {
                int count = this.m_reader.ReadInt32();
                bool flag;
                if (count == 0)
                {
                    flag = true;
                }
                else
                {
                    byte[] buffer = this.m_reader.ReadBytes(count);
                    flag = false;
                    using (MemoryStream memoryStream = new MemoryStream(buffer))
                    {
                        using (BinaryReader r = new BinaryReader((Stream)memoryStream))
                        {
                            rdtTcpMessage message = Activator.CreateInstance(System.Type.GetType(r.ReadString())) as rdtTcpMessage;
                            if (message != null)
                            {
                                message.Read(r);
                                this.m_dispatcher.Enqueue((Action)(() => this.m_callback(message)));
                            }
                            else
                                rdtDebug.Error((object)this, "Ignoring invalid message");
                        }
                    }
                }
                if (!flag)
                    return;
                this.m_state = ReadMessageThread.State.LostConnection;
            }
            catch (SocketException ex)
            {
                rdtDebug.Log((object)this, (Exception)ex, rdtDebug.LogLevel.Debug, "{0} socket exception", (object)this.m_name);
                rdtDebug.Debug((object)this, "{3} ErrorCode={0} SocketErrorCode={1} NativeErrorCode={2}", (object)ex.ErrorCode, (object)ex.SocketErrorCode, (object)ex.NativeErrorCode, (object)this.m_name);
                this.m_state = ReadMessageThread.State.LostConnection;
            }
            catch (IOException ex)
            {
                if (ex.InnerException is SocketException innerException)
                {
                    rdtDebug.Log((object)this, (Exception)innerException, rdtDebug.LogLevel.Debug, "{0} socket exception", (object)this.m_name);
                    rdtDebug.Debug((object)this, "{3} ErrorCode={0} SocketErrorCode={1} NativeErrorCode={2}", (object)innerException.ErrorCode, (object)innerException.SocketErrorCode, (object)innerException.NativeErrorCode, (object)this.m_name);
                }
                else
                    rdtDebug.Log((object)this, (Exception)ex, rdtDebug.LogLevel.Debug, "{0} thread lost connection", (object)this.m_name);
                this.m_state = ReadMessageThread.State.LostConnection;
            }
            catch (ObjectDisposedException ex)
            {
                rdtDebug.Debug((object)this, "{0} thread object disposed, lost connection  {1}", (object)this.m_name, ex.ToString());
                this.m_state = ReadMessageThread.State.LostConnection;
            }
            catch (Exception ex)
            {
                rdtDebug.Error((object)this, ex, "{0} thread unknown exception", (object)this.m_name);
                this.m_state = ReadMessageThread.State.LostConnection;
            }
        }

        private void OnLostConnection()
        {
            this.m_run = false;
        }

        private enum State
        {
            Reading,
            LostConnection,
            Max,
        }
    }
}
