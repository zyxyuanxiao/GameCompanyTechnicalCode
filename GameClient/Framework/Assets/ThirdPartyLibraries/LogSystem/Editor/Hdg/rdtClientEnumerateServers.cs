using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEditor;

namespace LogSystem
{
    public class rdtClientEnumerateServers
    {
        private object m_lock = new object();
        private List<rdtServerAddress> m_servers = new List<rdtServerAddress>();
        private UdpClient m_udpHello;
        private IPEndPoint m_endPoint;
        private bool m_alreadyInUse;

        public bool Stopped { get; private set; }

        public List<rdtServerAddress> Servers
        {
            get
            {
                lock (this.m_lock)
                    return new List<rdtServerAddress>((IEnumerable<rdtServerAddress>)this.m_servers);
            }
        }

        public rdtClientEnumerateServers()
        {
            this.Start();
        }

        public void Stop()
        {
            try
            {
                if (this.m_udpHello != null)
                    this.m_udpHello.Close();
            }
            catch (Exception ex)
            {
                rdtDebug.Error((object)this, ex, "Tried to stop the client enumerator but we got an exception");
            }
            this.Stopped = true;
        }

        public void Reset()
        {
            lock (this.m_lock)
                this.m_servers.Clear();
        }

        public void Update(double delta)
        {
            if (this.Stopped)
                this.Start();
            lock (this.m_lock)
            {
                List<rdtServerAddress> rdtServerAddressList = new List<rdtServerAddress>();
                foreach (rdtServerAddress server in this.m_servers)
                {
                    server.m_timer += delta;
                    if (server.m_timer > (double)Settings.BROADCAST_TIME * 2.0)
                        rdtServerAddressList.Add(server);
                }
                foreach (rdtServerAddress rdtServerAddress in rdtServerAddressList)
                {
                    rdtDebug.Debug((object)this, "Removing " + rdtServerAddress.FormattedName + " because we haven't heard from it  timestamp=" + DateTime.Now.TimeOfDay.TotalSeconds.ToString());
                    this.m_servers.Remove(rdtServerAddress);
                }
            }
        }

        private void Start()
        {
            rdtDebug.Debug((object)this, "Starting hello listener");
            this.Stopped = false;
            try
            {
                int port = EditorPrefs.GetInt("Hdg.RemoteDebug.BroadcastPort", 12000);
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, port);
                this.m_udpHello = new UdpClient();
                this.m_udpHello.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.IpTimeToLive, true);
                this.m_udpHello.Client.Bind((EndPoint)ipEndPoint);
                this.m_udpHello.BeginReceive(new AsyncCallback(this.OnReceiveHelloCallback), (object)null);
                this.m_alreadyInUse = false;
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.AddressAlreadyInUse)
                {
                    if (!this.m_alreadyInUse)
                    {
                        this.m_alreadyInUse = true;
                        rdtDebug.Error("Remote Debug: Another instance of the server enumerator is already running!");
                    }
                }
                else
                    rdtDebug.Error((object)this, (Exception)ex, "Exception");
                this.Stop();
            }
            catch (Exception ex)
            {
                rdtDebug.Error((object)this, ex, "Exception");
                this.Stop();
            }
        }

        private void OnReceiveHelloCallback(IAsyncResult result)
        {
            bool flag = true;
            try
            {
                rdtUdpMessageHello rdtUdpMessageHello;
                using (MemoryStream memoryStream = new MemoryStream(this.m_udpHello.EndReceive(result, ref this.m_endPoint)))
                {
                    using (BinaryReader r = new BinaryReader((Stream)memoryStream))
                    {
                        rdtUdpMessageHello = new rdtUdpMessageHello();
                        rdtUdpMessageHello.Read(r);
                    }
                }
                if (rdtUdpMessageHello != null)
                {
                    lock (this.m_lock)
                    {
                        rdtServerAddress rdtServerAddress = new rdtServerAddress(this.m_endPoint.Address, rdtUdpMessageHello.m_serverPort, rdtUdpMessageHello.m_deviceName, rdtUdpMessageHello.m_deviceType, rdtUdpMessageHello.m_devicePlatform, rdtUdpMessageHello.m_serverVersion);
                        int index = this.m_servers.IndexOf(rdtServerAddress);
                        if (index >= 0)
                        {
                            this.m_servers[index].m_timer = 0.0;
                        }
                        else
                        {
                            rdtDebug.Debug((object)this, "Found a new server " + (object)rdtServerAddress.IPAddress + " called " + rdtServerAddress.FormattedName + " timestamp=" + DateTime.Now.TimeOfDay.TotalSeconds.ToString());
                            rdtDebug.Debug((object)this, "Server has version " + rdtServerAddress.m_serverVersion);
                            this.m_servers.Add(rdtServerAddress);
                        }
                    }
                }
                else
                    rdtDebug.Error((object)this, "Ignoring invalid message");
            }
            catch (ObjectDisposedException ex)
            {
                flag = false;
                rdtDebug.Debug((object)this, "Hello listener disposed   " + ex.ToString());
                this.Stop();
            }
            catch (Exception ex)
            {
                rdtDebug.Error((object)this, "Error {0}", (object)ex.Message);
            }
            if (!flag)
                return;
            this.m_udpHello.BeginReceive(new AsyncCallback(this.OnReceiveHelloCallback), (object)null);
        }
    }
}
