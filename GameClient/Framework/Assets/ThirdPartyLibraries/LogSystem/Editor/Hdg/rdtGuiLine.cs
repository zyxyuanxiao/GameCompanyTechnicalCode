using UnityEditor;
using UnityEngine;

namespace LogSystem
{
    public static class rdtGuiLine
    {
        public static void DrawHorizontalLine()
        {
            Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.MaxHeight(1f), GUILayout.ExpandWidth(true));
            Color color1 = GUI.color;
            GUI.color = Color.white;
            Color color2 = EditorGUIUtility.isProSkin ? new Color(0.2784314f, 0.2784314f, 0.2784314f, 1f) : new Color(0.3647059f, 0.3647059f, 0.3647059f, (float) byte.MaxValue);
            EditorGUI.DrawRect(rect, color2);
            GUI.color = color1;
        }
    }
}