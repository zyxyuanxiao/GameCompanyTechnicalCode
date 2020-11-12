using UnityEngine;

public class HotUpdateManager : IManager
{
    public void Awake()
    {

    }

    public void Start()
    {
        GameManager.QueryManager<ProcessManager>().AddExecute(TaskProcessLayer.HotUpdate, () =>
        {
            Debug.Log("<color=green>热更流程结束</color>");
        });
    }

    public void OnDestroy()
    {

    }

    public void Update()
    {

    }
}
