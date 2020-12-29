
/**
 * <summary>
 * 具有缓存淘汰功能的cache都应该实现此接口
 * </summary>
 */
public interface IEliminateCache<TKey, TValue>
{
    event System.Action<TKey, TValue> OnEliminate;

 	void RegiestEvent(System.Action<TKey, TValue> OnEliminated);

    bool TryGet(TKey key, out TValue value);

    /// <summary>
    /// 仅仅只为查询
    /// </summary>
    bool TryGetNoUpdate(TKey key, out TValue value);

    TValue Add(TKey key, TValue value, bool updateExists = true);

    void Remove(TKey key);

    void Release();

    void DebugLog(string s);

    bool EnableDebug { get; set; }

    void LogAllAsset(string head = null);

    void ReleaseUnuseAsset();
}

/**
 * <summary>
 * 针对战斗状态资源缓存处理
 * </summary>
 */
public interface IEliminateCacheComboState<TKey, TValue>
{  
    bool ComboStateTryGet(TKey key, out TValue value);

    TValue ComboStateAdd(TKey key, TValue value, bool updateExists = true);
}