using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using LuaInterface;
using UnityEngine;
using Best;
using Best_Pool;
using UnityObject = UnityEngine.Object;

/**
  * <summary>
  * 用于C#和lua逻辑都要用到的资源cache（两边都要使用同一个cache才能保证逻辑一致性）
  * GCTODO: 暂时没有想到更好的c#和lua共用cache的逻辑
  * </summary>
  */
public class GlobalAssetCacheMgr:ILateUpdate
{

    [NoToLua] public const string DEBUG_POOL = "debugpool";
    [NoToLua] public const string DEBUG_CACHE = "debugcache";
    [NoToLua] public const string K = "k";
    [NoToLua] public const string SHORT_CAPACITY = "shorttermcapacity";
    [NoToLua] public const string LONG_CAPACITY = "longtermcapacity";
    [NoToLua] public const string SHORT_SURVIAL_TIME = "shorttermsurvialsecond";
    [NoToLua] public const string COMBO_SHORT_SURVIAL_TIME = "comboshorttermsurvialsecond";

    public const string POOL_BOPENDEAD = "pool_dead_bopen";
    public const string POOL_DEADTIME = "pool_dead_time";
    public const string POOL_INTERVALTIME = "pool_dead_intervaltime";
    public const string POOL_DEADREMAIN = "pool_dead_remain";

    public const string POOL_BPRECREATE = "pool_pre_bcreate";
    public const string POOL_PREFRAME = "pool_pre_frame";
    public const string POOL_PREDELAY = "pool_pre_delay";
    public const string POOL_PRECREATE_AMOUNT = "pool_pre_create_amount";

    public const string POOL_LIMIT_BOPEN = "pool_limit_bopen";
    public const string POOL_LIMIT_AMOUNT = "pool_limit_amount";

    public const string POOL_CULL_BOPEN = "pool_cull_bopen";
    public const string POOL_CULL_THRESHOLD = "pool_cull_threshold";
    public const string POOL_CULL_MAXPERPASS = "pool_cull_maxperpass";
    public const string POOL_CULL_DELAY = "pool_cull_delay";

    private static GlobalAssetCacheMgr _inst;
    public static GlobalAssetCacheMgr Instance
    {
        get
        {
            if (_inst == null)
            {
                _inst = new GlobalAssetCacheMgr();
                UpdatableRunner.Instance.AddLateUpdate(_inst);
            }
            return _inst;
        }
    }

    Dictionary<string, AssetCache<UnityObject>> m_cacheDic;
    Dictionary<string, AssetCache<UnityObject>> CacheDic
    {
        get
        {
            if (m_cacheDic == null)
                m_cacheDic = new Dictionary<string, AssetCache<UnityObject>>(4);
            return m_cacheDic;
        }
    }

