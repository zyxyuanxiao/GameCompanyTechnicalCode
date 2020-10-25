using SM = UnityEngine.SceneManagement.SceneManager;

public partial class SceneManager : IManager
{

    public void Awake()
    {
        SM.sceneLoaded -= MonitoringSceneLoad;
        SM.sceneLoaded += MonitoringSceneLoad;
        Load(SceneManager.Scene_GameStart);
    }

    public void Start()
    {

    }

    public void OnDestroy()
    {

    }

    public void Update()
    {
        UpdateProgress();
    }
}
