using System;
using System.Net;

namespace LogSystem
{
  [Serializable]
  public class rdtServerAddress : IComparable<rdtServerAddress>
  {
    public string m_deviceName;
    public string m_deviceType;
    public string m_devicePlatform;
    private byte[] m_address;
    public int m_port;
    public double m_timer;
    private string m_formattedName;
    public string m_serverVersion;

    public IPAddress IPAddress
    {
      get
      {
        return this.m_address != null && this.m_address.Length != 0 ? new IPAddress(this.m_address) : (IPAddress) null;
      }
    }

    public string FormattedName
    {
      get
      {
        return this.m_formattedName;
      }
    }

    public rdtServerAddress(
      IPAddress address,
      int port,
      string name,
      string type,
      string platform,
      string serverVersion)
    {
      this.m_serverVersion = serverVersion;
      this.m_address = address.GetAddressBytes();
      this.m_port = port;
      this.m_deviceName = name ?? "";
      this.m_deviceType = type ?? "";
      this.m_devicePlatform = platform ?? "";
      this.m_timer = 0.0;
      bool flag = this.m_deviceName.Equals("<unknown>");
      this.m_formattedName = flag ? "" : this.m_deviceName + " ";
      if (!string.IsNullOrEmpty(this.m_devicePlatform))
      {
        if (!flag)
          this.m_formattedName += "- ";
        this.m_formattedName = this.m_formattedName + this.m_devicePlatform + " ";
      }
      this.m_formattedName += string.Format("({0}@{1}:{2})", (object) this.m_deviceType, (object) address.ToString(), (object) this.m_port);
    }

    public override bool Equals(object o)
    {
      if (o == null)
        return false;
      rdtServerAddress rdtServerAddress = o as rdtServerAddress;
      return rdtServerAddress.m_deviceName == this.m_deviceName && rdtServerAddress.m_deviceType == this.m_deviceType && (rdtServerAddress.m_devicePlatform == this.m_devicePlatform && rdtServerAddress.IPAddress.Equals((object) this.IPAddress)) && rdtServerAddress.m_port == this.m_port;
    }

    public override int GetHashCode()
    {
      int num = 13;
      if (this.m_deviceName != null)
        num = num * 7 + this.m_deviceName.GetHashCode();
      if (this.m_deviceType != null)
        num = num * 7 + this.m_deviceType.GetHashCode();
      if (this.m_devicePlatform != null)
        num = num * 7 + this.m_devicePlatform.GetHashCode();
      return (num * 7 + this.m_port.GetHashCode()) * 7 + this.m_address.GetHashCode();
    }

    public override string ToString()
    {
      return this.FormattedName;
    }

    public int CompareTo(rdtServerAddress other)
    {
      return this.FormattedName.CompareTo(other.FormattedName);
    }
  }
}
