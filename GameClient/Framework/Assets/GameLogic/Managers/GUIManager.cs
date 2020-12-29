using Common;
using DLCAssets;
using UnityEngine;

public sealed class GUIManager : IManager
{
    public GameObject UIRoot { get; private set; }
    public void Awake()
    {
        GameObject uiRes = Resources.Load<GameObject>("UI/UIRoot"); //这个不是实例化的GameObject,只能作为资源使用
        UIRoot = GameObject.Instantiate(uiRes, Vector3.zero, Quaternion.identity);
        GameObject.DontDestroyOnLoad(UIRoot);
        AssetsNotification.AssetsMessageReceived += AssetsMessageReceived;
    }

    public void Start()
    {
        
    }

    public void OnDestroy()
    {
        AssetsNotification.AssetsMessageReceived -= AssetsMessageReceived;
    }

    public void Update()
    {

    }
    
    private void AssetsMessageReceived(IAssetsNotificationType notificationType, string messageInfo)
    {
        if (notificationType == IAssetsNotificationType.None) return;
        //UI 展示弹窗,弹出资源加载,下载,网络,等一系列问题
    }
}
