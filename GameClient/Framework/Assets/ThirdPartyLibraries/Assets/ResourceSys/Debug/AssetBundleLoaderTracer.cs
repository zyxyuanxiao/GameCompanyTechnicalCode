using UnityEngine;
using System.Collections.Generic;
using System.IO;


public class AssetBundleLoaderTracer 
{
    private static AssetBundleLoaderTracer _inst = new AssetBundleLoaderTracer();

    private bool m_enable = false;
    public bool Enable
    {
        get
        {
            return m_enable;
        }
        set
        {
            m_enable = value;
            Debug.LogWarning("AssetBundleLoaderTracer set state to " + value);
        }
    }

    public static AssetBundleLoaderTracer Instance
    {
        get
        {
            return _inst;
        }
    }

    class LoadingInfoEntry
    {
        public const int TIME_SCALE_FACTOR = 100000;
        public enum LIFECYCLE
        {
            INIT = 0,

            AB_LOADING ,
            AB_LOADED,

            ASSET_LOADING,
            ASSET_LOADED,

            AB_UNLOADED,
            ASSET_UNLOADED,
        }

        public LoadingInfoEntry(string _aburl)
        {
            ABURL = _aburl;
            Stage = LIFECYCLE.INIT;
        }

        public string ABURL{ get; set; }
        public LIFECYCLE Stage { get; private set; }

        public bool ABLoadSync { get; private set; }
        public float ABLoadStart { get; private set; }
        public float ABLoadFinished { get; private set; }
        public bool BundleLoadSuccess { get; private set; }

        public bool AssetLoadSync { get; private set; }
        public float AssetLoadStart { get; private set; }
        public float AssetLoadFinished { get; private set; }
        public float ABUnloaded { get; private set; }
        public float AssetUnloaded { get; private set; }

        public bool DanglingAsset { get; private set; }

        public string StackTrace;


        public bool AssetNotDestroyOnLoad { get; set; }

        public void OnABLoadStart(bool syncing)
        {
            ABLoadStart = Time.realtimeSinceStartup * TIME_SCALE_FACTOR;
            Stage = LIFECYCLE.AB_LOADING;
            ABLoadSync = syncing;
            StackTrace = StackTraceUtility.ExtractStackTrace();
        }

        public void OnABLoadFinished(bool success)
        {
            ABLoadFinished = Time.realtimeSinceStartup * TIME_SCALE_FACTOR;
            Stage = LIFECYCLE.AB_LOADED;
            BundleLoadSuccess = success;
        }

        public void OnAssetLoadStart(bool syncing)
        {
            AssetLoadStart = Time.realtimeSinceStartup * TIME_SCALE_FACTOR;
            Stage = LIFECYCLE.ASSET_LOADING;
            AssetLoadSync = syncing;
        }

        public void OnAssetLoadFinished()
        {   
            AssetLoadFinished = Time.realtimeSinceStartup * TIME_SCALE_FACTOR;
            Stage = LIFECYCLE.ASSET_LOADED;
        }

        public void OnABUnloaded(bool unloadAsset)
        {
            ABUnloaded = Time.realtimeSinceStartup * TIME_SCALE_FACTOR;
            Stage = LIFECYCLE.AB_UNLOADED;
            DanglingAsset = !unloadAsset;
            if (unloadAsset)
            {
                Stage = LIFECYCLE.ASSET_UNLOADED;
                AssetUnloaded = Time.realtimeSinceStartup * TIME_SCALE_FACTOR;
            }
        }
    }

    private Dictionary<string, List<LoadingInfoEntry>> m_loadingInfo = new Dictionary<string, List<LoadingInfoEntry>>();
    private Dictionary<string, List<string>> m_loadingErrMsg = new Dictionary<string, List<string>>();

    
    public void OnTransferFromAsyncToSync(string abUrl)
    {
        
    }

    public void OnABLoadStart(string abUrl, bool syncing)
    {
        if (!m_enable)
            return;

        List<LoadingInfoEntry> infoList = FetchEntries(abUrl, true);
        LoadingInfoEntry preInfo = infoList[infoList.Count - 1];
        if(preInfo.Stage == LoadingInfoEntry.LIFECYCLE.ASSET_UNLOADED)
        {
            var info = new LoadingInfoEntry(abUrl);
            info.OnABLoadStart(syncing);
            infoList.Add(info);
        }
        else
        {
            if(preInfo.Stage == LoadingInfoEntry.LIFECYCLE.AB_UNLOADED)
            {
                if(!preInfo.DanglingAsset)
                    LogError(abUrl, "ab unloaded while asset not dangling??");

                var info = new LoadingInfoEntry(abUrl);
                info.OnABLoadStart(syncing);
                infoList.Add(info);
            }
            else
            {
                LogError(abUrl, "Load new ab while same previous ab still not unloaded??");
            }
        }
    }

