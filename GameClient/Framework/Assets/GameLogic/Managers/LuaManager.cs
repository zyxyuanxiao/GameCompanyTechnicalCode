using System;
using System.IO;
using LuaInterface;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 不再使用 LuaSocket,后面会将 ToLua src runtime中的 luasocket 也会逐渐删除
/// </summary>
public sealed class LuaManager : IManager, IUpdate,IFixedUpdate,ILateUpdate
{
    private LuaState luaState;
    
    private LuaBeatEvent UpdateEvent { get; set; }
    
    private LuaBeatEvent LateUpdateEvent { get; set; }

    private LuaBeatEvent FixedUpdateEvent { get; set; }
    
    public void Awake()
    {
        
    }

    public void Start()
    {
        //初始化 LuaState
        luaState = new LuaState();
        //打开初始化库
        AddLibs();
        //设置空栈
        luaState.LuaSetTop(0);
        //绑定 C# 对象
        LuaBinder.Bind(luaState);
        //注册 C# 代理
        DelegateFactory.Init();   
        //注册 C# 协程
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
        FixedUpdateEvent.Dispose();
        FixedUpdateEvent = null;
        UpdateEvent.Dispose();
        UpdateEvent = null;
        LateUpdateEvent.Dispose();
        LateUpdateEvent = null;
        luaState.Dispose();
        luaState = null;
    }

    public void FixedUpdate()
    {
        if (luaState.LuaFixedUpdate(Time.fixedDeltaTime) != 0)
        {
            AddThrowException();
        }
        luaState.LuaPop(1);
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
    
    public void LateUpdate()
    {
        if (luaState.LuaLateUpdate() != 0)
        {
            AddThrowException();
        }
        luaState.StepCollect();
        luaState.LuaPop(1);
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
        LuaTable table = luaState.GetTable("FixedUpdateBeat");
        if (table == null)throw new LuaException("Lua table UpdateBeat not exists");
        FixedUpdateEvent = new LuaBeatEvent(table);
        table.Dispose();
        
        table = luaState.GetTable("UpdateBeat");
        if (table == null)throw new LuaException("Lua table UpdateBeat not exists");
        UpdateEvent = new LuaBeatEvent(table);
        table.Dispose();
        
        table = luaState.GetTable("LateUpdateBeat");
        if (table == null)throw new LuaException("Lua table UpdateBeat not exists");
        LateUpdateEvent = new LuaBeatEvent(table);
        table.Dispose();
        table = null;
    }

    //C# 调用 C,再调用 Main.lua
    private void AddCallMainLua()
    {
        luaState.DoFile("Main.lua");
        LuaFunction mainLuaFunction = luaState.GetFunction("Main.Main");
        mainLuaFunction.Call();
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
    
    //socket,这个地方使用 tcp,可以用来搞纯业务代码
    private void OpenSocket()
    {
        luaState.BeginPreLoad();
        luaState.RegFunction("socket.core", LuaOpen_Socket_Core);
        luaState.RegFunction("mime.core", LuaOpen_Mime_Core);                
        luaState.EndPreLoad();                     
    }
    
    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    private static int LuaOpen_Socket_Core(IntPtr L)
    {        
        return LuaDLL.luaopen_socket_core(L);
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    private static int LuaOpen_Mime_Core(IntPtr L)
    {
        return LuaDLL.luaopen_mime_core(L);
    }
    #endregion
}
