using System;
using System.IO;
using LuaInterface;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class LuaManager : IUpdateManager
{
    private LuaState luaState;
    
    private LuaBeatEvent UpdateEvent { get; set; }
    
    
    
    public void Awake()
    {
        
    }

    public void Start()
    {
        //Excel 的配置文件
        GDataCPPRelated.Started = false;
        GDataCPPRelated.OnGameStart();
        //初始化 LuaState
        luaState = new LuaState();
        //打开初始化库
        AddLibs();
        //设置空栈
        luaState.LuaSetTop(0);
        //绑定 C# 对象
        LuaBinder.Bind(luaState);
        DelegateFactory.Init();   
        LuaCoroutine.Register(luaState, GameManager.Instance);        
        //启动 Lua
        luaState.Start();
        //给 Lua 添加 Update 方法
        AddLuaUpdate();
        //Call Lua Function
        AddCallMainLua();
        //场景重新加载的时候,刷新一下数据
        SceneManager.sceneLoaded += UpdateOnSceneLoaded;
    }

    public void OnDestroy()
    {
        SceneManager.sceneLoaded -= UpdateOnSceneLoaded;
        luaState.Call("Main.OnApplicationQuit", false);
        luaState.Dispose();
        UpdateEvent.Dispose();
        UpdateEvent = null;
        luaState = null;
    }

    public void Update()
    {
        if (luaState.LuaUpdate(Time.deltaTime, Time.unscaledDeltaTime) != 0)
        {
            AddThrowException();
        }
        luaState.LuaPop(1);
        luaState.Collect();
#if UNITY_EDITOR
        luaState.CheckTop();
#endif
    }

    
    //注册库函数到 lua 虚拟机中
    private void AddLibs()
    {
        //打开 pb,struct,lpeg 等库
        luaState.OpenLibs(LuaDLL.luaopen_pb);
        luaState.OpenLibs(LuaDLL.luaopen_struct);
        luaState.OpenLibs(LuaDLL.luaopen_lpeg);
    }
    
    //给 Lua 添加 Update 方法
    private void AddLuaUpdate()
    {
        LuaTable table = luaState.GetTable("UpdateBeat");
        if (table == null)throw new LuaException("Lua table UpdateBeat not exists");
        UpdateEvent = new LuaBeatEvent(table);
        table.Dispose();
        table = null;
    }

    //C# 调用 C,再调用 Main.lua
    private void AddCallMainLua()
    {
        luaState.DoFile("Main.lua");
        LuaFunction mainLuaFunction = luaState.GetFunction("Main.Main");
        mainLuaFunction.BeginPCall();
        mainLuaFunction.PCall();
        mainLuaFunction.EndPCall();
        mainLuaFunction.Dispose();
        mainLuaFunction = null;
    }
    
    
    //错误输出
    private void AddThrowException()
    {
        string error = luaState.LuaToString(-1);
        luaState.LuaPop(2);                
        throw new LuaException(error, LuaException.GetLastError());
    }
    
    void UpdateOnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        luaState.RefreshDelegateMap();
    }
    
    //返回LuaState对象,以供其他地方也可以使用到
    public static LuaState QueryMainState()
    {
        return GameManager.QueryManager<LuaManager>().luaState;
    }

    #region Open Lua cjson

    
    //cjson 比较特殊，只new了一个table，没有注册库，这里注册一下
    private void OpenCJson()
    {
        luaState.LuaGetField(LuaIndexes.LUA_REGISTRYINDEX, "_LOADED");
        luaState.OpenLibs(LuaDLL.luaopen_cjson);
        luaState.LuaSetField(-2, "cjson");

        luaState.OpenLibs(LuaDLL.luaopen_cjson_safe);
        luaState.LuaSetField(-2, "cjson.safe");                               
    }
    
    #endregion
    
}
