using UnityEditor;

namespace LogSystem
{
    public class LogEditor : Editor
    {
        [MenuItem("Log/GameState")]
        public static void ShowWindow()
        {
            // 复刻 http://www.horsedrawngames.com
            EditorWindow.GetWindow<ConnectionWindow>(false, "Game State", true);
        }
    }
}