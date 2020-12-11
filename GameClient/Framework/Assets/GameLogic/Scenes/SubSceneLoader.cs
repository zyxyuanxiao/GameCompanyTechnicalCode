//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement;


//public enum SubSceneLoadingType // 以Loading场景为视角看待
//{
//    None = 0,           //初始状态,即场景加载完毕至场景卸载时的状态
//    Unloading,         //场景卸载中...
//    UnloadCompleted,  //场景卸载完毕
//    Loading,         //场景加载中...
//    LoadCompleted,  //场景加载完毕
//}

/// <summary>
/// 首先异步加载Loading场景
/// 这个时候界面上面显示 卸载中...
/// 卸载完毕之后,再加载本次真正的场景
/// 加载完毕之后,再进行重置
/// 通过判断SceneLoadingType的四种状态,来进行信息/进度条的展示
/// </summary>
//public partial class GameSceneManager
//{
//    public static string Scene_GameManager = "GameManager";//永驻场景
//    public static string Scene_Empty = "Empty";
//    public static string Scene_Loading = "Loading";

//    加载过程中的进度条
//    public float Progress;

//    场景加载类型
//    public SubSceneLoadingType SubSceneLoadingType;

//    当前激活的场景
//    public string _activeSceneName = string.Empty;

//    加载是否结束
//    private Action<SubSceneLoadingType> _aoCompleted;

//    加载下一个场景
//    public string LoadSubSceneName = string.Empty;

//    加载场景的句柄
//    private AsyncOperation _asyncOperation;

//    过渡帧,当Progress执行到1的时候,过渡10帧再执行下一个任务
//    private byte _transitionFrame;

//    需要卸载的场景
//    private List<Scene> _unLoadScene = new List<Scene>();

//    / <summary>
//    / 加载子场景
//    / </summary>
//    / <param name = "name" ></ param >
//    / < param name="action"></param>
//    public void LoadSubScene(string name, Action<SubSceneLoadingType> action = null)
//    {
//        检查场景是否已加载
//        if (SceneManager.GetSceneByName(name).isLoaded)
//        {
//            Debug.LogError(name + " 场景已加载,无需再次加载");
//            return;
//        }
//        LoadSubSceneName = name;
//        _aoCompleted = action;
//        UnLoadSubScene();//第一步,先卸载不包括GameManager的其他子场景
//    }

//    private void UnLoadSubScene()
//    {
//        for (int i = 0; i < SceneManager.sceneCount; i++)
//        {
//            Scene scene = SceneManager.GetSceneAt(i);
//            if (!scene.name.Equals(Scene_GameManager))
//            {
//                _unLoadScene.Add(scene);
//            }
//        }
//        if (_unLoadScene.Count > 0)
//        {
//            卸载
//            SubSceneLoadingType = SubSceneLoadingType.Unloading;
//        }
//        else
//        {
//            加载
//            SubSceneLoadingType = SubSceneLoadingType.Loading;
//        }
//        _aoCompleted?.Invoke(SubSceneLoadingType);//传给外部
//    }

//    / <summary>
//    / 更新场景的状态
//    / </summary>
//    private void UpdateStatus()
//    {
//        即场景加载完毕至场景卸载时的状态 不进行进度条的计算
//        if (SubSceneLoadingType == SubSceneLoadingType.None) return;

//        if (SubSceneLoadingType == SubSceneLoadingType.Unloading)
//        {
//            if (_asyncOperation == null && _unLoadScene.Count > 0)
//            {
//                Progress = 0;
//                _asyncOperation = SceneManager.UnloadSceneAsync(_unLoadScene[0]);
//            }
//            else
//            {
//                UpdateProgress();
//                if (Progress >= 1.0f && _asyncOperation.isDone)
//                {
//                    _unLoadScene?.RemoveAt(0);
//                    _asyncOperation = null;
//                    if (_unLoadScene?.Count <= 0)
//                    {
//                        SubSceneLoadingType = SubSceneLoadingType.UnloadCompleted;
//                        _aoCompleted?.Invoke(SubSceneLoadingType);
//                    }
//                }
//            }
//            return;
//        }

//        if (SubSceneLoadingType == SubSceneLoadingType.UnloadCompleted)
//        {
//            开启加载场景的任务,此时回调给外部
//            SubSceneLoadingType = SubSceneLoadingType.Loading;
//            _aoCompleted?.Invoke(SubSceneLoadingType);
//        }

//        if (SubSceneLoadingType == SubSceneLoadingType.Loading)
//        {
//            if (_asyncOperation == null)
//            {
//                Progress = 0;
//                _asyncOperation = SceneManager.LoadSceneAsync(LoadSubSceneName, LoadSceneMode.Additive);
//                _asyncOperation.allowSceneActivation = false;
//            }
//            else
//            {
//                UpdateProgress();
//                if (Progress >= 1.0f)
//                {
//                    if (_transitionFrame == 0)
//                    {
//                        SubSceneLoadingType = SubSceneLoadingType.LoadCompleted;
//                        _aoCompleted?.Invoke(SubSceneLoadingType);
//                        _asyncOperation.allowSceneActivation = true;
//                        _asyncOperation = null;
//                    }
//                    _transitionFrame++;
//                    if (_transitionFrame >= 10)
//                    {
//                        Reset();
//                    }
//                }
//            }
//        }
//    }

//    private void UpdateProgress()
//    {
//        if (Progress >= 0.9f)
//        {
//            Progress += 0.0033f;//0.1 进度,跑30帧,大概是1s左右
//            _transitionFrame = 0;
//        }
//        else
//        {
//            if (_asyncOperation != null) Progress = _asyncOperation.progress;
//        }
//    }

//    private void Reset()
//    {
//        Progress = 0;
//        LoadSubSceneName = string.Empty;
//        _asyncOperation = null;
//        _aoCompleted = null;
//        SubSceneLoadingType = SubSceneLoadingType.None;
//    }

//    private static void MonitoringSceneLoad(Scene scene, LoadSceneMode mode)
//    {
//        GameSceneManager gsm = GameManager.Instance.QueryManager<GameSceneManager>();
//        gsm._activeSceneName = scene.name;
//        SceneManager.SetActiveScene(scene);
//    }
//}
