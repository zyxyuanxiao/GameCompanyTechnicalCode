// Decompiled with JetBrains decompiler
// Type: Hdg.rdtClient
// Assembly: HdgRemoteDebugEditor, Version=2.4.3599.0, Culture=neutral, PublicKeyToken=null
// MVID: A5601E38-B559-4F32-B1F6-261518F6D247
// Assembly location: /Users/xlcw/Desktop/HdgRemoteDebugEditor.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace LogSystem
{
    public class rdtClient
    {
        private rdtDispatcher m_dispatcher = new rdtDispatcher();
        private Dictionary<System.Type, Action<rdtTcpMessage>> m_messageCallbacks = new Dictionary<System.Type, Action<rdtTcpMessage>>();
        private rdtClient.State m_state;
        private TcpClient m_client;
        private IAsyncResult m_currentAsyncResult;
        private Action<double>[] m_stateDelegates;
        private WriteMessageThread m_writeThread;
        private ReadMessageThread m_readThread;

        public bool IsConnected
        {
            get
            {
                return this.m_state == rdtClient.State.Connected;
            }
        }

        public bool IsConnecting
        {
            get
            {
                return this.m_state == rdtClient.State.Connecting;
            }
        }

        public rdtClient()
        {
            this.m_stateDelegates = new Action<double>[4];
            this.m_stateDelegates[0] = (Action<double>)null;
            this.m_stateDelegates[1] = new Action<double>(this.OnConnecting);
            this.m_stateDelegates[2] = new Action<double>(this.OnConnected);
            this.m_stateDelegates[3] = new Action<double>(this.OnDisconnected);
        }

        public void AddCallback(System.Type type, Action<rdtTcpMessage> callback)
        {
            if (!this.m_messageCallbacks.ContainsKey(type))
                this.m_messageCallbacks[type] = (Action<rdtTcpMessage>)null;
            this.m_messageCallbacks[type] += callback;
        }

        private void SetState(rdtClient.State state)
        {
            rdtDebug.Debug((object)this, "State is {0}", (object)state);
            this.m_state = state;
        }

        public void Connect(IPAddress address, int port)
        {
            this.m_client = new TcpClient();
            this.m_currentAsyncResult = this.m_client.BeginConnect(address, port, (AsyncCallback)null, (object)null);
            this.SetState(rdtClient.State.Connecting);
        }

        public void EnqueueMessage(rdtTcpMessage message)
        {
            if (this.m_writeThread == null)
                return;
            this.m_writeThread.EnqueueMessage(message);
        }

        public void Stop()
        {
            if (this.m_client != null)
            {
                rdtDebug.Debug((object)this, "Stopping connection");
                this.m_client.Close();
                this.m_client = (TcpClient)null;
            }
            if (this.m_readThread != null)
            {
                this.m_readThread.Stop();
                this.m_readThread = (ReadMessageThread)null;
            }
            if (this.m_writeThread != null)
            {
                this.m_writeThread.Stop();
                this.m_writeThread = (WriteMessageThread)null;
            }
            this.m_dispatcher.Clear();
            this.SetState(rdtClient.State.None);
        }

        public void Update(double delta)
        {
            if (this.m_stateDelegates[(int)this.m_state] == null)
                return;
            this.m_stateDelegates[(int)this.m_state](delta);
        }

        private void OnConnecting(double delta)
        {
            if (!this.m_currentAsyncResult.IsCompleted)
                return;
            try
            {
                this.m_client.EndConnect(this.m_currentAsyncResult);
                rdtDebug.Debug((object)this, "Connected to server");
                this.m_writeThread = new WriteMessageThread((Stream)this.m_client.GetStream(), nameof(rdtClient));
                this.m_readThread = new ReadMessageThread((Stream)this.m_client.GetStream(), this.m_dispatcher, new Action<rdtTcpMessage>(this.OnReadMessage), nameof(rdtClient));
                this.SetState(rdtClient.State.Connected);
            }
            catch (SocketException ex)
            {
                switch (ex.ErrorCode)
                {
                    case 10060:
                        rdtDebug.Info("RemoteDebug: Connection timed out");
                        break;
                    case 10061:
                        rdtDebug.Info("RemoteDebug: Connection was refused");
                        break;
                    default:
                        rdtDebug.Info("RemoteDebug: Failed to connect to server (error code " + (object)ex.ErrorCode + ")");
                        break;
                }
                this.Stop();
            }
            catch (ObjectDisposedException ex)
            {
                rdtDebug.Error((object)this, "Client was disposed   " + ex.ToString());
                this.Stop();
            }
        }

        private void OnConnected(double delta)
        {
            if (!this.m_client.Connected || !this.m_writeThread.IsConnected || !this.m_readThread.IsConnected)
                this.SetState(rdtClient.State.Disconnected);
            else
                this.m_dispatcher.Update();
        }

        private void OnDisconnected(double delta)
        {
            rdtDebug.Debug((object)this, "Server disconnected");
            this.Stop();
        }

        private void OnReadMessage(rdtTcpMessage message)
        {
            if (message is rdtTcpMessageLog rdtTcpMessageLog)
            {
                switch (rdtTcpMessageLog.m_logType)
                {
                    case LogType.Error:
                        Debug.LogError((object)rdtTcpMessageLog);
                        break;
                    case LogType.Assert:
                        Debug.LogError((object)rdtTcpMessageLog);
                        break;
                    case LogType.Warning:
                        Debug.LogWarning((object)rdtTcpMessageLog);
                        break;
                    case LogType.Log:
                        Debug.Log((object)rdtTcpMessageLog);
                        break;
                    case LogType.Exception:
                        Debug.LogError((object)rdtTcpMessageLog);
                        break;
                }
            }
            if (!this.m_messageCallbacks.ContainsKey(message.GetType()))
                return;
            this.m_messageCallbacks[message.GetType()](message);
        }

        private enum State
        {
            None,
            Connecting,
            Connected,
            Disconnected,
            Max,
        }
    }
}
