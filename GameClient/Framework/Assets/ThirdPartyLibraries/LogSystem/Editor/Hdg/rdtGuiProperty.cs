using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LogSystem
{
  public class rdtGuiProperty
  {
    private rdtExpandedCache m_expandedCache;
    private rdtGuiProperty.ComponentValueChangedHandler m_componentValueChangedHandler;
    private Stack<rdtTcpMessageComponents.Property> m_currentHierarchy;
    private rdtTcpMessageComponents.Component m_currentComponent;

    private void DrawList(
      string label,
      IList list,
      ref bool foldout,
      rdtGuiProperty.ValueChangedHandler onValueChanged,
      string foldoutKey)
    {
      EditorGUILayout.BeginVertical();
      GUILayout.Label("");
      Rect lastRect = GUILayoutUtility.GetLastRect();
      foldout = EditorGUI.Foldout(lastRect, foldout, label, true);
      if (foldout)
      {
        ++EditorGUI.indentLevel;
        int num1 = list != null ? list.Count : 0;
        System.Type type = list != null ? list.GetType().GetListElementType() : (System.Type) null;
        bool isUserStruct = (object) type == (object) typeof (List<rdtTcpMessageComponents.Property>) || (object) type == null;
        if (list is List<rdtTcpMessageComponents.Property> properties)
        {
          this.DrawComponent(properties, foldoutKey, onValueChanged);
        }
        else
        {
          EditorGUILayout.BeginHorizontal();
          this.Space();
          int num2 = EditorGUILayout.IntField("Size", num1);
          EditorGUILayout.EndHorizontal();
          if (num1 != num2)
          {
            rdtGuiProperty.ValueChangedEvent valueChangedEvent = new rdtGuiProperty.ValueChangedEvent()
            {
              NewArraySize = num2
            };
            onValueChanged(valueChangedEvent);
          }
          for (int i = 0; i < num1; i++)
          {
            rdtGuiProperty.ValueChangedHandler onValueChanged1 = (rdtGuiProperty.ValueChangedHandler) (valueChangedEvent =>
            {
              if (isUserStruct)
              {
                valueChangedEvent.UpdateProperty = true;
                valueChangedEvent.ArrayIndex = i;
                onValueChanged(valueChangedEvent);
              }
              else
              {
                list[i] = valueChangedEvent.NewValue;
                valueChangedEvent.OldValue = (object) null;
                valueChangedEvent.NewValue = (object) null;
                valueChangedEvent.UpdateProperty = false;
                onValueChanged(valueChangedEvent);
              }
            });
            this.Draw("Element " + (object) i, list[i], onValueChanged1, foldoutKey + ">Element" + (object) i, false);
          }
        }
        --EditorGUI.indentLevel;
      }
      EditorGUILayout.EndVertical();
    }

    private Matrix4x4 DrawMatrix(string label, Matrix4x4 matrix, ref bool foldout)
    {
      Matrix4x4 matrix4x4 = matrix;
      EditorGUILayout.BeginVertical();
      GUILayout.Label("");
      Rect lastRect = GUILayoutUtility.GetLastRect();
      foldout = EditorGUI.Foldout(lastRect, foldout, label, true);
      if (foldout)
      {
        ++EditorGUI.indentLevel;
        matrix4x4.m00 = this.DrawFloat("E00", matrix4x4.m00);
        matrix4x4.m01 = this.DrawFloat("E01", matrix4x4.m01);
        matrix4x4.m02 = this.DrawFloat("E02", matrix4x4.m02);
        matrix4x4.m03 = this.DrawFloat("E03", matrix4x4.m03);
        matrix4x4.m10 = this.DrawFloat("E10", matrix4x4.m10);
        matrix4x4.m11 = this.DrawFloat("E11", matrix4x4.m11);
        matrix4x4.m12 = this.DrawFloat("E12", matrix4x4.m12);
        matrix4x4.m13 = this.DrawFloat("E13", matrix4x4.m13);
        matrix4x4.m20 = this.DrawFloat("E20", matrix4x4.m20);
        matrix4x4.m21 = this.DrawFloat("E21", matrix4x4.m21);
        matrix4x4.m22 = this.DrawFloat("E22", matrix4x4.m22);
        matrix4x4.m23 = this.DrawFloat("E23", matrix4x4.m23);
        matrix4x4.m30 = this.DrawFloat("E30", matrix4x4.m30);
        matrix4x4.m31 = this.DrawFloat("E31", matrix4x4.m31);
        matrix4x4.m32 = this.DrawFloat("E32", matrix4x4.m32);
        matrix4x4.m33 = this.DrawFloat("E33", matrix4x4.m33);
        --EditorGUI.indentLevel;
      }
      EditorGUILayout.EndVertical();
      return matrix4x4;
    }

    public rdtGuiProperty(
      rdtGuiProperty.ComponentValueChangedHandler componentValueChangedHandler)
    {
      this.m_expandedCache = new rdtExpandedCache();
      this.m_componentValueChangedHandler = componentValueChangedHandler;
    }

    public void DrawComponent(
      int gameObjInstanceId,
      rdtTcpMessageComponents.Component component,
      List<rdtTcpMessageComponents.Property> properties)
    {
      this.m_currentHierarchy = new Stack<rdtTcpMessageComponents.Property>();
      this.m_currentComponent = component;
      string foldoutKeyInit = string.Format("{0}>{1}({2})", (object) gameObjInstanceId.ToString(), (object) component.m_name, (object) component.m_instanceId);
      this.DrawComponent(properties, foldoutKeyInit, (rdtGuiProperty.ValueChangedHandler) null);
    }

    private void DrawComponent(
      List<rdtTcpMessageComponents.Property> properties,
      string foldoutKeyInit = "",
      rdtGuiProperty.ValueChangedHandler valueChangedHandler = null)
    {
      if (properties == null || properties.Count == 0)
        return;
      foreach (rdtTcpMessageComponents.Property property in properties)
      {
        object propValue = property.m_value;
        if (propValue != null || property.m_isArray)
        {
          EditorGUILayout.BeginHorizontal();
          string str1 = ObjectNames.NicifyVariableName(property.m_name);
          System.Type type1 = propValue?.GetType();
          this.m_currentHierarchy.Push(property);
          string str2 = property.m_name;
          if (!string.IsNullOrEmpty(foldoutKeyInit))
            str2 = foldoutKeyInit + ">" + str2;
          System.Type type2 = typeof (List<rdtTcpMessageComponents.Property>);
          if ((object) type1 == (object) type2)
          {
            EditorGUILayout.BeginVertical();
            GUILayout.Label("");
            bool expanded = EditorGUI.Foldout(GUILayoutUtility.GetLastRect(), this.m_expandedCache.IsExpanded(this.m_currentComponent, str2), str1, true);
            if (expanded)
            {
              ++EditorGUI.indentLevel;
              this.DrawComponent((List<rdtTcpMessageComponents.Property>) propValue, str2, valueChangedHandler);
              --EditorGUI.indentLevel;
            }
            this.m_expandedCache.SetExpanded(expanded, this.m_currentComponent, str2);
            EditorGUILayout.EndVertical();
          }
          else
          {
            rdtGuiProperty.ValueChangedHandler onValueChanged = valueChangedHandler ?? (rdtGuiProperty.ValueChangedHandler) (valueChangedEvent =>
            {
              valueChangedEvent.Component = this.m_currentComponent;
              valueChangedEvent.Hierarchy = this.m_currentHierarchy;
              this.m_componentValueChangedHandler(valueChangedEvent);
            });
            this.Draw(str1, propValue, onValueChanged, str2, property.m_isArray);
          }
          this.m_currentHierarchy.Pop();
          EditorGUILayout.EndHorizontal();
        }
      }
    }

    private void Draw(
      string propName,
      object propValue,
      rdtGuiProperty.ValueChangedHandler onValueChanged,
      string foldoutKey,
      bool isArray = false)
    {
      bool foldout = this.m_expandedCache.IsExpanded(this.m_currentComponent, foldoutKey);
      if (propValue == null && !isArray)
        return;
      System.Type type = propValue?.GetType();
      bool flag1 = false;
      if ((object) type != null && !type.IsArray && (!type.IsGenericList() && (object) type != (object) typeof (Vector4)) && ((object) type != (object) typeof (Matrix4x4) && (object) type != (object) typeof (Quaternion)))
      {
        flag1 = true;
        EditorGUILayout.BeginHorizontal();
        this.Space();
      }
      if ((object) type == (object) typeof (float))
      {
        float num1 = (float) propValue;
        float num2 = EditorGUILayout.FloatField(propName, num1);
        if (!num1.Equals(num2))
        {
          rdtGuiProperty.ValueChangedEvent valueChangedEvent = new rdtGuiProperty.ValueChangedEvent((object) num1, (object) num2, true, -1);
          onValueChanged(valueChangedEvent);
        }
      }
      else if ((object) type == (object) typeof (double))
      {
        double num1 = (double) propValue;
        double num2 = EditorGUILayout.DoubleField(propName, num1);
        if (!num1.Equals(num2))
        {
          rdtGuiProperty.ValueChangedEvent valueChangedEvent = new rdtGuiProperty.ValueChangedEvent((object) num1, (object) num2, true, -1);
          onValueChanged(valueChangedEvent);
        }
      }
      else if ((object) type == (object) typeof (int))
      {
        int num1 = (int) propValue;
        int num2 = EditorGUILayout.IntField(propName, num1);
        if (!num1.Equals(num2))
        {
          rdtGuiProperty.ValueChangedEvent valueChangedEvent = new rdtGuiProperty.ValueChangedEvent((object) num1, (object) num2, true, -1);
          onValueChanged(valueChangedEvent);
        }
      }
      else if ((object) type == (object) typeof (uint))
      {
        int num1 = (int) (uint) propValue;
        int num2 = EditorGUILayout.IntField(propName, num1);
        if (!num1.Equals(num2))
        {
          rdtGuiProperty.ValueChangedEvent valueChangedEvent = new rdtGuiProperty.ValueChangedEvent((object) num1, (object) (uint) num2, true, -1);
          onValueChanged(valueChangedEvent);
        }
      }
      else if ((object) type == (object) typeof (Vector2))
      {
        Vector2 vector2_1 = (Vector2) propValue;
        Vector2 vector2_2 = EditorGUILayout.Vector2Field(propName, vector2_1);
        if (!vector2_1.Equals((object) vector2_2))
        {
          rdtGuiProperty.ValueChangedEvent valueChangedEvent = new rdtGuiProperty.ValueChangedEvent((object) vector2_1, (object) vector2_2, true, -1);
          onValueChanged(valueChangedEvent);
        }
      }
      else if ((object) type == (object) typeof (Vector3))
      {
        Vector3 vector3_1 = (Vector3) propValue;
        Vector3 vector3_2 = EditorGUILayout.Vector3Field(propName, vector3_1);
        if (vector3_1 != vector3_2)
        {
          rdtGuiProperty.ValueChangedEvent valueChangedEvent = new rdtGuiProperty.ValueChangedEvent((object) vector3_1, (object) vector3_2, true, -1);
          onValueChanged(valueChangedEvent);
        }
      }
      else if ((object) type == (object) typeof (Vector4))
      {
        Vector4 vector4_1 = (Vector4) propValue;
        Vector4 vector4_2 = this.DrawVector4(propName, vector4_1, ref foldout);
        if (!vector4_1.Equals((object) vector4_2))
        {
          rdtGuiProperty.ValueChangedEvent valueChangedEvent = new rdtGuiProperty.ValueChangedEvent((object) vector4_1, (object) vector4_2, true, -1);
          onValueChanged(valueChangedEvent);
        }
      }
      else if ((object) type == (object) typeof (Matrix4x4))
      {
        Matrix4x4 matrix = (Matrix4x4) propValue;
        Matrix4x4 matrix4x4 = this.DrawMatrix(propName, matrix, ref foldout);
        if (!matrix.Equals((object) matrix4x4))
        {
          rdtGuiProperty.ValueChangedEvent valueChangedEvent = new rdtGuiProperty.ValueChangedEvent((object) matrix, (object) matrix4x4, true, -1);
          onValueChanged(valueChangedEvent);
        }
      }
      else if ((object) type == (object) typeof (bool))
      {
        bool flag2 = (bool) propValue;
        bool flag3 = EditorGUILayout.Toggle(propName, flag2);
        if (!flag2.Equals(flag3))
        {
          rdtGuiProperty.ValueChangedEvent valueChangedEvent = new rdtGuiProperty.ValueChangedEvent((object) flag2, (object) flag3, true, -1);
          onValueChanged(valueChangedEvent);
        }
      }
      else if ((object) type != null && type.IsEnum)
      {
        Enum selected = (Enum) propValue;
        Enum @enum = EditorGUILayout.EnumPopup(propName, selected);
        if (!selected.Equals((object) @enum))
        {
          rdtGuiProperty.ValueChangedEvent valueChangedEvent = new rdtGuiProperty.ValueChangedEvent((object) selected, (object) @enum, true, -1);
          onValueChanged(valueChangedEvent);
        }
      }
      else if ((object) type == (object) typeof (char))
      {
        string text = new string((char) propValue, 1);
        string str = EditorGUILayout.TextField(propName, text);
        if (!text.Equals(str))
        {
          rdtGuiProperty.ValueChangedEvent valueChangedEvent = new rdtGuiProperty.ValueChangedEvent((object) text, (object) (char) (str.Length > 0 ? (int) str[0] : (int) (char) propValue), true, -1);
          onValueChanged(valueChangedEvent);
        }
      }
      else if ((object) type == (object) typeof (string))
      {
        string text = (string) propValue;
        string str = EditorGUILayout.TextField(propName, text);
        if (!text.Equals(str))
        {
          rdtGuiProperty.ValueChangedEvent valueChangedEvent = new rdtGuiProperty.ValueChangedEvent((object) text, (object) str, true, -1);
          onValueChanged(valueChangedEvent);
        }
      }
      else if ((object) type == (object) typeof (Color))
      {
        Color color1 = (Color) propValue;
        Color color2 = EditorGUILayout.ColorField(propName, color1);
        if (!color1.Equals((object) color2))
        {
          rdtGuiProperty.ValueChangedEvent valueChangedEvent = new rdtGuiProperty.ValueChangedEvent((object) color1, (object) color2, true, -1);
          onValueChanged(valueChangedEvent);
        }
      }
      else if ((object) type == (object) typeof (Color32))
      {
        Color32 color32_1 = (Color32) propValue;
        Color32 color32_2 = (Color32) EditorGUILayout.ColorField(propName, (Color) color32_1);
        if (!color32_1.Equals((object) color32_2))
        {
          rdtGuiProperty.ValueChangedEvent valueChangedEvent = new rdtGuiProperty.ValueChangedEvent((object) color32_1, (object) color32_2, true, -1);
          onValueChanged(valueChangedEvent);
        }
      }
      else if ((object) type == (object) typeof (Quaternion))
      {
        Quaternion quaternion1 = (Quaternion) propValue;
        Vector4 vector4_1 = new Vector4(quaternion1.x, quaternion1.y, quaternion1.z, quaternion1.w);
        Vector4 vector4_2 = this.DrawVector4(propName, vector4_1, ref foldout);
        Quaternion quaternion2 = new Quaternion(vector4_2.x, vector4_2.y, vector4_2.z, vector4_2.w);
        if (!quaternion1.Equals((object) quaternion2))
        {
          rdtGuiProperty.ValueChangedEvent valueChangedEvent = new rdtGuiProperty.ValueChangedEvent((object) quaternion1, (object) quaternion2, true, -1);
          onValueChanged(valueChangedEvent);
        }
      }
      else if ((object) type == (object) typeof (Bounds))
      {
        Bounds bounds1 = (Bounds) propValue;
        Bounds bounds2 = EditorGUILayout.BoundsField(propName, bounds1);
        if (!bounds1.Equals((object) bounds2))
        {
          rdtGuiProperty.ValueChangedEvent valueChangedEvent = new rdtGuiProperty.ValueChangedEvent((object) bounds1, (object) bounds2, true, -1);
          onValueChanged(valueChangedEvent);
        }
      }
      else if ((object) type == (object) typeof (Rect))
      {
        Rect rect1 = (Rect) propValue;
        Rect rect2 = EditorGUILayout.RectField(propName, rect1);
        if (!rect1.Equals((object) rect2))
        {
          rdtGuiProperty.ValueChangedEvent valueChangedEvent = new rdtGuiProperty.ValueChangedEvent((object) rect1, (object) rect2, true, -1);
          onValueChanged(valueChangedEvent);
        }
      }
      else if ((object) type == null & isArray || type.IsArray)
      {
        Array array = (Array) propValue;
        this.DrawList(propName, (IList) array, ref foldout, onValueChanged, foldoutKey);
      }
      else if ((object) type != null && type.IsGenericList())
      {
        IList list = (IList) propValue;
        this.DrawList(propName, list, ref foldout, onValueChanged, foldoutKey);
      }
      else if ((object) type == (object) typeof (rdtSerializerButton))
      {
        Vector2 vector2 = GUI.skin.button.CalcSize(new GUIContent(propName));
        if (GUILayout.Button(propName, GUILayout.Width(vector2.x + 40f)))
        {
          rdtGuiProperty.ValueChangedEvent valueChangedEvent = new rdtGuiProperty.ValueChangedEvent((object) false, (object) new rdtSerializerButton(true), true, -1);
          onValueChanged(valueChangedEvent);
        }
      }
      else if ((object) type == (object) typeof (rdtSerializerSlider))
      {
        rdtSerializerSlider serializerSlider1 = (rdtSerializerSlider) propValue;
        float invalue = EditorGUILayout.Slider(propName, serializerSlider1.Value, serializerSlider1.LimitMin, serializerSlider1.LimitMax);
        if (!serializerSlider1.Value.Equals(invalue))
        {
          rdtSerializerSlider serializerSlider2 = new rdtSerializerSlider(invalue, serializerSlider1.LimitMin, serializerSlider1.LimitMax);
          rdtGuiProperty.ValueChangedEvent valueChangedEvent = new rdtGuiProperty.ValueChangedEvent((object) serializerSlider1, (object) serializerSlider2, true, -1);
          onValueChanged(valueChangedEvent);
        }
      }
      else
        rdtDebug.Debug("rdtGuiProperty: Unknown type: " + ((object) type != null ? type.Name : "<null>") + " (name=" + propName + ", value=" + propValue + ")");
      if (flag1)
        EditorGUILayout.EndHorizontal();
      this.m_expandedCache.SetExpanded(foldout, this.m_currentComponent, foldoutKey);
    }

    private void Space()
    {
      GUILayout.Space((float) (EditorStyles.foldout.padding.left + EditorStyles.foldout.margin.left - EditorStyles.label.padding.left));
    }

    private float DrawFloat(string label, float value)
    {
      EditorGUILayout.BeginHorizontal();
      this.Space();
      value = EditorGUILayout.FloatField(label, value);
      EditorGUILayout.EndHorizontal();
      return value;
    }

    private Vector4 DrawVector4(string label, Vector4 value, ref bool foldout)
    {
      EditorGUILayout.BeginVertical();
      GUILayout.Label("");
      Rect lastRect = GUILayoutUtility.GetLastRect();
      foldout = EditorGUI.Foldout(lastRect, foldout, label, true);
      if (foldout)
      {
        ++EditorGUI.indentLevel;
        value.x = this.DrawFloat("X", value.x);
        value.y = this.DrawFloat("Y", value.y);
        value.z = this.DrawFloat("Z", value.z);
        value.w = this.DrawFloat("W", value.w);
        --EditorGUI.indentLevel;
      }
      EditorGUILayout.EndVertical();
      return value;
    }

    public class ValueChangedEvent
    {
      public ValueChangedEvent()
      {
        this.NewArraySize = -1;
        this.ArrayIndex = -1;
      }

      public ValueChangedEvent(int arrayIndex)
      {
        this.ArrayIndex = arrayIndex;
        this.NewArraySize = -1;
      }

      public ValueChangedEvent(
        object oldValue,
        object newValue,
        bool updateProperty,
        int arrayIndex)
      {
        this.OldValue = oldValue;
        this.NewValue = newValue;
        this.UpdateProperty = updateProperty;
        this.ArrayIndex = arrayIndex;
        this.NewArraySize = -1;
      }

      public rdtTcpMessageComponents.Component Component { get; set; }

      public Stack<rdtTcpMessageComponents.Property> Hierarchy { get; set; }

      public object OldValue { get; set; }

      public object NewValue { get; set; }

      public bool UpdateProperty { get; set; }

      public int ArrayIndex { get; set; }

      public int NewArraySize { get; set; }
    }

    public delegate void ValueChangedHandler(rdtGuiProperty.ValueChangedEvent valueChangedEvent);

    public delegate void ComponentValueChangedHandler(
      rdtGuiProperty.ValueChangedEvent valueChangedEvent);
  }
}
