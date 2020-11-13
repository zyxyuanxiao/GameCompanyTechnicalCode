using UnityEngine;

public class HotUpdateManager : IManager
{
    public void Awake()
    {

    }

    public void Start()
    {
        GameManager.QueryManager<ProcessManager>().AddExecute(TaskProcessLayer.HotUpdate, hotUpdateEnd);
    }

    public void OnDestroy()
    {

    }

    public void Update()
    {

    }

    
    private void hotUpdateEnd()
    {
        Debug.Log("<color=green>热更流程结束</color>");
    }
}
