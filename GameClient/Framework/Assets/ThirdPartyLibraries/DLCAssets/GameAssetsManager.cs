using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssetsManager
{
    private static GameAssetsManager s_instance;
    public static GameAssetsManager Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = new GameAssetsManager();
            }
            return s_instance;
        }
    }
    
    
    
}
