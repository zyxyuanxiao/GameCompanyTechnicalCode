using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class ConfigManager  : IManager
{
    public static GameConfig GameConfig;
    
    public void Awake()
    {
        ConfigManager.GameConfig = Resources.Load<GameConfig>("Configs/GameConfig");
    }

    public void Start()
    {
        
    }

    public void OnDestroy()
    {

    }

    public void Update()
    {

    }
}
