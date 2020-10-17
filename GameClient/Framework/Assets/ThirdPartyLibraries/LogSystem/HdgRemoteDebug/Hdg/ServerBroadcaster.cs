using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using UnityEngine;

namespace LogSystem
{
  internal class ServerBroadcaster
  {
    private Thread m_thread;
    private rdtUdpMessageHello m_message;
    private bool m_run;

    public ServerBroadcaster()
    {
      this.m_message = new rdtUdpMessageHello();
      this.m_message.m_deviceName = SystemInfo.deviceName;
      this.m_message.m_deviceType = SystemInfo.deviceType.ToString();
      this.m_message.m_devicePlatform = Application.platform.ToString();
      Assembly executingAssembly = Assembly.GetExecutingAssembly();
      Version version = executingAssembly.GetName().Version;
      object[] customAttributes1 = executingAssembly.GetCustomAttributes(typeof (AssemblyInformationalVersionAttribute), false);
      string str1 = "";
      if (customAttributes1.Length != 0)
        str1 = ((AssemblyInformationalVersionAttribute) customAttributes1[0]).InformationalVersion;
      object[] customAttributes2 = executingAssembly.GetCustomAttributes(typeof (AssemblyConfigurationAttribute), false);
      string str2 = "";
      if (customAttributes2.Length != 0)
        str2 = ((AssemblyConfigurationAttribute) customAttributes2[0]).Configuration;
      this.m_message.m_serverVersion = string.Format("{0}.{1}.{2} {3} {4}", (object) version.Major, (object) version.Minor, (object) version.Build, (object) str1, (object) str2);
      this.m_message.m_serverPort = Settings.DEFAULT_SERVER_PORT;
      this.m_run = true;
      this.m_thread = new Thread(new ThreadStart(this.ThreadFunc));
      this.m_thread.Name = "rdtServerBroadcaster";
      this.m_thread.Start();
    }

    public void Stop()
    {
      rdtDebug.Debug("Stopping server broadcaster thread");
      this.m_run = false;
      this.m_thread.Join();
    }

    private void ThreadFunc()
    {
      while (this.m_run)
      {
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, Settings.DEFAULT_BROADCAST_PORT);
        try
        {
          using (MemoryStream memoryStream = new MemoryStream())
          {
            using (BinaryWriter w = new BinaryWriter((Stream) memoryStream))
            {
              this.m_message.Write(w);
              byte[] array = memoryStream.ToArray();
              UdpClient udpClient = new UdpClient();
              udpClient.EnableBroadcast = true;
              udpClient.Connect(endPoint);
              udpClient.Send(array, array.Length);
              udpClient.Close();
            }
          }
        }
        catch (SocketException ex)
        {
          Exception exception = ex.InnerException != null ? ex.InnerException : (Exception) ex;
          rdtDebug.Error((object) this, (Exception) ex, "Couldn't broadcast hello message: {0}", (object) exception.Message);
          break;
        }
        catch (Exception ex)
        {
          Exception exception = ex.InnerException != null ? ex.InnerException : ex;
          rdtDebug.Error((object) this, ex, "Error broadcasting hello message: {0}", (object) exception.Message);
        }
        Thread.Sleep(Settings.BROADCAST_TIME * 1000);
      }
      rdtDebug.Debug((object) this, "Finishing broadcast thread");
    }
  }
}
