using System.Collections;
using UnityEngine;


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
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        //其他服务启动完毕之后,开始启动整个游戏的流程,需要流程管理者进行第一步游戏加载,启动一个全局协程
    }

    // Update is called once per frame
    protected override void Update()
    {
        // yield return OneFrame;
        base.Update();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
    }

    #endregion

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    #region 初始化运行逻辑

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeOnBeforeSceneLoad()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
        GameObject go = GameObject.Find("GameManager");

        if (go != null) Instance = go.GetComponent<GameManager>();

        if (Instance == null)
        {
            go = new GameObject("GameManager")
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            Instance = go.AddComponent<GameManager>();
        }
        DontDestroyOnLoad(go);//游戏开启后,有2个未显示在Hierarchy的游戏物体,一个是GameManager,一个是HTTP Update Delegator
    }

#if UNITY_EDITOR
    private static void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange playMode)
    {
        if (playMode == UnityEditor.PlayModeStateChange.EnteredPlayMode)
        {
            Debug.Log("<color=green>Game start</color>");
        }
        else if (playMode == UnityEditor.PlayModeStateChange.EnteredEditMode)
        {
            Debug.Log("<color=red>Game Quit</color>");
            GameObject go = GameObject.Find("GameManager");
            DestroyImmediate(go, true);
        }
    }
#endif
    #endregion
}
