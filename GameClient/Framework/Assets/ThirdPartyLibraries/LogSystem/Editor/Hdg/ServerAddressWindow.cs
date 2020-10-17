using System;
using UnityEditor;
using UnityEngine;

namespace LogSystem
{
    public class ServerAddressWindow : EditorWindow
    {
        private string m_address;

        public Action<string> Callback { get; set; }

        private void OnGUI()
        {
            EditorGUI.DrawRect(new Rect(0.0f, 0.0f, this.maxSize.x, this.maxSize.y), (Color) (EditorGUIUtility.isProSkin ? new Color32((byte) 99, (byte) 99, (byte) 99, byte.MaxValue) : new Color32((byte) 130, (byte) 130, (byte) 130, byte.MaxValue)));
            EditorGUI.DrawRect(new Rect(1f, 1f, this.maxSize.x - 2f, this.maxSize.y - 2f), (Color) (EditorGUIUtility.isProSkin ? new Color32((byte) 49, (byte) 49, (byte) 49, byte.MaxValue) : new Color32((byte) 193, (byte) 193, (byte) 193, byte.MaxValue)));
            GUILayout.Space(8f);
            this.m_address = EditorGUILayout.TextField("Server IP:port", this.m_address);
            GUILayout.Space(4f);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Cancel", GUILayout.Width(100f)))
                this.Close(false);
            if (GUILayout.Button("Connect", GUILayout.Width(100f)))
                this.Close(true);
            EditorGUILayout.EndHorizontal();
        }

        private void Close(bool connect)
        {
            if (this.Callback != null & connect)
                this.Callback(this.m_address);
            this.Close();
        }
    }
}