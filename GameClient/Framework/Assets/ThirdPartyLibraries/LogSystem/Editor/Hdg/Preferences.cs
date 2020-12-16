using UnityEditor;
using UnityEngine;

namespace LogSystem
{
    public static class Preferences
    {
        //修改时,需要使用 SettingsProvider 进行替换
        [PreferenceItem("Remote Debug")]
        public static void OnGUI()
        {
            EditorGUILayout.Space();
            int num = EditorGUILayout.IntField("Server broadcast port", EditorPrefs.GetInt("Hdg.RemoteDebug.BroadcastPort", 12000));
            if (GUI.changed)
            {
                EditorPrefs.SetInt("Hdg.RemoteDebug.BroadcastPort", num);
                if ((bool)(Object)ConnectionWindow.Instance)
                    ConnectionWindow.Instance.RestartServerEnumerator();
            }
            bool flag = EditorGUILayout.Toggle("Debug mode", EditorPrefs.GetBool("Hdg.RemoteDebug.Debug", false));
            if (!GUI.changed)
                return;
            EditorPrefs.SetBool("Hdg.RemoteDebug.Debug", flag);
            rdtDebug.s_logLevel = flag ? rdtDebug.LogLevel.Debug : rdtDebug.LogLevel.Info;
        }
    }
}
