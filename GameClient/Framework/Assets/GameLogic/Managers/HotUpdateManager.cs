using UnityEngine;

public class HotUpdateManager : IManager
{
    public void Awake()
    {

    }

    public void Start()
    {
        GameManager.QueryManager<ProcessManager>().AddExecuteTasks(ProcessLayer.HotUpdate);
    }

    public void OnDestroy()
    {

    }

    public void Update()
    {

    }
}
