using UnityEditor;
using UnityEditor.SceneManagement;

public class SceneEditor : Editor
{
    [MenuItem("Scenes/OpenScene/GameStart")]
    private static void OpenGameStart()
    {
        OpenScene("Assets/Resources/Scenes/GameStart.unity");
    }

    private static void OpenScene(string name)
    {
        if (EditorApplication.isPlaying) return;
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene(name);
    }
}
