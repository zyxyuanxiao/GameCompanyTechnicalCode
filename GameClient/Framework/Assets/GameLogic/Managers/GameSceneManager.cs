using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// 场景的设计模式
/// 1:GameManager场景为Root场景,其他场景都是附加在上上面的场景
/// </summary>
public partial class GameSceneManager : IManager, IUpdate
{

    public void Awake()
    {
        SceneManager.sceneLoaded -= MonitoringSceneLoad;
        SceneManager.sceneLoaded += MonitoringSceneLoad;
    }

    public void Start()
    {
        Load(Scene_GameManager, (SceneLoadingType type) =>
        {
            Debug.Log("场景加载回调" + type);
        });
    }

    public void OnDestroy()
    {

    }

    public void Update()
    {
        UpdateStatus();
    }
}
