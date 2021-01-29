using System;
using UnityEngine;
using SM = UnityEngine.SceneManagement.SceneManager;


/// <summary>
/// 唯一游戏启动,退出,生命周期的管理器
/// </summary>
///
/**
 * 分级概念,中央集权,热插拔模式设计
 *      一级管理者:皇帝      GameManager  永生
 *      二级管理者:中央官员   各类型的 Managers,由 GameManager 控制其生命周期
 *      三级业务员:各地官员   第三方库,大型模块,具体固定的模块,必要玩法,必须实现的模块
 *      四级协作员:村长      具体模块,小型模块组成大型模块(易推翻,模块)
 *      五级办事人:公民      具体小模块的实现人员(随时被推翻)
 * 上级严格控制下级的生命周期,不能越级控制.
 */
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
        //其他服务启动完毕之后,开始启动整个游戏的流程
        GameManager.QueryManager<CommandManager>().AddCommand(new InitSceneCommand());
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
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    private static void Main()
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
