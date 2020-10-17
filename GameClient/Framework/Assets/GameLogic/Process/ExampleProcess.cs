using UnityEngine;

//例子
// public sealed class LoadGameStart : IProcess
// {
//     public byte ID => 0;
//     public byte layer => 0;
//     public byte DelayFrame => 1;
//     public void Work()
//     {
//         this.isDone = false;
//         SceneManager sm = GameManager.Instance.QueryManager<SceneManager>();
//         if (!string.Equals(sm.QueryName(), SceneManager.Scene_GameStart))
//         {
//             sm.Load(SceneManager.Scene_GameStart,(SceneLoadingType SceneLoadingType) =>
//                                                  {
//                                                      if (SceneLoadingType == SceneLoadingType.LoadNextCompleted)
//                                                          this.isDone = true;
//                                                  });
//         }
//         else
//         {
//             this.isDone = true;
//         }
//     }
//
//     public bool isDone { get; set; }
//     public void Reset()
//     {
//         this.isDone = false;
//     }
// }

//例子
// public sealed class LoadUIRoot : IProcess
// {
//     public byte ID => 1;
//     public byte layer => 0;
//     public byte DelayFrame => 1;
//     public void Work()
//     {
//         this.isDone     = false;
//         GameObject uiRes = Resources.Load<GameObject>("UI/UIRoot");//这个不是实例化的GameObject,只能作为资源使用
//         GameObject uiRoot =  GameObject.Instantiate(uiRes,Vector3.zero, Quaternion.identity);
//         GameObject.DontDestroyOnLoad(uiRoot);
//         GUIManager guiManager = GameManager.Instance.QueryManager<GUIManager>();
//         guiManager.UIRoot = uiRoot;
//         if (uiRoot)
//         {
//             this.isDone = true;
//         }
//     }
//
//     public bool isDone { get; set; }
//     public void Reset()
//     {
//         this.isDone     = false;
//         Resources.UnloadUnusedAssets();
//     }
// }