    Best.AssetCache<Texture2D> m_TextureCache;
    public Best.AssetCache<Texture2D> TextureCache
    {
        get
        {
            if (m_TextureCache == null)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();// CacheConfigUtility.GetValueDic("GlobalTextureCache");
                Best.LRUKCache<string, Best.AssetCache<Texture2D>.Bucket> cacheAlgo = new Best.LRUKCache<string, Best.AssetCache<Texture2D>.Bucket>
                    ("GlobalTextureCache",
                    uint.Parse(dic[K]),
                    uint.Parse(dic[SHORT_CAPACITY]),
                    uint.Parse(dic[LONG_CAPACITY]),
                    float.Parse(dic[SHORT_SURVIAL_TIME]));
                //GatherInstancePool<Texture2D> pool = Framework.GameKernel.GetPoolManager().GetGatherInstancePool<Texture2D>("GlobalTexture",
                //    (ref bool blimit, ref int amount) => {
                //        blimit = true;
                //        amount = 1;
                //    });
                //pool.IsDebug = !BuildManifestUtility.GetBuildManifest().IsReleaseVer && dic[DEBUG_POOL].Equals("true");
                m_TextureCache = new Best.AssetCache<Texture2D>("GlobalTextureCache", cacheAlgo);
                m_TextureCache.EnableDebug = !BuildManifestUtility.GetBuildManifest().IsReleaseVer && dic[DEBUG_CACHE].Equals("true");
            }
            return m_TextureCache;
        }
    }

    public void InitTextureCache(Best.LRUKCache<string, Best.AssetCache<Texture2D>.Bucket> cachestrategy)
    {
        if (m_TextureCache != null)
        {
            Debug.LogErrorFormat("The texture cache had init!");
            return;
        }
        //GatherInstancePool<Texture2D> pool = Framework.GameKernel.GetPoolManager().GetGatherInstancePool<Texture2D>("GlobalTexture", (ref bool blimit, ref int amount) =>
        //{
        //    blimit = true;
        //    amount = 1;
        //});
        m_TextureCache = new Best.AssetCache<Texture2D>("GlobalTextureCache", cachestrategy);
    }

    // Best.ObjectCache<NewEffectManager.PauseEffectInfo, GameObject> m_EffectInfoCache;
    // public Best.ObjectCache<NewEffectManager.PauseEffectInfo, GameObject> EffecetInfoCache
    // {
    //     get
    //     {
    //         if (m_EffectInfoCache == null)
    //         {
    //             Dictionary<string, string> dic = CacheConfigUtility.GetValueDic("SkillEffectCache");
    //             //Best.LRUKCache<string, Best.ObjectCache<NewEffectManager.PauseEffectInfo, GameObject>.Bucket> strategy
    //             //    = new Best.LRUKCache<string, Best.ObjectCache<NewEffectManager.PauseEffectInfo, GameObject>.Bucket>("SkillEffectCache",
    //             //    uint.Parse(dic[K]),
    //             //    uint.Parse(dic[SHORT_CAPACITY]),
    //             //    uint.Parse(dic[LONG_CAPACITY]),
    //             //    float.MaxValue);
    //             float comboShortSurvialTime = 0;
    //             float.TryParse(dic[COMBO_SHORT_SURVIAL_TIME], out comboShortSurvialTime);
    //             Best.LRUCacheWithTTL<string, Best.ObjectCache<NewEffectManager.PauseEffectInfo, GameObject>.Bucket> strategy =
    //                 new LRUCacheWithTTL<string, ObjectCache<NewEffectManager.PauseEffectInfo, GameObject>.Bucket>(
    //                     "SkillEffectCache",
    //                     uint.Parse(dic[SHORT_CAPACITY]),
    //                     float.Parse(dic[SHORT_SURVIAL_TIME]),
    //                     comboShortSurvialTime);
    //             GatherObjectPool<NewEffectManager.PauseEffectInfo> pool = Framework.GameKernel.GetPoolManager().GetGatherObjectPool<NewEffectManager.PauseEffectInfo>("SkillEffect", 
    //                 (ref bool blimit, ref int amount) => {
    //                     blimit = true;
    //                     amount = 200;
    //                 });
    //             pool.IsDebug = !BuildManifestUtility.GetBuildManifest().IsReleaseVer && dic[DEBUG_POOL].Equals("true");
    //             m_EffectInfoCache = new Best.ObjectCache<NewEffectManager.PauseEffectInfo, GameObject>(strategy, pool, "SkillEffectCache",true, comboShortSurvialTime);
    //             m_EffectInfoCache.EnableDebug = !BuildManifestUtility.GetBuildManifest().IsReleaseVer && dic[DEBUG_CACHE].Equals("true");
    //         }
    //         return m_EffectInfoCache;
    //     }
    //     set
    //     {
    //         m_EffectInfoCache.Release();
    //         m_EffectInfoCache = null;
    //     }
    // }

    // ObjectCache<MissileObject, GameObject> m_missileCache;
    // public ObjectCache<MissileObject, GameObject> MissileCache
    // {
    //     get
    //     {
            // if (m_missileCache == null)
            // {
            //     Dictionary<string, string> dic = CacheConfigUtility.GetValueDic("MissileObjectCache");
            //     //LRUKCache<string, ObjectCache<MissileObject, GameObject>.Bucket> cachestrategy = new LRUKCache<string, ObjectCache<MissileObject, GameObject>.Bucket>("MissileObjectCache", 
            //     //    uint.Parse(dic[K]), 
            //     //    uint.Parse(dic[SHORT_CAPACITY]),
            //     //    uint.Parse(dic[LONG_CAPACITY]),
            //     //    float.MaxValue);
            //     float comboShortSurvialTime = 0;
            //     float.TryParse(dic[COMBO_SHORT_SURVIAL_TIME], out comboShortSurvialTime);
            //     LRUCacheWithTTL<string, ObjectCache<MissileObject, GameObject>.Bucket> strategy = new LRUCacheWithTTL<string, ObjectCache<MissileObject, GameObject>.Bucket>(
            //         "MissileObjectCache",
            //         uint.Parse(dic[SHORT_CAPACITY]),
            //         uint.Parse(dic[SHORT_SURVIAL_TIME]),
            //         comboShortSurvialTime);
            //     GatherObjectPool<MissileObject> pool = Framework.GameKernel.GetPoolManager().GetGatherObjectPool<MissileObject>("MissileObject", 
            //         (ref bool blimit, ref int amount) => {
            //             blimit = true;
            //             amount = 10;
            //         });
            //     pool.IsDebug = !BuildManifestUtility.GetBuildManifest().IsReleaseVer && dic[DEBUG_POOL].Equals("true");             
            //     m_missileCache = new ObjectCache<MissileObject, GameObject>(strategy, pool, "MissileObjectCache",true, comboShortSurvialTime);
            //     m_missileCache.EnableDebug = !BuildManifestUtility.GetBuildManifest().IsReleaseVer && dic[DEBUG_CACHE].Equals("true");
            // }
    //         return m_missileCache;
    //     }
    //     set
    //     {
    //         m_missileCache.Release();
    //         m_missileCache = value;
    //     }
    // }

    AssetCache<GameObject> m_avatarEffectCache;
    [NoToLua]
    public AssetCache<GameObject> AvatarEffectCache
    {
        get
        {
            if (m_avatarEffectCache == null)
            {
                m_avatarEffectCache = CreateGameObjectCache("AvatarEffec");
            }
            return m_avatarEffectCache;
        }
    }

    AssetCache<GameObject> m_equipCache;
    [NoToLua]
    public AssetCache<GameObject> EquipCache
    {
        get
        {
            if (m_equipCache == null)
                m_equipCache = CreateGameObjectCache("Equip");
            return m_equipCache;
        }
    }

    public AssetCache<GameObject> CreateGameObjectCache(string tag, bool instantiable = true)
    {
        string name = tag + "Cache";
        AssetCache<GameObject> cache;
        if (GameObjectCacheDic.TryGetValue(name, out cache))
        {
            return cache;
        }
        // Dictionary<string, string> dic = CacheConfigUtility.GetValueDic(name);
        // if (dic == null)
        //     Debug.LogErrorFormat("The config of cache '{0}' is null! Please additive it to config file {1}.", name, CacheConfigUtility.FileName);
        // LRUKCache<string, AssetCache<GameObject>.Bucket> strategy = new LRUKCache<string, AssetCache<GameObject>.Bucket>(name,
        //     uint.Parse(dic[K]),
        //     uint.Parse(dic[SHORT_CAPACITY]),
        //     uint.Parse(dic[LONG_CAPACITY]),
        //     float.Parse(dic[SHORT_SURVIAL_TIME]));
        // GatherInstancePool<GameObject> pool = null;
        // if (instantiable)
        // {
        //     pool = Framework.GameKernel.GetPoolManager().GetGatherInstancePool<GameObject>(tag,
        //     (ref bool blimit, ref int amount) =>
        //     {
        //         blimit = true;
        //         amount = 10;
        //     });
        //     pool.IsDebug = !BuildManifestUtility.GetBuildManifest().IsReleaseVer && dic[DEBUG_POOL].Equals("true");
        // }
        // cache = new AssetCache<GameObject>(name, strategy, instantiable, pool);
        // cache.EnableDebug = !BuildManifestUtility.GetBuildManifest().IsReleaseVer && dic[DEBUG_CACHE].Equals("true");
        GameObjectCacheDic.Add(name, cache);
        return cache;
    }

    Dictionary<string, AssetCache<GameObject>> m_gameObjectCacheDic;
    Dictionary<string, AssetCache<GameObject>> GameObjectCacheDic
    {
        get
        {
            if (m_gameObjectCacheDic == null)
                m_gameObjectCacheDic = new Dictionary<string, AssetCache<GameObject>>();
            return m_gameObjectCacheDic;
        }
    }

