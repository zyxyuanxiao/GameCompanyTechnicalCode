using Common;
using DLCAssets;
using LitJson;
using UnityEngine;

/// <summary>
/// 资源管理,管理所有的资源,包括 AssetBundle 的加载卸载
/// </summary>
public sealed class AssetManager : IManager
{
    public long InstanceId { get; set; }
    public void Awake()
    {
        AssetsNotification.AssetsMessageReceived += AssetsMessageReceived;
    }

    public void Start()
    {

    }

    public void OnDestroy()
    {
        AssetsNotification.AssetsMessageReceived -= AssetsMessageReceived;
    }


    private void AssetsMessageReceived(IAssetsNotificationType notificationType, string json)
    {
        // if (notificationType != IAssetsNotificationType.Info) return;
        JsonData jsonData = JsonMapper.ToObject(json);
        Debug.Log(notificationType.ToString() + "    " + jsonData["message"]);
    }
    
    
}
