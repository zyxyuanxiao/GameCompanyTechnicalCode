using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace LogSystem
{
  public class rdtSerializerRegistry
  {
    private Dictionary<System.Type, rdtSerializerRegistry.ConvertObjectDelegate> m_converters = new Dictionary<System.Type, rdtSerializerRegistry.ConvertObjectDelegate>();
    private HashSet<System.Type> m_failures = new HashSet<System.Type>();
    private HashSet<System.Type> m_referenceFailures = new HashSet<System.Type>();
    private HashSet<System.Type> m_unknownPrimitives = new HashSet<System.Type>();
    private HashSet<string> m_skipProperties = new HashSet<string>();
    private HashSet<string> m_skipTypes = new HashSet<string>();
    private Dictionary<string, HashSet<string>> m_skipPropertiesPerType = new Dictionary<string, HashSet<string>>();
    private Dictionary<string, HashSet<string>> m_includePropertiesPerType = new Dictionary<string, HashSet<string>>();
    private HashSet<string> m_dontReadProperties = new HashSet<string>();

    private object NotHandledConversion(object objIn, rdtSerializerRegistry r)
    {
      return (object) null;
    }

    public rdtSerializerRegistry()
    {
      this.m_converters.Add(typeof (Vector2), (rdtSerializerRegistry.ConvertObjectDelegate) ((objIn, r) => (object) new rdtSerializerVector2((Vector2) objIn)));
      this.m_converters.Add(typeof (Vector3), (rdtSerializerRegistry.ConvertObjectDelegate) ((objIn, r) => (object) new rdtSerializerVector3((Vector3) objIn)));
      this.m_converters.Add(typeof (Vector4), (rdtSerializerRegistry.ConvertObjectDelegate) ((objIn, r) => (object) new rdtSerializerVector4((Vector4) objIn)));
      this.m_converters.Add(typeof (Quaternion), (rdtSerializerRegistry.ConvertObjectDelegate) ((objIn, r) => (object) new rdtSerializerQuaternion((Quaternion) objIn)));
      this.m_converters.Add(typeof (Color), (rdtSerializerRegistry.ConvertObjectDelegate) ((objIn, r) => (object) new rdtSerializerColor((Color) objIn)));
      this.m_converters.Add(typeof (Color32), (rdtSerializerRegistry.ConvertObjectDelegate) ((objIn, r) => (object) new rdtSerializerColor32((Color32) objIn)));
      this.m_converters.Add(typeof (Rect), (rdtSerializerRegistry.ConvertObjectDelegate) ((objIn, r) => (object) new rdtSerializerRect((Rect) objIn)));
      this.m_converters.Add(typeof (Bounds), (rdtSerializerRegistry.ConvertObjectDelegate) ((objIn, r) => (object) new rdtSerializerBounds((Bounds) objIn)));
      this.m_converters.Add(typeof (Matrix4x4), (rdtSerializerRegistry.ConvertObjectDelegate) ((objIn, r) => (object) new rdtSerializerMatrix4x4((Matrix4x4) objIn)));
      this.m_converters.Add(typeof (List<>), new rdtSerializerRegistry.ConvertObjectDelegate(rdtSerializerContainerArray.Serialize));
      this.m_converters.Add(typeof (Dictionary<,>), new rdtSerializerRegistry.ConvertObjectDelegate(this.NotHandledConversion));
      this.m_converters.Add(typeof (Array), new rdtSerializerRegistry.ConvertObjectDelegate(rdtSerializerContainerArray.Serialize));
      this.InitSkipProperties();
    }

    public void AddUnknownPrimitive(System.Type type)
    {
      if (this.m_unknownPrimitives.Contains(type))
        return;
      rdtDebug.Warning("Remote Debug: Tried to serialise an unknown primitive type '{0}' ({1})", (object) type, (object) type.FullName);
      this.m_unknownPrimitives.Add(type);
    }

    public object Serialize(object obj)
    {
      if (obj == null || obj.Equals((object) null))
        return (object) null;
      object obj1 = obj;
      System.Type type1 = obj.GetType();
      if (typeof (rdtSerializerInterface).IsAssignableFrom(type1))
        return obj1;
      if (type1.IsArray)
        type1 = typeof (Array);
      else if (type1.IsGenericType)
        type1 = type1.GetGenericTypeDefinition();
      rdtSerializerRegistry.ConvertObjectDelegate convertObjectDelegate;
      if (this.m_converters.TryGetValue(type1, out convertObjectDelegate))
        obj1 = convertObjectDelegate(obj, this);
      else if (type1.IsUserStruct() || type1.IsReference())
        obj1 = (object) this.ReadAllFields(obj);
      if (obj1 != null)
      {
        System.Type type2 = obj1.GetType();
        if (!type2.IsSerializable && !typeof (rdtSerializerInterface).IsAssignableFrom(type2))
        {
          if (!this.m_failures.Contains(type2))
          {
            rdtDebug.Warning("Remote Debug: Object '{0}' (type {1}) is not serializable!", obj1, (object) type2.Name);
            this.m_failures.Add(type2);
          }
          return (object) null;
        }
      }
      return obj1;
    }

    public object Deserialize(object obj)
    {
      if (obj == null || obj.Equals((object) null))
        return (object) null;
      object obj1 = obj;
      if (obj is rdtSerializerInterface serializerInterface)
        obj1 = serializerInterface.Deserialize(this);
      if (obj1 is List<rdtTcpMessageComponents.Property> propertyList)
      {
        for (int index = 0; index < propertyList.Count; ++index)
        {
          rdtTcpMessageComponents.Property property = propertyList[index];
          property.Deserialise(this);
          propertyList[index] = property;
        }
      }
      return obj1;
    }

    private void AddField(
      List<rdtTcpMessageComponents.Property> allFields,
      string name,
      object value,
      rdtTcpMessageComponents.Property.Type type,
      RangeAttribute rangeAttribute,
      bool isArrayOrList)
    {
      object obj;
      if (rangeAttribute != null && value is float invalue)
      {
        obj = (object) new rdtSerializerSlider(invalue, rangeAttribute.min, rangeAttribute.max);
      }
      else
      {
        obj = this.Serialize(value);
        if (value != null && (obj == null || obj.Equals((object) null)))
          return;
      }
      allFields.Add(new rdtTcpMessageComponents.Property()
      {
        m_isArray = obj is rdtSerializerContainerArray | isArrayOrList,
        m_name = name,
        m_value = obj,
        m_type = type
      });
    }

    private bool CanAddMember(object owner, MemberInfo memberInfo, System.Type memberType)
    {
      int num1 = memberInfo.IsDefined(typeof (ObsoleteAttribute), false) ? 1 : 0;
      bool flag1 = memberInfo.IsDefined(typeof (HideInInspector), false);
      bool flag2 = memberType.IsSubclassOf(typeof (UnityEngine.Component));
      int num2 = flag1 ? 1 : 0;
      return (num1 | num2 | (flag2 ? 1 : 0)) == 0;
    }

    public List<rdtTcpMessageComponents.Property> ReadAllFields(
      object owner)
    {
      System.Type type = owner.GetType();
      string name1 = type.Name;
      if (this.m_skipTypes.Contains(name1))
        return (List<rdtTcpMessageComponents.Property>) null;
      List<rdtTcpMessageComponents.Property> allFields = new List<rdtTcpMessageComponents.Property>();
      PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
      if (!(owner is MonoBehaviour) && !this.m_dontReadProperties.Contains(name1))
      {
        for (int index = 0; index < properties.Length; ++index)
        {
          PropertyInfo propertyInfo = properties[index];
          if (propertyInfo.CanRead && propertyInfo.CanWrite && (propertyInfo.GetIndexParameters().Length == 0 && !propertyInfo.PropertyType.IsEnum))
          {
            string name2 = propertyInfo.Name;
            bool flag = !this.HasIncludePerType(name1);
            if ((flag || this.IncludeMember(name1, name2)) && (!flag || !this.SkipMember(name1, name2)))
            {
              MethodInfo getMethod = propertyInfo.GetGetMethod();
              if ((object) getMethod != null && getMethod.IsPublic)
              {
                MethodInfo setMethod = propertyInfo.GetSetMethod();
                if ((object) setMethod != null && setMethod.IsPublic && this.CanAddMember(owner, (MemberInfo) propertyInfo, propertyInfo.PropertyType))
                {
                  RangeAttribute rangeAttribute = (RangeAttribute) null;
                  object obj = propertyInfo.GetValue(owner, (object[]) null);
                  System.Type propertyType = propertyInfo.PropertyType;
                  bool isArrayOrList = propertyType.IsGenericList() || propertyType.IsArray;
                  if (isArrayOrList)
                  {
                    System.Type listElementType = propertyType.GetListElementType();
                    if (listElementType.IsSubclassOf(typeof (UnityEngine.Component)) || !listElementType.IsSerializable && !listElementType.IsUserStruct())
                      continue;
                  }
                  this.AddField(allFields, name2, obj, rdtTcpMessageComponents.Property.Type.Property, rangeAttribute, isArrayOrList);
                }
              }
            }
          }
        }
      }
      foreach (FieldInfo fieldInfo in type.GetAllFields().ToArray())
      {
        bool flag1 = fieldInfo.IsDefined(typeof (SerializeField), false);
        RangeAttribute rangeAttribute = (RangeAttribute) null;
        if ((fieldInfo.IsPublic || flag1) && !fieldInfo.FieldType.IsEnum)
        {
          string name2 = fieldInfo.Name;
          bool flag2 = !this.HasIncludePerType(name1);
          if ((flag2 || this.IncludeMember(name1, name2)) && (!flag2 || !this.SkipMember(name1, name2)) && this.CanAddMember(owner, (MemberInfo) fieldInfo, fieldInfo.FieldType))
          {
            object obj = fieldInfo.GetValue(owner);
            System.Type fieldType = fieldInfo.FieldType;
            bool isArrayOrList = fieldType.IsGenericList() || fieldType.IsArray;
            if (isArrayOrList)
            {
              System.Type listElementType = fieldType.GetListElementType();
              if (listElementType.IsSubclassOf(typeof (UnityEngine.Component)) || !listElementType.IsSerializable && !listElementType.IsUserStruct())
                continue;
            }
            this.AddField(allFields, name2, obj, rdtTcpMessageComponents.Property.Type.Field, rangeAttribute, isArrayOrList);
          }
        }
      }
      foreach (MethodInfo method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public))
      {
        if (method.IsDefined(typeof (ButtonAttribute), false))
          allFields.Add(new rdtTcpMessageComponents.Property()
          {
            m_name = method.Name,
            m_value = (object) new rdtSerializerButton(false),
            m_type = rdtTcpMessageComponents.Property.Type.Method
          });
      }
      return allFields;
    }

    private object MakeNewList(IList oldValue, System.Type listType, int arraySize)
    {
      System.Type listElementType = listType.GetListElementType();
      object obj;
      if (oldValue == null)
      {
        IList list;
        if (listType.IsArray)
        {
          list = (IList) Array.CreateInstance(listElementType, arraySize);
        }
        else
        {
          list = (IList) typeof (List<>).MakeGenericType(listElementType).GetConstructor(System.Type.EmptyTypes).Invoke((object[]) null);
          for (int index = 0; index < arraySize; ++index)
            list.Add((object) null);
        }
        for (int index = 0; index < arraySize; ++index)
        {
          object instance = Activator.CreateInstance(listElementType);
          list[index] = instance;
        }
        obj = (object) list;
      }
      else if (listType.IsArray)
      {
        Array sourceArray = oldValue as Array;
        Array instance1 = Array.CreateInstance(listElementType, arraySize);
        Array.Copy(sourceArray, instance1, Mathf.Min(arraySize, sourceArray.Length));
        for (int length = sourceArray.Length; length < arraySize; ++length)
        {
          object instance2 = Activator.CreateInstance(listElementType);
          instance1.SetValue(instance2, length);
        }
        obj = (object) instance1;
      }
      else
      {
        IList list = oldValue;
        if (arraySize < list.Count)
        {
          int num = list.Count - arraySize;
          for (int index = 0; index < num; ++index)
            list.RemoveAt(list.Count - 1);
        }
        else if (arraySize > list.Count)
        {
          int num = arraySize - list.Count;
          for (int index = 0; index < num; ++index)
          {
            object instance = Activator.CreateInstance(listElementType);
            list.Add(instance);
          }
        }
        obj = (object) list;
      }
      return obj;
    }

public void SetArraySize(
      object owner,
      List<rdtTcpMessageComponents.Property> allFields,
      int arraySize)
    {
      if (arraySize < 0)
        return;
      List<rdtTcpMessageComponents.Property> allFields1 = allFields;
      rdtTcpMessageComponents.Property property1 = allFields1[0];
      System.Type type = owner.GetType();
      if (!property1.m_isArray && !(property1.m_value == allFields1))
        rdtDebug.Error((object) this, "Expected to find a list of properties at {0}, but found {1} while trying to set array size", (object) property1.m_name, property1.m_value != null ? (object) property1.m_value.GetType().Name : (object) "<null>");
      else if (property1.m_type == rdtTcpMessageComponents.Property.Type.Property)
      {
        PropertyInfo property2 = type.GetProperty(property1.m_name);
        if ((object) property2 == null)
          return;
        if (property1.m_isArray)
        {
          System.Type propertyType = property2.PropertyType;
          object obj = this.MakeNewList(property2.GetValue(owner, (object[]) null) as IList, propertyType, arraySize);
          property2.SetValue(owner, obj, (object[]) null);
        }
        else
        {
          object owner1 = property2.GetValue(owner, (object[]) null);
          this.SetArraySize(owner1, allFields1, arraySize);
          property2.SetValue(owner, owner1, (object[]) null);
        }
      }
      else if (property1.m_type == rdtTcpMessageComponents.Property.Type.Field)
      {
        FieldInfo field = type.GetField(property1.m_name);
        if ((object) field == null)
          return;
        if (property1.m_isArray)
        {
          System.Type fieldType = field.FieldType;
          object obj = this.MakeNewList(field.GetValue(owner) as IList, fieldType, arraySize);
          field.SetValue(owner, obj);
        }
        else
        {
          object owner1 = field.GetValue(owner);
          this.SetArraySize(owner1, allFields1, arraySize);
          field.SetValue(owner, owner1);
        }
      }
      else
        rdtDebug.Error((object) this, "Unexpected property type {0} when setting array size on {1}", (object) property1.m_type.ToString(), (object) property1.m_name);
    }

    public void WriteAllFields(
      object realOwner,
      List<rdtTcpMessageComponents.Property> allFields,
      int arrayIndex = -1)
    {
      bool flag = false;
      object obj1 = realOwner;
      System.Type type1 = obj1.GetType();
      if ((type1.IsArray || type1.IsGenericList()) && arrayIndex != -1)
      {
        flag = true;
        obj1 = ((IList) obj1)[arrayIndex];
      }
      rdtDebug.Debug((object) this, nameof (WriteAllFields));
      System.Type type2 = obj1.GetType();
      for (int index = 0; index < allFields.Count; ++index)
      {
        rdtTcpMessageComponents.Property allField = allFields[index];
        object obj2 = this.Deserialize(allField.m_value);
        if (obj2 is rdtSerializerSlider)
          obj2 = (object) ((rdtSerializerSlider) obj2).Value;
        if (allField.m_type == rdtTcpMessageComponents.Property.Type.Property)
        {
          PropertyInfo property = type2.GetProperty(allField.m_name);
          if ((object) property != null)
          {
            if (allField.m_value is List<rdtTcpMessageComponents.Property> allFields1)
            {
              object realOwner1 = property.GetValue(obj1, (object[]) null);
              this.WriteAllFields(realOwner1, allFields1, arrayIndex);
              property.SetValue(obj1, realOwner1, (object[]) null);
            }
            else
            {
              try
              {
                rdtDebug.Debug((object) this, "Setting property {0} to {1}", (object) allField.m_name, (object) obj2.ToString());
                property.SetValue(obj1, obj2, (object[]) null);
              }
              catch (Exception ex)
              {
                rdtDebug.Warning((object) this, "Property '{0}' could not be set: {1}!", (object) allField.m_name, (object) ex.Message);
              }
            }
          }
        }
        else if (allField.m_type == rdtTcpMessageComponents.Property.Type.Field)
        {
          FieldInfo fieldInHierarchy = type2.GetFieldInHierarchy(allField.m_name);
          if ((object) fieldInHierarchy != null)
          {
            if (allField.m_value is List<rdtTcpMessageComponents.Property> allFields1)
            {
              object realOwner1 = fieldInHierarchy.GetValue(obj1);
              this.WriteAllFields(realOwner1, allFields1, arrayIndex);
              fieldInHierarchy.SetValue(obj1, realOwner1);
            }
            else
            {
              try
              {
                rdtDebug.Debug((object) this, "Setting field {0} to {1}", (object) allField.m_name, (object) obj2.ToString());
                fieldInHierarchy.SetValue(obj1, obj2);
              }
              catch (ArgumentException ex)
              {
                rdtDebug.Error((object) this, "'{0}' could not be assigned: {1}!", (object) allField.m_name, (object) ex.Message);
              }
            }
          }
        }
        else
        {
          MethodInfo method = type2.GetMethod(allField.m_name);
          if ((object) method != null)
          {
            try
            {
              if (((rdtSerializerButton) allField.m_value).Pressed)
                method.Invoke(obj1, (object[]) null);
            }
            catch (Exception ex)
            {
              string str = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
              string fmt = ex.InnerException != null ? ex.InnerException.StackTrace : ex.StackTrace;
              rdtDebug.Error("RemoteDebugServer: Method '{0}' failed: {1}", (object) allField.m_name, (object) str);
              rdtDebug.Error(fmt);
            }
          }
        }
      }
      if (!flag)
        return;
      ((IList) realOwner)[arrayIndex] = obj1;
    }

    private void InitSkipProperties()
    {
      this.m_skipTypes.Add("ParticleSystemRenderer");
      this.m_skipProperties.Add("hideFlags");
      this.m_skipProperties.Add("useGUILayout");
      this.m_skipProperties.Add("tag");
      this.m_skipProperties.Add("name");
      this.m_skipProperties.Add("enabled");
      this.m_skipProperties.Add("m_CachedPtr");
      this.m_skipProperties.Add("m_InstanceID");
      this.AddSkipForType("Rigidbody2D", "position", "rotation", "freezeRotation");
      this.AddSkipForType("Rigidbody", "position", "rotation", "freezeRotation", "useConeFriction");
      string[] strArray1 = new string[4]
      {
        "material",
        "sharedMaterial",
        "density",
        "sharedMesh"
      };
      this.AddSkipForType("BoxCollider", strArray1);
      this.AddSkipForType("BoxCollider2D", strArray1);
      this.AddSkipForType("CircleCollider2D", strArray1);
      this.AddSkipForType("SphereCollider", strArray1);
      this.AddSkipForType("PolygonCollider2D", strArray1);
      this.AddSkipForType("MeshCollider", strArray1);
      this.AddSkipForType("CapsuleCollider", strArray1);
      this.AddSkipForType("EdgeCollider2D", strArray1);
      this.AddSkipForType("WheelCollider", strArray1);
      this.AddSkipForType("TerrainCollider", strArray1);
      this.AddSkipForType("TerrainCollider", "isTrigger", "terrainData");
      this.AddSkipForType("CharacterController", strArray1);
      this.AddSkipForType("CharacterController", "isTrigger", "contactOffset");
      this.AddSkipForType("Cloth", "capsuleColliders", "sphereColliders", "solverFrequency", "useContinuousCollision", "useVirtualParticles");
      string[] strArray2 = new string[3]
      {
        "breakForce",
        "breakTorque",
        "connectedBody"
      };
      this.AddSkipForType("HingeJoint2D", strArray2);
      this.AddSkipForType("FixedJoint2D", strArray2);
      this.AddSkipForType("SpringJoint2D", strArray2);
      this.AddSkipForType("DistanceJoint2D", strArray2);
      this.AddSkipForType("FrictionJoint2D", strArray2);
      this.AddSkipForType("RelativeJoint2D", strArray2);
      this.AddSkipForType("SliderJoint2D", strArray2);
      this.AddSkipForType("WheelJoint2D", strArray2);
      this.AddSkipForType("TargetJoint2D", "enableCollision");
      this.AddSkipForType("TargetJoint2D", strArray2);
      string[] strArray3 = new string[1]{ "connectedBody" };
      this.AddSkipForType("CharacterJoint", strArray3);
      this.AddSkipForType("ConfigurableJoint", strArray3);
      this.AddSkipForType("FixedJoint", strArray3);
      this.AddSkipForType("HingeJoint", strArray3);
      this.AddSkipForType("SpringJoint", strArray3);
      this.AddSkipForType("ReflectionProbe", "bakedTexture", "customBakedTexture");
      this.AddSkipForType("Skybox", "material");
      this.AddSkipForType("NavMeshAgent", "velocity", "nextPosition");
      this.AddSkipForType("AudioSource", "clip", "outputAudioMixerGroup");
      this.AddSkipForType("AudioLowPassFilter", "customCutoffCurve");
      this.AddSkipForType("AudioReverbZone", "reverbDelay", "reflectionsDelay");
      this.AddSkipForType("LensFlare", "flare");
      this.AddSkipForType("Projector", "material");
      this.AddSkipForType("EventSystem", "m_FirstSelected", "firstSelectedGameObject");
      this.AddSkipForType("EventTrigger", "m_Delegates");
      this.AddSkipForType("Canvas", "worldCamera");
      this.AddSkipForType("TouchInputModule", "forceModuleActive");
      this.AddSkipForType("Light", "flare", "cookie");
      string[] strArray4 = new string[2]
      {
        "m_Padding",
        "padding"
      };
      this.AddSkipForType("GridLayoutGroup", strArray4);
      this.AddSkipForType("HorizontalLayoutGroup", strArray4);
      this.AddSkipForType("VerticalLayoutGroup", strArray4);
      this.AddSkipForType("TextMesh", "font");
      this.AddSkipForType("Animation", "clip");
      this.AddSkipForType("Animator", "runtimeAnimatorController", "avatar", "bodyPosition", "bodyRotation", "playbackTime");
      this.AddSkipForType("NetworkView", "observed", "viewID");
      this.AddSkipForType("Terrain", "terrainData", "materialTemplate");
      this.AddSkipForType("NavMeshAgent", "path");
      this.AddSkipForType("OffMeshLink", "startTransform", "endTransform");
      string[] strArray5 = new string[9]
      {
        "m_SpawnPrefabs",
        "m_ConnectionConfig",
        "m_GlobalConfig",
        "m_Channels",
        "m_PlayerPrefab",
        "client",
        "matchInfo",
        "matchMaker",
        "matches"
      };
      this.AddSkipForType("NetworkManager", strArray5);
      this.AddSkipForType("NetworkLobbyManager", strArray5);
      this.AddSkipForType("NetworkLobbyManager", "m_LobbyPlayerPrefab", "m_GamePlayerPrefab");
      this.AddSkipForType("NetworkTransform", "m_ClientMoveCallback3D", "m_ClientMoveCallback2D");
      this.AddSkipForType("NetworkTransformVisualizer", "m_VisualizerPrefab");
      this.AddSkipForType("GUIText", "material", "font");
      this.AddSkipForType("GUITexture", "texture", "border");
      string[] strArray6 = new string[30]
      {
        "m_OnClick",
        "m_TargetGraphic",
        "m_AnimationTriggers",
        "m_SpriteState",
        "m_OnCullStateChanged",
        "m_Template",
        "m_CaptionText",
        "m_CaptionImage",
        "m_Options",
        "m_OnValueChanged",
        "m_ItemText",
        "m_ItemImage",
        "m_Sprite",
        "m_Material",
        "m_TextComponent",
        "m_Placeholder",
        "m_OnEndEdit",
        "m_OnValidateInput",
        "m_Texture",
        "m_HandleRect",
        "m_FontData",
        "m_Group",
        "m_AsteriskChar",
        "m_FillRect",
        "onValueChanged",
        "graphic",
        "m_HorizontalScrollbar",
        "m_Content",
        "m_VerticalScrollbar",
        "m_Viewport"
      };
      this.AddSkipForType("Navigation", "m_SelectOnUp", "m_SelectOnDown", "m_SelectOnLeft", "m_SelectOnRight");
      this.AddSkipForType("Button", strArray6);
      this.AddSkipForType("Dropdown", strArray6);
      this.AddSkipForType("Image", strArray6);
      this.AddSkipForType("InputField", strArray6);
      this.AddSkipForType("RawImage", strArray6);
      this.AddSkipForType("Scrollbar", strArray6);
      this.AddSkipForType("ScrollRect", strArray6);
      this.AddSkipForType("Selectable", strArray6);
      this.AddSkipForType("Slider", strArray6);
      this.AddSkipForType("Text", strArray6);
      this.AddSkipForType("Toggle", strArray6);
      this.AddDontReadProperties("ColorBlock");
      this.AddDontReadProperties("Navigation");
      string[] strArray7 = new string[3]
      {
        "localPosition",
        "localEulerAngles",
        "localScale"
      };
      this.AddIncludeForType("Transform", strArray7);
      string[] strArray8 = new string[6]
      {
        "anchoredPosition",
        "anchorMax",
        "anchorMin",
        "offsetMax",
        "offsetMin",
        "pivot"
      };
      this.AddIncludeForType("RectTransform", strArray7);
      this.AddIncludeForType("RectTransform", strArray8);
      string[] strArray9 = new string[4]
      {
        "shadowCastingMode",
        "receiveShadows",
        "useLightProbes",
        "reflectionProbeUsage"
      };
      this.AddIncludeForType("MeshRenderer", strArray9);
      this.AddIncludeForType("SpriteRenderer", strArray9);
      this.AddIncludeForType("SpriteRenderer", "color", "flipX", "flipY");
      string[] strArray10 = new string[11]
      {
        "alignment",
        "cameraVelocityScale",
        "lengthScale",
        "maxParticleSize",
        "minParticleSize",
        "normalDirection",
        "pivot",
        "renderMode",
        "sortingFudge",
        "sortMode",
        "velocityScale"
      };
      this.AddIncludeForType("ParticleSystemRenderer", strArray9);
      this.AddIncludeForType("ParticleSystemRenderer", strArray10);
      this.AddIncludeForType("TrailRenderer", strArray9);
      this.AddIncludeForType("TrailRenderer", "autodestruct", "endWidth", "startWidth", "time");
      this.AddIncludeForType("SkinnedMeshRenderer", strArray9);
      this.AddIncludeForType("SkinnedMeshRenderer", "quality", "updateWhenOffscreen", "localBounds");
      this.AddIncludeForType("LineRenderer", strArray9);
      this.AddIncludeForType("LineRenderer", "useWorldSpace");
      this.AddIncludeForType("BillboardRenderer", strArray9);
      this.AddIncludeForType("Camera", "clearFlags", "backgroundColor", "cullingMask", "orthographic", "orthographicSize", "fov", "nearClipPlane", "farClipPlane", "rect", "depth", "renderingPath", "useOcclusionCulling", "hdr", "targetDisplay");
      this.AddIncludeForType("MeshFilter");
      this.AddIncludeForType("NetworkAnimator");
      this.AddIncludeForType("NetworkIdentity", "m_ServerOnly", "m_LocalPlayerAuthority");
      this.AddIncludeForType("CanvasRenderer");
    }

    private void AddDontReadProperties(string typeName)
    {
      this.m_dontReadProperties.Add(typeName);
    }

    private void AddSkipForType(string typeName, params string[] properties)
    {
      HashSet<string> stringSet;
      if (!this.m_skipPropertiesPerType.TryGetValue(typeName, out stringSet))
      {
        stringSet = new HashSet<string>();
        this.m_skipPropertiesPerType.Add(typeName, stringSet);
      }
      stringSet.UnionWith((IEnumerable<string>) properties);
    }

    private void AddIncludeForType(string typeName, params string[] properties)
    {
      HashSet<string> stringSet;
      if (!this.m_includePropertiesPerType.TryGetValue(typeName, out stringSet))
      {
        stringSet = new HashSet<string>();
        this.m_includePropertiesPerType.Add(typeName, stringSet);
      }
      stringSet.UnionWith((IEnumerable<string>) properties);
    }

    private bool HasIncludePerType(string ownerTypeName)
    {
      return this.m_includePropertiesPerType.ContainsKey(ownerTypeName);
    }

    private bool IncludeMember(string ownerTypeName, string memberInfoName)
    {
      HashSet<string> stringSet = (HashSet<string>) null;
      return !this.m_includePropertiesPerType.TryGetValue(ownerTypeName, out stringSet) || stringSet.Contains(memberInfoName);
    }

    private bool SkipMember(string ownerTypeName, string memberInfoName)
    {
      if (this.m_skipProperties.Contains(memberInfoName))
        return true;
      HashSet<string> stringSet = (HashSet<string>) null;
      return this.m_skipPropertiesPerType.TryGetValue(ownerTypeName, out stringSet) && stringSet.Contains(memberInfoName);
    }

    private delegate object ConvertObjectDelegate(object objIn, rdtSerializerRegistry registry);
  }
}