#if DEVELOPMENT_BUILD || UNITY_EDITOR
#endif

    public void CacheLog(string cachename,bool debugpool)
    {
        switch (cachename)
        {
            case "GlobalTextureCache":
                TextureCache.DebugLog(debugpool);
                break;
            // case "SkillEffectCache":
            //     EffecetInfoCache.DebugLog(debugpool);
            //     break;
            // case "MissileObjectCache":
            //     // MissileCache.DebugLog(debugpool);
            //     break;
            case "AvatarEffecCache":
                AvatarEffectCache.DebugLog(debugpool);
                break;
            case "EquipCache":
                EquipCache.DebugLog(debugpool);
                break;
            case "LuaAtlasCache":
                GameObjectCacheDic[cachename].DebugLog(debugpool);
                break;
        }
    }

    public void LogAllAsset(string cachename)
    {
        switch (cachename)
        {
            case "GlobalTextureCache":
                TextureCache.LogAllAsset("贴图缓存数据");
                break;
            // case "SkillEffectCache":
            //     EffecetInfoCache.LogAllAsset("子弹特效缓存数据");
            //     break;
            // case "MissileObjectCache":
            //     // MissileCache.LogAllAsset("技能特效缓存数据");
            //     break;
            case "AvatarEffectCache":
                AvatarEffectCache.LogAllAsset("时装特效数据");
                break;
            case "EquipCache":
                EquipCache.LogAllAsset("时装特效数据");
                break;
            case "LuaAtlasCache":
                GameObjectCacheDic[cachename].LogAllAsset("贴图数据");
                break;
        }
    }

    public void ReleaseUnuseAsset(string cachename)
    {
        StopCleanCache();
        switch (cachename)
        {
            case "GlobalTextureCache":
                TextureCache.ReleaseUnuseAsset();
                break;
            // case "SkillEffectCache":
            //     EffecetInfoCache.ReleaseUnuseAsset();
            //     break;
            // case "MissileObjectCache":
            //     // MissileCache.ReleaseUnuseAsset();
            //     break;
            case "AvatarEffectCache":
                AvatarEffectCache.ReleaseUnuseAsset();
                break;
            case "EquipCache":
                EquipCache.ReleaseUnuseAsset();
                break;
            case "LuaAtlasCache":
                GameObjectCacheDic[cachename].ReleaseUnuseAsset();
                break;
        }
    }

    [NoToLua]
    public void ReleaseAllUnuseAsset()
    {
        StopCleanCache();
        TextureCache.ReleaseUnuseAsset();
        // EffecetInfoCache.ReleaseUnuseAsset();
        // MissileCache.ReleaseUnuseAsset();
        AvatarEffectCache.ReleaseUnuseAsset();
        EquipCache.ReleaseUnuseAsset();
        foreach (var keyValuePair in GameObjectCacheDic)
        {
            keyValuePair.Value.ReleaseUnuseAsset();
        }
    }

    [NoToLua]
    public bool OpenAssetCacheRevertMode { get; set; }
    public void OpenRevertInvaildAssetMode(bool bOpen)
    {
        OpenAssetCacheRevertMode = bOpen;
    }

    private enum EnumCache
    {
        GlobalTextureCache,
        EffecetInfoCache,
        MissileObjectCache,
        AvatarEffectCache,
        EquipCache,
        LuaAtlasCache,
        None,//停止
    }

    private float m_startTime;
    private int m_frameInterval;
    private int m_cleanCountOneFrame;
    private int m_frameCounter;
    private int m_cleanCounter;
    private int m_cleanOverIndex;
    private EnumCache m_cleanCacheType = EnumCache.None;
    private HashSet<string> m_cleanPathList = new HashSet<string>();

    public void StartCleanCache(int frameInterval = 1, int cleanCountOneFrame = 1)
    {
        m_cleanCacheType = EnumCache.GlobalTextureCache;
        m_startTime = Time.realtimeSinceStartup;
        m_frameInterval = frameInterval;
        m_cleanCountOneFrame = cleanCountOneFrame;
        m_cleanOverIndex = 0;
        UpdateCurrntFrameNoRefCache(TextureCache);
    }

    private void UpdateCurrntFrameNoRefCache(ICacheForceEliminated cache)
    {
        var noRefBucketDict = cache.GetClientNoRefBucket();
        m_cleanPathList.Clear();
        foreach (var item in noRefBucketDict)
        {
            if (0 < item.Value && item.Value <= m_startTime)
            {
                m_cleanPathList.Add(item.Key);
            }
        }
    }

    public void StopCleanCache()
    {
        m_cleanCacheType = EnumCache.None;
        m_startTime = 0;
        m_frameInterval = 0;
        m_cleanCountOneFrame = 0;
        m_cleanOverIndex = 0;
        m_cleanPathList.Clear();
    }

    [NoToLua]
    public void LateUpdate()
    {     
        if (m_cleanCacheType == EnumCache.None)
            return;
   
        m_frameCounter++;
        if (m_frameCounter < m_frameInterval)
        {
            return;
        }
        m_frameCounter = 0;

        ExcuteCleanProcess(m_cleanCacheType);
    }

    private void ExcuteCleanProcess(EnumCache curCacheType)
    {
        ICacheForceEliminated curCache = GetCacheByCacheEnum(m_cleanCacheType);
        if (curCache == null)
        {
            StopCleanCache();
            return;
        }
        else
        {                  
            int startIndex = m_cleanOverIndex;
            int len = m_cleanPathList.Count;
            m_cleanCounter = 0;
            if (startIndex < len)
            {
                for (int i = startIndex; i < len; i++)
                {
                    curCache.OnForceEliminated(m_cleanPathList.ElementAt(i));
                    ++m_cleanCounter;
                    ++m_cleanOverIndex;
                    if (m_cleanCounter >= m_cleanCountOneFrame)
                        return;
                }
            }
        }
  
        m_cleanOverIndex = 0;
        int curTypeIndex = (int)curCacheType;
        int nextTypeIndex = ++curTypeIndex;
        m_cleanCacheType = (EnumCache)nextTypeIndex;
        ICacheForceEliminated nextCache = GetCacheByCacheEnum(m_cleanCacheType);

        if (nextCache == null)
        {
            StopCleanCache();
            return;
        }
        else
        {
            UpdateCurrntFrameNoRefCache(nextCache);          
        }
    }

    private ICacheForceEliminated GetCacheByCacheEnum(EnumCache enumProcess)
    {
        switch (enumProcess)
        {
            case EnumCache.GlobalTextureCache:
                return TextureCache; 
            // case EnumCache.EffecetInfoCache:
            //     return EffecetInfoCache;           
            // case EnumCache.MissileObjectCache:
            //     // return MissileCache;              
            case EnumCache.AvatarEffectCache:
                return AvatarEffectCache;              
            case EnumCache.EquipCache:
                return EquipCache;              
            case EnumCache.LuaAtlasCache:
                return GameObjectCacheDic["LuaAtlasCache"];
            default:
                return null;
        }
    }
}