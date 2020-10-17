using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace LogSystem
{
  public class rdtSerializerContainerArray : rdtSerializerInterface
  {
    private rdtSerializerContainerArray.ListType m_listType;
    private IList m_array;
    private SerialisationHelpers.ArrayElementType m_arrayElementType;

    public static object Serialize(object objIn, rdtSerializerRegistry registry)
    {
      rdtSerializerContainerArray serializerContainerArray = new rdtSerializerContainerArray();
      serializerContainerArray.SerializeImp((IList) objIn, registry);
      return (object) serializerContainerArray;
    }

    private void SerializeImp(IList array, rdtSerializerRegistry registry)
    {
      System.Type type = array.GetType();
      System.Type listElementType = type.GetListElementType();
      bool isSerializable = listElementType.IsSerializable;
      this.m_listType = type.IsGenericList() ? rdtSerializerContainerArray.ListType.List : rdtSerializerContainerArray.ListType.Array;
      if (!isSerializable || (object) listElementType == (object) typeof (object) || (listElementType.IsUserStruct() || listElementType.IsReference()))
      {
        int count = array.Count;
        this.m_array = (IList) new object[count];
        for (int index = 0; index < count; ++index)
        {
          object obj1 = array[index];
          object obj2 = registry.Serialize(obj1);
          this.m_array[index] = obj2;
        }
        this.m_arrayElementType = this.m_array.Count <= 0 || !(this.m_array[0] is rdtSerializerInterface) ? SerialisationHelpers.ArrayElementType.UserStruct : SerialisationHelpers.ArrayElementType.SerialiserInterface;
      }
      else
      {
        this.m_array = array;
        this.m_arrayElementType = SerialisationHelpers.ArrayElementType.Primitive;
      }
    }

    public object Deserialize(rdtSerializerRegistry registry)
    {
      return this.m_array == null || this.m_array.Count == 0 ? (object) null : this.DeserializeArray(registry.Deserialize(this.m_array[0]).GetType(), registry);
    }

    public object DeserializeArray(System.Type elementType, rdtSerializerRegistry registry)
    {
      object obj1 = (object) null;
      switch (this.m_listType)
      {
        case rdtSerializerContainerArray.ListType.Array:
          Array instance = Array.CreateInstance(elementType, this.m_array.Count);
          for (int index = 0; index < this.m_array.Count; ++index)
          {
            object obj2 = registry.Deserialize(this.m_array[index]);
            instance.SetValue(obj2, index);
          }
          obj1 = (object) instance;
          break;
        case rdtSerializerContainerArray.ListType.List:
          IList list = (IList) typeof (List<>).MakeGenericType(elementType).GetConstructor(System.Type.EmptyTypes).Invoke((object[]) null);
          for (int index = 0; index < this.m_array.Count; ++index)
          {
            object obj2 = registry.Deserialize(this.m_array[index]);
            list.Add(obj2);
          }
          obj1 = (object) list;
          break;
      }
      return obj1;
    }

    public void Write(BinaryWriter w)
    {
      w.Write((int) this.m_listType);
      SerialisationHelpers.WriteList(w, this.m_array, this.m_arrayElementType);
    }

    public void Read(BinaryReader r)
    {
      this.m_listType = (rdtSerializerContainerArray.ListType) r.ReadInt32();
      SerialisationHelpers.ReadList(r, out this.m_array, out this.m_arrayElementType);
    }

    private enum ListType
    {
      Array,
      List,
    }
  }
}
