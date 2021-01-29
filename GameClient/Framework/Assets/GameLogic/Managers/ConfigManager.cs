using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public sealed class ConfigManager  : IManager
{
    public static GameConfig GameConfig;
    public long InstanceId { get; set; }

    public void Awake()
    {
        ConfigManager.GameConfig = Resources.Load<GameConfig>("Configs/GameConfig");
    }

    public void Start()
    {
        //Excel 的配置文件
        GDataCPPRelated.Started = false;
        GDataCPPRelated.OnGameStart();
    }

    public void OnDestroy()
    {

    }

    public void Update()
    {

    }
}
