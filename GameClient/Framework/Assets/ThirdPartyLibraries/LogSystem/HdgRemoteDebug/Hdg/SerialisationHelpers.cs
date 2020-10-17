using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace LogSystem
{
  internal static class SerialisationHelpers
  {
    private static System.Type[] PrimitiveTypes = new System.Type[15]
    {
      typeof (byte),
      typeof (sbyte),
      typeof (int),
      typeof (uint),
      typeof (short),
      typeof (ushort),
      typeof (long),
      typeof (ulong),
      typeof (float),
      typeof (double),
      typeof (char),
      typeof (bool),
      typeof (string),
      typeof (Decimal),
      null
    };

    public static void WriteList(
      BinaryWriter bw,
      IList list,
      SerialisationHelpers.ArrayElementType type)
    {
      bw.Write((int) type);
      if (list == null || list.Count == 0)
      {
        bw.Write(0);
      }
      else
      {
        int count = list.Count;
        bw.Write(count);
        switch (type)
        {
          case SerialisationHelpers.ArrayElementType.Primitive:
            SerialisationHelpers.WritePrimitiveList(bw, list);
            break;
          case SerialisationHelpers.ArrayElementType.UserStruct:
            SerialisationHelpers.WriteUserStructList(bw, list);
            break;
          case SerialisationHelpers.ArrayElementType.SerialiserInterface:
            SerialisationHelpers.WriteSerialiserList(bw, list);
            break;
        }
      }
    }

    public static void WritePrimitiveList(BinaryWriter bw, IList array)
    {
      int count = array.Count;
      System.Type listElementType = array.GetType().GetListElementType();
      if ((object) listElementType == (object) typeof (byte))
      {
        bw.Write(0);
        for (int index = 0; index < count; ++index)
          bw.Write((byte) array[index]);
      }
      else if ((object) listElementType == (object) typeof (sbyte))
      {
        bw.Write(1);
        for (int index = 0; index < count; ++index)
          bw.Write((sbyte) array[index]);
      }
      else if ((object) listElementType == (object) typeof (int))
      {
        bw.Write(2);
        for (int index = 0; index < count; ++index)
          bw.Write((int) array[index]);
      }
      else if ((object) listElementType == (object) typeof (uint))
      {
        bw.Write(3);
        for (int index = 0; index < count; ++index)
          bw.Write((uint) array[index]);
      }
      else if ((object) listElementType == (object) typeof (short))
      {
        bw.Write(4);
        for (int index = 0; index < count; ++index)
          bw.Write((short) array[index]);
      }
      else if ((object) listElementType == (object) typeof (ushort))
      {
        bw.Write(5);
        for (int index = 0; index < count; ++index)
          bw.Write((ushort) array[index]);
      }
      else if ((object) listElementType == (object) typeof (long))
      {
        bw.Write(6);
        for (int index = 0; index < count; ++index)
          bw.Write((long) array[index]);
      }
      else if ((object) listElementType == (object) typeof (ulong))
      {
        bw.Write(7);
        for (int index = 0; index < count; ++index)
          bw.Write((ulong) array[index]);
      }
      else if ((object) listElementType == (object) typeof (float))
      {
        bw.Write(8);
        for (int index = 0; index < count; ++index)
          bw.Write((float) array[index]);
      }
      else if ((object) listElementType == (object) typeof (double))
      {
        bw.Write(9);
        for (int index = 0; index < count; ++index)
          bw.Write((double) array[index]);
      }
      else if ((object) listElementType == (object) typeof (char))
      {
        bw.Write(10);
        for (int index = 0; index < count; ++index)
          bw.Write((char) array[index]);
      }
      else if ((object) listElementType == (object) typeof (bool))
      {
        bw.Write(11);
        for (int index = 0; index < count; ++index)
          bw.Write((bool) array[index]);
      }
      else if ((object) listElementType == (object) typeof (string))
      {
        bw.Write(12);
        for (int index = 0; index < count; ++index)
          bw.Write((string) array[index]);
      }
      else if ((object) listElementType == (object) typeof (Decimal))
      {
        bw.Write(13);
        for (int index = 0; index < count; ++index)
          bw.Write((Decimal) array[index]);
      }
      else
      {
        RemoteDebugServer.Instance.SerializerRegistry.AddUnknownPrimitive(listElementType);
        bw.Write(14);
      }
    }

    public static void WriteSerialiserList(BinaryWriter bw, IList array)
    {
      string fullName = array[0].GetType().FullName;
      bw.Write(fullName);
      for (int index = 0; index < array.Count; ++index)
        (array[index] as rdtSerializerInterface).Write(bw);
    }

    public static void WriteUserStructList(BinaryWriter bw, IList array)
    {
      for (int index = 0; index < array.Count; ++index)
      {
        List<rdtTcpMessageComponents.Property> properties = array[index] as List<rdtTcpMessageComponents.Property>;
        rdtTcpMessageComponents.Component.WriteProperties(bw, properties);
      }
    }

    public static void WritePrimitive(BinaryWriter bw, object value)
    {
      if (value == null)
      {
        bw.Write(14);
      }
      else
      {
        System.Type type = value.GetType();
        if ((object) type == (object) typeof (byte))
        {
          bw.Write(0);
          bw.Write((byte) value);
        }
        else if ((object) type == (object) typeof (sbyte))
        {
          bw.Write(1);
          bw.Write((sbyte) value);
        }
        else if ((object) type == (object) typeof (int))
        {
          bw.Write(2);
          bw.Write((int) value);
        }
        else if ((object) type == (object) typeof (uint))
        {
          bw.Write(3);
          bw.Write((uint) value);
        }
        else if ((object) type == (object) typeof (short))
        {
          bw.Write(4);
          bw.Write((short) value);
        }
        else if ((object) type == (object) typeof (ushort))
        {
          bw.Write(5);
          bw.Write((ushort) value);
        }
        else if ((object) type == (object) typeof (long))
        {
          bw.Write(6);
          bw.Write((long) value);
        }
        else if ((object) type == (object) typeof (ulong))
        {
          bw.Write(7);
          bw.Write((ulong) value);
        }
        else if ((object) type == (object) typeof (float))
        {
          bw.Write(8);
          bw.Write((float) value);
        }
        else if ((object) type == (object) typeof (double))
        {
          bw.Write(9);
          bw.Write((double) value);
        }
        else if ((object) type == (object) typeof (char))
        {
          bw.Write(10);
          bw.Write((char) value);
        }
        else if ((object) type == (object) typeof (bool))
        {
          bw.Write(11);
          bw.Write((bool) value);
        }
        else if ((object) type == (object) typeof (string))
        {
          bw.Write(12);
          bw.Write((string) value);
        }
        else if ((object) type == (object) typeof (Decimal))
        {
          bw.Write(13);
          bw.Write((Decimal) value);
        }
        else
        {
          RemoteDebugServer.Instance.SerializerRegistry.AddUnknownPrimitive(type);
          bw.Write(14);
        }
      }
    }

    public static void ReadList(
      BinaryReader br,
      out IList list,
      out SerialisationHelpers.ArrayElementType type)
    {
      type = (SerialisationHelpers.ArrayElementType) br.ReadInt32();
      int count = br.ReadInt32();
      list = (IList) null;
      if (count == 0)
        return;
      switch (type)
      {
        case SerialisationHelpers.ArrayElementType.Primitive:
          list = (IList) SerialisationHelpers.ReadPrimitiveArray(br, count);
          break;
        case SerialisationHelpers.ArrayElementType.UserStruct:
          list = SerialisationHelpers.ReadUserStructArray(br, count);
          break;
        case SerialisationHelpers.ArrayElementType.SerialiserInterface:
          list = SerialisationHelpers.ReadSerialiserArray(br, count);
          break;
      }
    }

    public static Array ReadPrimitiveArray(BinaryReader r, int count)
    {
      SerialisationHelpers.PrimitiveType primitiveType = (SerialisationHelpers.PrimitiveType) r.ReadInt32();
      Array instance = Array.CreateInstance(typeof (object), count);
      SerialisationHelpers.ReadPrimitives(r, (IList) instance, count, primitiveType);
      return instance;
    }

    public static IList ReadPrimitiveList(BinaryReader r)
    {
      int count = r.ReadInt32();
      if (count == 0)
        return (IList) null;
      SerialisationHelpers.PrimitiveType primitiveType = (SerialisationHelpers.PrimitiveType) r.ReadInt32();
      IList array = (IList) typeof (List<>).MakeGenericType(SerialisationHelpers.PrimitiveTypes[(int) primitiveType]).GetConstructor(System.Type.EmptyTypes).Invoke((object[]) null);
      SerialisationHelpers.ReadPrimitives(r, array, count, primitiveType);
      return array;
    }

    public static IList ReadSerialiserArray(BinaryReader br, int count)
    {
      System.Type type = System.Type.GetType(br.ReadString());
      IList list = (IList) new object[count];
      for (int index = 0; index < count; ++index)
      {
        rdtSerializerInterface instance = Activator.CreateInstance(type) as rdtSerializerInterface;
        instance.Read(br);
        list[index] = (object) instance;
      }
      return list;
    }

    public static IList ReadUserStructArray(BinaryReader br, int count)
    {
      List<rdtTcpMessageComponents.Property>[] propertyListArray = new List<rdtTcpMessageComponents.Property>[count];
      for (int index = 0; index < count; ++index)
        propertyListArray[index] = rdtTcpMessageComponents.Component.ReadProperties(br);
      return (IList) propertyListArray;
    }

    private static void ReadPrimitives(
      BinaryReader r,
      IList array,
      int count,
      SerialisationHelpers.PrimitiveType primitiveType)
    {
      switch (primitiveType)
      {
        case SerialisationHelpers.PrimitiveType.Byte:
          for (int index = 0; index < count; ++index)
          {
            byte num = r.ReadByte();
            if (array.IsFixedSize)
              array[index] = (object) num;
            else
              array.Add((object) num);
          }
          break;
        case SerialisationHelpers.PrimitiveType.SByte:
          for (int index = 0; index < count; ++index)
          {
            sbyte num = r.ReadSByte();
            if (array.IsFixedSize)
              array[index] = (object) num;
            else
              array.Add((object) num);
          }
          break;
        case SerialisationHelpers.PrimitiveType.Int:
          for (int index = 0; index < count; ++index)
          {
            int num = r.ReadInt32();
            if (array.IsFixedSize)
              array[index] = (object) num;
            else
              array.Add((object) num);
          }
          break;
        case SerialisationHelpers.PrimitiveType.UInt:
          for (int index = 0; index < count; ++index)
          {
            uint num = r.ReadUInt32();
            if (array.IsFixedSize)
              array[index] = (object) num;
            else
              array.Add((object) num);
          }
          break;
        case SerialisationHelpers.PrimitiveType.Short:
          for (int index = 0; index < count; ++index)
          {
            short num = r.ReadInt16();
            if (array.IsFixedSize)
              array[index] = (object) num;
            else
              array.Add((object) num);
          }
          break;
        case SerialisationHelpers.PrimitiveType.UShort:
          for (int index = 0; index < count; ++index)
          {
            ushort num = r.ReadUInt16();
            if (array.IsFixedSize)
              array[index] = (object) num;
            else
              array.Add((object) num);
          }
          break;
        case SerialisationHelpers.PrimitiveType.Long:
          for (int index = 0; index < count; ++index)
          {
            long num = r.ReadInt64();
            if (array.IsFixedSize)
              array[index] = (object) num;
            else
              array.Add((object) num);
          }
          break;
        case SerialisationHelpers.PrimitiveType.ULong:
          for (int index = 0; index < count; ++index)
          {
            ulong num = r.ReadUInt64();
            if (array.IsFixedSize)
              array[index] = (object) num;
            else
              array.Add((object) num);
          }
          break;
        case SerialisationHelpers.PrimitiveType.Float:
          for (int index = 0; index < count; ++index)
          {
            float num = r.ReadSingle();
            if (array.IsFixedSize)
              array[index] = (object) num;
            else
              array.Add((object) num);
          }
          break;
        case SerialisationHelpers.PrimitiveType.Double:
          for (int index = 0; index < count; ++index)
          {
            double num = r.ReadDouble();
            if (array.IsFixedSize)
              array[index] = (object) num;
            else
              array.Add((object) num);
          }
          break;
        case SerialisationHelpers.PrimitiveType.Char:
          for (int index = 0; index < count; ++index)
          {
            char ch = r.ReadChar();
            if (array.IsFixedSize)
              array[index] = (object) ch;
            else
              array.Add((object) ch);
          }
          break;
        case SerialisationHelpers.PrimitiveType.Bool:
          for (int index = 0; index < count; ++index)
          {
            bool flag = r.ReadBoolean();
            if (array.IsFixedSize)
              array[index] = (object) flag;
            else
              array.Add((object) flag);
          }
          break;
        case SerialisationHelpers.PrimitiveType.String:
          for (int index = 0; index < count; ++index)
          {
            string str = r.ReadString();
            if (array.IsFixedSize)
              array[index] = (object) str;
            else
              array.Add((object) str);
          }
          break;
        case SerialisationHelpers.PrimitiveType.Decimal:
          for (int index = 0; index < count; ++index)
          {
            Decimal num = r.ReadDecimal();
            if (array.IsFixedSize)
              array[index] = (object) num;
            else
              array.Add((object) num);
          }
          break;
      }
    }

    public static object ReadPrimitive(BinaryReader r)
    {
      switch ((SerialisationHelpers.PrimitiveType) r.ReadInt32())
      {
        case SerialisationHelpers.PrimitiveType.Byte:
          return (object) r.ReadByte();
        case SerialisationHelpers.PrimitiveType.SByte:
          return (object) r.ReadSByte();
        case SerialisationHelpers.PrimitiveType.Int:
          return (object) r.ReadInt32();
        case SerialisationHelpers.PrimitiveType.UInt:
          return (object) r.ReadUInt32();
        case SerialisationHelpers.PrimitiveType.Short:
          return (object) r.ReadInt16();
        case SerialisationHelpers.PrimitiveType.UShort:
          return (object) r.ReadUInt16();
        case SerialisationHelpers.PrimitiveType.Long:
          return (object) r.ReadInt64();
        case SerialisationHelpers.PrimitiveType.ULong:
          return (object) r.ReadUInt64();
        case SerialisationHelpers.PrimitiveType.Float:
          return (object) r.ReadSingle();
        case SerialisationHelpers.PrimitiveType.Double:
          return (object) r.ReadDouble();
        case SerialisationHelpers.PrimitiveType.Char:
          return (object) r.ReadChar();
        case SerialisationHelpers.PrimitiveType.Bool:
          return (object) r.ReadBoolean();
        case SerialisationHelpers.PrimitiveType.String:
          return (object) r.ReadString();
        case SerialisationHelpers.PrimitiveType.Decimal:
          return (object) r.ReadDecimal();
        case SerialisationHelpers.PrimitiveType.Null:
          return (object) null;
        default:
          return (object) null;
      }
    }

    public enum ArrayElementType
    {
      Primitive,
      UserStruct,
      SerialiserInterface,
    }

    private enum PrimitiveType
    {
      Byte,
      SByte,
      Int,
      UInt,
      Short,
      UShort,
      Long,
      ULong,
      Float,
      Double,
      Char,
      Bool,
      String,
      Decimal,
      Null,
    }
  }
}
