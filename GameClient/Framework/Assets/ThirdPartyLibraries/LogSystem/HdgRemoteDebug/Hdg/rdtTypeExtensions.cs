using System.Collections.Generic;
using System.Reflection;

namespace LogSystem
{
  public static class rdtTypeExtensions
  {
    private static List<FieldInfo> s_fields = new List<FieldInfo>(256);

    public static bool IsUserStruct(this System.Type type)
    {
      System.Type type1 = type;
      return !type1.IsPrimitive && !type1.IsEnum && type1.IsValueType;
    }

    public static bool IsGenericList(this System.Type type)
    {
      return type.IsGenericType && type.GetGenericTypeDefinition().IsAssignableFrom(typeof (List<>));
    }

    public static bool IsReference(this System.Type type)
    {
      return !type.IsValueType && (object) type != (object) typeof (string) && !type.IsArray && !type.IsGenericList();
    }

    public static System.Type GetListElementType(this System.Type type)
    {
      if (type.IsArray)
        return type.GetElementType();
      return !type.IsGenericList() ? (System.Type) null : type.GetGenericArguments()[0];
    }

    public static List<FieldInfo> GetAllFields(this System.Type t)
    {
      rdtTypeExtensions.s_fields.Clear();
      rdtTypeExtensions.GetAllFieldsImp(t);
      return rdtTypeExtensions.s_fields;
    }

    public static FieldInfo GetFieldInHierarchy(this System.Type t, string name)
    {
      if ((object) t == null)
        return (FieldInfo) null;
      BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
      FieldInfo fieldInfo = t.GetField(name, bindingAttr);
      if ((object) fieldInfo == null)
        fieldInfo = t.BaseType.GetFieldInHierarchy(name);
      return fieldInfo;
    }

    private static void GetAllFieldsImp(System.Type t)
    {
      if ((object) t == null)
        return;
      BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
      FieldInfo[] fields = t.GetFields(bindingAttr);
      rdtTypeExtensions.s_fields.AddRange((IEnumerable<FieldInfo>) fields);
      rdtTypeExtensions.GetAllFieldsImp(t.BaseType);
    }
  }
}
