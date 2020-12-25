using System;
using UnityEngine;
using SM = UnityEngine.SceneManagement.SceneManager;


/// <summary>
/// 唯一游戏启动,退出,生命周期的管理器
/// </summary>
public sealed class GameManager : BaseManager
{
    #region 静态变量

    public static GameManager Instance;

    #endregion

    #region 生命周期

    protected override void Awake()
    {
        BestHTTP.HTTPManager.Setup();
        Instance = this;
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
        //其他服务启动完毕之后,开始启动整个游戏的流程,需要流程管理者进行第一步游戏加载,启动一个全局协程
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    protected override void Update()
    {
        base.Update();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
        Quit();
    }

    public void Quit()
    {
#if UNITY_EDITOR
        DestroyImmediate(this.gameObject, true);
        UnityEditor.EditorApplication.isPlaying = false;
        Debug.Log("<color=red>Game Quit</color>");
#else
        Application.Quit();
#endif
    }

    #endregion

    #region 初始化运行逻辑
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeOnBeforeSceneLoad()
    {
#if UNITY_EDITOR
        Debug.Log("<color=green>Game start</color>");
#endif
        GameObject go = GameObject.Find("GameManager");

        if (go != null) Instance = go.GetComponent<GameManager>();

        if (Instance == null)
        {
            go = new GameObject("GameManager");
            Instance = go.AddComponent<GameManager>();
        }
        //游戏开启后,有2个未显示在Hierarchy的游戏物体,一个是GameManager,一个是HTTP Update Delegator
        DontDestroyOnLoad(go);
    }
    #endregion
}
