using UnityEditor;

namespace LogSystem
{
    public class LogEditor : Editor
    {
        [MenuItem("Tools/Log/GameState")]
        public static void ShowWindow()
        {
            // 复刻 http://www.horsedrawngames.com
            EditorWindow.GetWindow<ConnectionWindow>(false, "Game State", true);
        }
    }
}