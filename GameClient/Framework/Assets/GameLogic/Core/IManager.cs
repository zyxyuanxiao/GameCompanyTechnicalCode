public interface IManager
{
    void Awake();

    void Start();

    void OnDestroy();
}

public interface IUpdate : IManager
{
    void Update();
}