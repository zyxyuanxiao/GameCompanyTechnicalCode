using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using SM = UnityEngine.SceneManagement.SceneManager;


public enum SceneLoadingType // 以Loading场景为视角看待
{
    None = 0,//初始状态,即场景加载完毕至场景卸载时的状态
    UnloadLast, //场景卸载中...
    UnloadLastCompleted, //场景卸载完毕
    LoadNext, //场景加载中...
    LoadNextCompleted, //场景加载完毕
}

/// <summary>
/// 首先异步加载Loading场景
/// 这个时候界面上面显示  卸载中...
/// 卸载完毕之后,再加载本次真正的场景
/// 加载完毕之后,再进行重置
/// 通过判断SceneLoadingType的四种状态,来进行信息/进度条的展示
/// </summary>
public partial class SceneManager
{
    public static string Scene_GameStart = "GameStart";
    public static string Scene_Empty = "Empty";
    public static string Scene_Loading = "Loading";

    //加载过程中的进度条
    public float Progress;

    //场景加载类型
    public SceneLoadingType SceneLoadingType;
    
    //当前激活的场景
    public string _activeSceneName = string.Empty;

    //加载是否结束
    private Action<SceneLoadingType> _aoCompleted;
    
    //加载下一个场景
    private string _cacheSceneName = string.Empty;

    //加载场景的句柄
    private AsyncOperation _asyncOperation;

    //过渡帧,当Progress执行到1的时候,过渡10帧再执行下一个任务
    private byte _transitionFrame;

    

    
    public string QueryName()
    {
        return _activeSceneName;
    }

    public void Load(string name,Action<SceneLoadingType> action = null)
    {
        _cacheSceneName = name;
        _aoCompleted = action;
        LoadName(Scene_Loading);//先加载过度场景
    }

    private void LoadName(string name)
    {
        Progress = 0;
        if (name.CompareTo(Scene_Loading) == 0)
        {
            SceneLoadingType = SceneLoadingType.UnloadLast;
        }
        else
        {
            SceneLoadingType = SceneLoadingType.LoadNext;
        }
        _aoCompleted?.Invoke(SceneLoadingType);
        _asyncOperation = SM.LoadSceneAsync(name);
        _asyncOperation.allowSceneActivation = false;
    }

    /// <summary>
    /// 更新加载场景的进度
    /// </summary>
    private void UpdateProgress()
    {
        //即场景加载完毕至场景卸载时的状态 不进行进度条的计算
        if (SceneLoadingType == SceneLoadingType.None) return;

        if (Progress >= 1.0f)//先将 Progress 加到 1.0之上,再进行下一步
        {
            Progress = 1.0f;
            _asyncOperation.allowSceneActivation = true;//加到1之后,其场景设置为激活状态
            if (SceneLoadingType == SceneLoadingType.UnloadLast)
            {//改变卸载中的状态为卸载完毕
                SceneLoadingType = SceneLoadingType.UnloadLastCompleted;
            }
            else if (SceneLoadingType == SceneLoadingType.UnloadLastCompleted)
            {//卸载完毕之后,加载下一个真正使用的场景
                _transitionFrame++;
                if (_transitionFrame > 10)
                {
                    _aoCompleted?.Invoke(SceneLoadingType.UnloadLastCompleted);
                    LoadName(_cacheSceneName);
                }
            }
            else if (SceneLoadingType == SceneLoadingType.LoadNext)
            {//加载完毕,进度条>=1,将状态改成加载完毕
                SceneLoadingType = SceneLoadingType.LoadNextCompleted;
            }
            else if (SceneLoadingType == SceneLoadingType.LoadNextCompleted)
            {//加载完毕之后,将状态进行重置,这个地方添加过渡帧,是为了防止外部在同一帧上面无法获取到进度条
                _transitionFrame++;
                if (_transitionFrame > 10)
                {
                    _aoCompleted?.Invoke(SceneLoadingType.LoadNextCompleted);
                    Reset();
                }
            }
        }
        else
        {//当场景加载到0.9之上,则Unity已经将场景加载完毕了,此时不将场景进行激活,并进行过渡帧操作,直到进度条加到1之上
            if (Progress >= 0.9f)
            {
                Progress += 0.0033f;//0.1 进度,跑30帧,大概是1s左右
                _transitionFrame = 0;
            }
            else
            {//当场景正在进行异步加载的时候,使用Update获取进度条信息
                Progress = _asyncOperation.progress;
            }
        }
    }

    private void Reset()
    {
        Progress = 0;
        _cacheSceneName = null;
        _asyncOperation = null;
        _aoCompleted = null;
        SceneLoadingType = SceneLoadingType.None;
    }

    private static void MonitoringSceneLoad(Scene scene, LoadSceneMode mode)
    {
        SceneManager sceneManager = GameManager.Instance.QueryManager<SceneManager>();
        sceneManager._activeSceneName = scene.name;
    }
}
