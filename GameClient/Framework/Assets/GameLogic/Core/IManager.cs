public interface IManager
{
    void Awake();

    void Start();

    void OnDestroy();
}

public interface IUpdate
{
    void Update();
}

public interface ILateUpdate
{
    void LateUpdate();
}

public interface IFixedUpdate
{
    void FixedUpdate();
}