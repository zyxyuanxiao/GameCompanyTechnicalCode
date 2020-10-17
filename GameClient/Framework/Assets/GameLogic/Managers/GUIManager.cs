using UnityEngine;

public class GUIManager : IManager
{
    public GameObject UIRoot { get; private set; }
    public void Awake()
    {
        GameObject uiRes  = Resources.Load<GameObject>("UI/UIRoot"); //这个不是实例化的GameObject,只能作为资源使用
        UIRoot =  GameObject.Instantiate(uiRes, Vector3.zero, Quaternion.identity);
        GameObject.DontDestroyOnLoad(UIRoot);
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
