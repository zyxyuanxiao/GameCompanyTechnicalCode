using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace LogSystem
{
    [DisallowMultipleComponent]
    public class RemoteDebugServer : MonoBehaviour
    {
        private rdtDispatcher m_dispatcher = new rdtDispatcher();
        private Dictionary<System.Type, Action<rdtTcpMessage>> m_messageCallbacks = new Dictionary<System.Type, Action<rdtTcpMessage>>();
        private rdtSerializerRegistry m_serializerRegistry = new rdtSerializerRegistry();
        private List<GameObject> m_dontDestroyOnLoadObjects = new List<GameObject>();
        private ServerBroadcaster m_broadcaster;
        private RemoteDebugServer.State m_state;
        private TcpListener m_listener;
        private TcpClient m_client;
        private IAsyncResult m_currentAsyncResult;
        private Action[] m_stateDelegates;
        private WriteMessageThread m_writeThread;
        private ReadMessageThread m_readThread;
        private List<rdtTcpMessage> m_messagesToProcess;
        private static RemoteDebugServer s_instance;

        public static RemoteDebugServer Instance
        {
            get
            {
                if ((UnityEngine.Object)RemoteDebugServer.s_instance == (UnityEngine.Object)null)
                    RemoteDebugServer.s_instance = UnityEngine.Object.FindObjectOfType<RemoteDebugServer>();
                return RemoteDebugServer.s_instance;
            }
            set
            {
                RemoteDebugServer.s_instance = value;
            }
        }

        public List<GameObject> DontDestroyOnLoadObjects
        {
            get
            {
                return this.m_dontDestroyOnLoadObjects;
            }
        }

        public rdtSerializerRegistry SerializerRegistry
        {
            get
            {
                if (this.m_serializerRegistry == null)
                    this.m_serializerRegistry = new rdtSerializerRegistry();
                return this.m_serializerRegistry;
            }
        }

        public string ClientIP
        {
            get
            {
                return this.m_client == null ? "" : this.m_client.Client.RemoteEndPoint.ToString();
            }
        }

        public void AddCallback(System.Type type, Action<rdtTcpMessage> callback)
        {
            if (!this.m_messageCallbacks.ContainsKey(type))
                this.m_messageCallbacks[type] = (Action<rdtTcpMessage>)null;
            this.m_messageCallbacks[type] += callback;
        }

        public static void AddDontDestroyOnLoadObject(GameObject gob)
        {
            if ((UnityEngine.Object)RemoteDebugServer.Instance == (UnityEngine.Object)null)
                return;
            RemoteDebugServer.Instance.m_dontDestroyOnLoadObjects.Add(gob);
        }

        public static void RemoveDontDestroyOnLoadObject(GameObject gob)
        {
            if ((UnityEngine.Object)RemoteDebugServer.Instance == (UnityEngine.Object)null)
                return;
            RemoteDebugServer.Instance.m_dontDestroyOnLoadObjects.Remove(gob);
        }

        public void EnqueueMessage(rdtTcpMessage message)
        {
            if (this.m_writeThread == null)
                return;
            this.m_writeThread.EnqueueMessage(message);
        }

        [Button]
        public void ToggleWorldPaused()
        {
            Time.timeScale = (double)Time.timeScale == 0.0 ? 1f : 0.0f;
        }

        private void Init()
        {
            this.RegisterCallbacks();
            if (Application.platform == RuntimePlatform.IPhonePlayer)
                Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
            this.m_messagesToProcess = new List<rdtTcpMessage>(16);
            this.m_stateDelegates = new Action[5];
            this.m_stateDelegates[0] = (Action)null;
            this.m_stateDelegates[1] = new Action(this.OnWaiting);
            this.m_stateDelegates[2] = new Action(this.OnConnecting);
            this.m_stateDelegates[3] = new Action(this.OnConnected);
            this.m_stateDelegates[4] = new Action(this.OnDisconnected);
            this.m_broadcaster = new ServerBroadcaster();
        }

        private void RegisterCallbacks()
        {
            if (!Application.isEditor)
                Application.logMessageReceivedThreaded += new Application.LogCallback(this.OnLogMessageReceivedThreaded);
            this.m_messageCallbacks.Clear();
            rdtMessageGameObjectsHandler gameObjectsHandler = new rdtMessageGameObjectsHandler(this);
        }

        private void OnLogMessageReceivedThreaded(string message, string stackTrace, LogType type)
        {
            this.EnqueueMessage((rdtTcpMessage)new rdtTcpMessageLog(message, stackTrace, type));
        }

        private void OnEnable()
        {
            rdtDebug.Debug((object)this, nameof(OnEnable));
            if ((UnityEngine.Object)RemoteDebugServer.s_instance != (UnityEngine.Object)null && RemoteDebugServer.s_instance.GetInstanceID() != this.GetInstanceID())
            {
                UnityEngine.Object.Destroy((UnityEngine.Object)this.gameObject);
                RemoteDebugServer.RemoveDontDestroyOnLoadObject(this.gameObject);
            }
            else
            {
                RemoteDebugServer.Instance = this;
                rdtDebug.Debug((object)this, "{0} {1}", (object)RemoteDebugServer.s_instance.GetInstanceID(), (object)this.gameObject.GetInstanceID());
                UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object)this.gameObject);
                RemoteDebugServer.AddDontDestroyOnLoadObject(this.gameObject);
                this.Init();
                this.StartListening();
            }
        }

        private void OnDisable()
        {
            rdtDebug.Debug((object)this, nameof(OnDisable));
            if (this.m_broadcaster == null)
                return;
            this.m_broadcaster.Stop();
            if (this.m_listener != null)
                this.m_listener.Stop();
            this.m_listener = (TcpListener)null;
            this.Stop();
            this.SetState(RemoteDebugServer.State.None);
        }

        private void SetState(RemoteDebugServer.State state)
        {
            rdtDebug.Debug((object)this, "State is {0}", (object)state);
            this.m_state = state;
        }

        private void StartListening()
        {
            try
            {
                rdtDebug.Debug((object)this, "StartListening()");
                this.m_listener = new TcpListener(new IPEndPoint(IPAddress.Any, Settings.DEFAULT_SERVER_PORT));
                this.m_listener.Start(1);
                this.SetState(RemoteDebugServer.State.Waiting);
            }
            catch (Exception ex)
            {
                rdtDebug.Error((object)this, "Failed to listen to server port ({0})", (object)ex.Message);
            }
        }

        private void Stop()
        {
            if (this.m_client != null)
            {
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
        }

        private void Update()
        {
            try
            {
                if (this.m_stateDelegates[(int)this.m_state] == null)
                    return;
                this.m_stateDelegates[(int)this.m_state]();
            }
            catch (SocketException ex)
            {
                UnityEngine.Debug.Log("RemoteDebugServer SocketException:" + ex);
                this.Stop();
                this.StartListening();
            }
        }

        private void OnWaiting()
        {
            if (!this.m_listener.Pending())
                return;
            this.SetState(RemoteDebugServer.State.Connecting);
            this.m_currentAsyncResult = this.m_listener.BeginAcceptTcpClient((AsyncCallback)null, (object)null);
        }

        private void OnConnecting()
        {
            if (!this.m_currentAsyncResult.IsCompleted)
                return;
            try
            {
                this.m_client = this.m_listener.EndAcceptTcpClient(this.m_currentAsyncResult);
                rdtDebug.Debug((object)this, "Client connected from " + (object)this.m_client.Client.RemoteEndPoint);
                this.m_readThread = new ReadMessageThread((Stream)this.m_client.GetStream(), this.m_dispatcher, new Action<rdtTcpMessage>(this.OnReadMessage), "Server");
                this.m_writeThread = new WriteMessageThread((Stream)this.m_client.GetStream(), "Server");
                this.SetState(RemoteDebugServer.State.Connected);
            }
            catch (SocketException ex)
            {
                rdtDebug.Error((object)this, "Socket exception while client was connecting (error code " + (object)ex.ErrorCode + ")");
                this.StartListening();
            }
            catch (ObjectDisposedException ex)
            {
                rdtDebug.Error((object)this, "Tcp listener was disposed " + ex.ToString());
                this.StartListening();
            }
        }

        private void OnConnected()
        {
            if (this.m_listener.Pending())
                this.m_listener.AcceptTcpClient().Close();
            if (!this.m_readThread.IsConnected || !this.m_writeThread.IsConnected || !this.m_client.Connected)
            {
                rdtDebug.Debug((object)this, "Read/write thread or the client disconnected");
                this.SetState(RemoteDebugServer.State.Disconnected);
            }
            else
            {
                this.m_dispatcher.Update();
                bool flag1 = false;
                bool flag2 = false;
                for (int index = 0; index < this.m_messagesToProcess.Count; ++index)
                {
                    System.Type type = this.m_messagesToProcess[index].GetType();
                    if ((object)type == (object)typeof(rdtTcpMessageGetGameObjects))
                    {
                        if (flag1)
                        {
                            this.m_messagesToProcess.RemoveAt(index);
                            --index;
                        }
                        else
                            flag1 = true;
                    }
                    else if ((object)type == (object)typeof(rdtTcpMessageGetComponents))
                    {
                        if (flag2)
                        {
                            this.m_messagesToProcess.RemoveAt(index);
                            --index;
                        }
                        else
                            flag2 = true;
                    }
                }
                while (this.m_messagesToProcess.Count > 0)
                {
                    rdtTcpMessage rdtTcpMessage = this.m_messagesToProcess[0];
                    this.m_messagesToProcess.RemoveAt(0);
                    this.m_messageCallbacks[rdtTcpMessage.GetType()](rdtTcpMessage);
                }
            }
        }

        private void OnDisconnected()
        {
            rdtDebug.Debug((object)this, "Client disconnected");
            this.Stop();
            this.SetState(RemoteDebugServer.State.Waiting);
        }

        private void OnReadMessage(rdtTcpMessage message)
        {
            if (!this.m_messageCallbacks.ContainsKey(message.GetType()))
                return;
            this.m_messagesToProcess.Add(message);
        }

        private void OnApplicationPause(bool pause)
        {
            if (!Application.isMobilePlatform)
                return;
            if (pause)
            {
                rdtDebug.Debug((object)this, "OnApplicationPause: Is pausing");
                if (this.m_listener != null)
                {
                    this.m_listener.Stop();
                    this.m_listener = (TcpListener)null;
                }
                this.Stop();
                this.SetState(RemoteDebugServer.State.None);
            }
            else
            {
                if (this.m_listener != null)
                    return;
                rdtDebug.Debug((object)this, "OnApplicationPause: Is resuming");
                this.StartListening();
            }
        }

        private enum State
        {
            None,
            Waiting,
            Connecting,
            Connected,
            Disconnected,
            Max,
        }
    }
}
