using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityEditorInternals
{
    public static class GUI
    {
        private static MethodInfo s_toolbarSearchField;

        public static string ToolbarSearchField(string text, params GUILayoutOption[] options)
        {
            if ((object) GUI.s_toolbarSearchField == null)
                GUI.s_toolbarSearchField = typeof (EditorGUILayout).GetMethod(nameof (ToolbarSearchField), BindingFlags.NonPublic | BindingFlags.Static, (Binder) null, new System.Type[2]
                                                                                                                                                                        {
                                                                                                                                                                            typeof (string),
                                                                                                                                                                            typeof (GUILayoutOption[])
                                                                                                                                                                        }, (ParameterModifier[]) null);
            object[] parameters = new object[2]
                                  {
                                      (object) text,
                                      (object) options
                                  };
            string str = "";
            if ((object) GUI.s_toolbarSearchField != null)
                str = (string) GUI.s_toolbarSearchField.Invoke((object) null, parameters);
            return str;
        }
    }
}