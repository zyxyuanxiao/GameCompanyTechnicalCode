using System.Collections.Generic;
using System.IO;

namespace LogSystem
{
  public struct rdtTcpMessageGameObjects : rdtTcpMessage
  {
    public List<rdtTcpMessageGameObjects.Gob> m_allGobs;

    public void Write(BinaryWriter w)
    {
      int count = this.m_allGobs.Count;
      w.Write(count);
      for (int index = 0; index < count; ++index)
        this.m_allGobs[index].Write(w);
    }

    public void Read(BinaryReader r)
    {
      this.m_allGobs = new List<rdtTcpMessageGameObjects.Gob>();
      int num = r.ReadInt32();
      for (int index = 0; index < num; ++index)
      {
        rdtTcpMessageGameObjects.Gob gob = new rdtTcpMessageGameObjects.Gob();
        gob.Read(r);
        this.m_allGobs.Add(gob);
      }
    }

    public struct Gob
    {
      public bool m_enabled;
      public string m_name;
      public int m_instanceId;
      public bool m_hasParent;
      public int m_parentInstanceId;
      public string m_scene;

      public override string ToString()
      {
        string str = this.m_name;
        if (rdtDebug.s_logLevel == rdtDebug.LogLevel.Debug)
          str = str + ":" + (object) this.m_instanceId;
        return str;
      }

      public override bool Equals(object obj)
      {
        return this.m_instanceId == ((rdtTcpMessageGameObjects.Gob) obj).m_instanceId;
      }

      public override int GetHashCode()
      {
        return this.m_instanceId;
      }

      public void Write(BinaryWriter w)
      {
        w.Write(this.m_enabled);
        w.Write(this.m_name);
        w.Write(this.m_instanceId);
        w.Write(this.m_hasParent);
        w.Write(this.m_parentInstanceId);
        w.Write(this.m_scene);
      }

      public void Read(BinaryReader r)
      {
        this.m_enabled = r.ReadBoolean();
        this.m_name = r.ReadString();
        this.m_instanceId = r.ReadInt32();
        this.m_hasParent = r.ReadBoolean();
        this.m_parentInstanceId = r.ReadInt32();
        this.m_scene = r.ReadString();
      }
    }
  }
}
