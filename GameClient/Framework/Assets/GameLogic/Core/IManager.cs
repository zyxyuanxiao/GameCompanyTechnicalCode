public interface IManager
{
    void Awake();

    void Start();

    void OnDestroy();
}

public interface IUpdateManager : IManager
{
    void Update();
}