using UnityEditor;
using UnityEditor.SceneManagement;

public class SceneEditor : Editor
{
    [MenuItem("Scenes/OpenScene/GameManager")]
    private static void OpenGameManager()
    {
        OpenScene("Assets/Resources/Scenes/GameManager.unity");
    }

    private static void OpenScene(string name)
    {
        if (EditorApplication.isPlaying) return;
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene(name);
    }
}