    public void OnABLoadFinished(string abUrl, bool success)
    {
        if (!m_enable)
            return ;

        List<LoadingInfoEntry> infoList = FetchEntries(abUrl);
        if (infoList == null)
        {
            LogError(abUrl, "ab load finished while not started???");
            return;
        }

        infoList[infoList.Count - 1].OnABLoadFinished(success);
    }

    public void OnAssetLoadStart(string abUrl, bool syncing)
    {
        if (!m_enable)
            return;

        List<LoadingInfoEntry> infoList = FetchEntries(abUrl);
        if (infoList == null)
        {
            LogError(abUrl, "asset loading start while no loading info???");
            return;
        }
        infoList[infoList.Count - 1].OnAssetLoadStart(syncing);
    }

    public void OnAssetLoadFinished(string abUrl)
    {
        if (!m_enable)
            return;

        List<LoadingInfoEntry> infoList = FetchEntries(abUrl);
        if (infoList == null)
        {
            LogError(abUrl, "asset loading finished while no loading info???");
            return;
        }
        infoList[infoList.Count - 1].OnAssetLoadFinished();
    }

    public void OnABUnloaded(string abUrl, bool unloadAsset)
    {
        if (!m_enable)
            return;

        List<LoadingInfoEntry> infoList = FetchEntries(abUrl);
        if (infoList == null)
        {
            LogError(abUrl, "ab unload while no loading info??");
            return;
        }
        infoList[infoList.Count - 1].OnABUnloaded(unloadAsset);
    }


    public static void Log()
    {
        using (StreamWriter sw = File.CreateText(Application.persistentDataPath + "/ab_load_tracing.log"))
        {
            foreach (var kv in Instance.m_loadingInfo)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.AppendFormat("AB: {0} \n", kv.Key);
                for(int i = 0; i < kv.Value.Count; i++)
                {
                    var info = kv.Value[i];
                    sb.AppendFormat(@"\t loading {0} time:\n\t\tab {1} load begin at{2}\n\t\tab load finished at {3}\n\t\tasset {4} load begin at{5}
                                    \n\t\tasset load finished at{6}\n\t\tab unload at{7}\n\t\tasset unload at{8}",
                                    i, info.ABLoadSync, info.ABLoadStart, info.ABLoadFinished, info.AssetLoadSync ,info.AssetLoadStart, 
                                    info.AssetLoadFinished, info.ABUnloaded, info.DanglingAsset ? "dangling" : info.AssetUnloaded.ToString());

                }
                sb.AppendFormat("\n\n");
                sw.Write(sb.ToString());
            }

            sw.Write("Error Message Begin\n");
            foreach(var kv in Instance.m_loadingErrMsg)
            {
                foreach(var errMsg in kv.Value)
                {
                    sw.Write(string.Format("{0}: {1}", kv.Key, errMsg));
                }
            }
        }
    }

    public static void Clear()
    {
        Instance.m_loadingInfo.Clear();
        Instance.m_loadingErrMsg.Clear();
    }

    private List<LoadingInfoEntry> FetchEntries(string abUrl, bool Create = false)
    {
        List<LoadingInfoEntry> infoList;
        if(!m_loadingInfo.TryGetValue(abUrl, out infoList) && Create)
        {
            infoList = new List<LoadingInfoEntry>();
            infoList.Add(new LoadingInfoEntry(abUrl));
            m_loadingInfo.Add(abUrl, infoList);
        }

        return infoList;
    }



    private void LogError(string abUrl, string errMsg)
    {
        List<string> errMsgs;
        if(!m_loadingErrMsg.TryGetValue(abUrl, out errMsgs))
        {
            errMsgs = new List<string>();
            m_loadingErrMsg.Add(abUrl, errMsgs);
        }
        errMsgs.Add(errMsg);
        Debug.LogError(string.Format("{0}  error: {1}", Time.realtimeSinceStartup, errMsg));
    }
}
