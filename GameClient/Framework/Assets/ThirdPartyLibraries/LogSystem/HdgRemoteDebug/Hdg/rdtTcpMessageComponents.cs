using System;
using System.Collections.Generic;
using System.IO;

namespace LogSystem
{
  public struct rdtTcpMessageComponents : rdtTcpMessage
  {
    public List<rdtTcpMessageComponents.Component> m_components;
    public int m_layer;
    public string m_tag;
    public bool m_enabled;
    public int m_instanceId;

    public void Write(BinaryWriter w)
    {
      int count = this.m_components.Count;
      w.Write(count);
      for (int index = 0; index < count; ++index)
        this.m_components[index].Write(w);
      w.Write(this.m_layer);
      w.Write(this.m_tag);
      w.Write(this.m_enabled);
      w.Write(this.m_instanceId);
    }

    public void Read(BinaryReader r)
    {
      this.m_components = new List<rdtTcpMessageComponents.Component>();
      int num = r.ReadInt32();
      for (int index = 0; index < num; ++index)
      {
        rdtTcpMessageComponents.Component component = new rdtTcpMessageComponents.Component();
        component.Read(r);
        this.m_components.Add(component);
      }
      this.m_layer = r.ReadInt32();
      this.m_tag = r.ReadString();
      this.m_enabled = r.ReadBoolean();
      this.m_instanceId = r.ReadInt32();
    }

    public struct Property
    {
      public string m_name;
      public object m_value;
      public rdtTcpMessageComponents.Property.Type m_type;
      public bool m_isArray;

      public override string ToString()
      {
        return string.Format("{0} = {1}", (object) this.m_name, this.m_value);
      }

      public rdtTcpMessageComponents.Property Clone()
      {
        return new rdtTcpMessageComponents.Property()
        {
          m_name = this.m_name,
          m_type = this.m_type,
          m_isArray = this.m_isArray
        };
      }

      public void Deserialise(rdtSerializerRegistry registry)
      {
        if (this.m_value is List<rdtTcpMessageComponents.Property> propertyList)
        {
          for (int index = 0; index < propertyList.Count; ++index)
          {
            rdtTcpMessageComponents.Property property = propertyList[index];
            property.Deserialise(registry);
            propertyList[index] = property;
          }
        }
        else
          this.m_value = registry.Deserialize(this.m_value);
      }

      public void Write(BinaryWriter w)
      {
        w.Write(this.m_name);
        w.Write((int) this.m_type);
        w.Write(this.m_isArray);
        if (this.m_value == null)
        {
          w.Write(true);
        }
        else
        {
          w.Write(false);
          List<rdtTcpMessageComponents.Property> properties = this.m_value as List<rdtTcpMessageComponents.Property>;
          w.Write(properties != null);
          if (properties != null)
          {
            rdtTcpMessageComponents.Component.WriteProperties(w, properties);
          }
          else
          {
            System.Type type = this.m_value.GetType();
            System.Type c = typeof (string);
            bool flag = type.IsPrimitive || type.IsAssignableFrom(c);
            w.Write(flag);
            if (flag)
            {
              SerialisationHelpers.WritePrimitive(w, this.m_value);
            }
            else
            {
              rdtSerializerInterface serializerInterface = this.m_value as rdtSerializerInterface;
              string fullName = serializerInterface.GetType().FullName;
              w.Write(fullName);
              serializerInterface.Write(w);
            }
          }
        }
      }

      public void Read(BinaryReader r)
      {
        this.m_name = r.ReadString();
        this.m_type = (rdtTcpMessageComponents.Property.Type) r.ReadInt32();
        this.m_isArray = r.ReadBoolean();
        if (r.ReadBoolean())
          return;
        if (r.ReadBoolean())
          this.m_value = (object) rdtTcpMessageComponents.Component.ReadProperties(r);
        else if (r.ReadBoolean())
        {
          this.m_value = SerialisationHelpers.ReadPrimitive(r);
        }
        else
        {
          rdtSerializerInterface instance = Activator.CreateInstance(System.Type.GetType(r.ReadString())) as rdtSerializerInterface;
          instance.Read(r);
          this.m_value = (object) instance;
        }
      }

      public enum Type
      {
        Field,
        Property,
        Method,
      }
    }

    public struct Component
    {
      public bool m_canBeDisabled;
      public bool m_enabled;
      public string m_name;
      public string m_assemblyName;
      public int m_instanceId;
      public List<rdtTcpMessageComponents.Property> m_properties;

      public override string ToString()
      {
        return string.Format("Component {0}", (object) this.m_name);
      }

      public void Write(BinaryWriter w)
      {
        w.Write(this.m_canBeDisabled);
        w.Write(this.m_enabled);
        w.Write(this.m_name);
        w.Write(this.m_assemblyName);
        w.Write(this.m_instanceId);
        rdtTcpMessageComponents.Component.WriteProperties(w, this.m_properties);
      }

      public static void WriteProperties(
        BinaryWriter w,
        List<rdtTcpMessageComponents.Property> properties)
      {
        int num = properties != null ? properties.Count : 0;
        w.Write(num);
        for (int index = 0; index < num; ++index)
          properties[index].Write(w);
      }

      public void Read(BinaryReader r)
      {
        this.m_canBeDisabled = r.ReadBoolean();
        this.m_enabled = r.ReadBoolean();
        this.m_name = r.ReadString();
        this.m_assemblyName = r.ReadString();
        this.m_instanceId = r.ReadInt32();
        this.m_properties = rdtTcpMessageComponents.Component.ReadProperties(r);
      }

      public static List<rdtTcpMessageComponents.Property> ReadProperties(
        BinaryReader r)
      {
        List<rdtTcpMessageComponents.Property> propertyList = new List<rdtTcpMessageComponents.Property>();
        int num = r.ReadInt32();
        for (int index = 0; index < num; ++index)
        {
          rdtTcpMessageComponents.Property property = new rdtTcpMessageComponents.Property();
          property.Read(r);
          propertyList.Add(property);
        }
        return propertyList;
      }
    }
  }
}
