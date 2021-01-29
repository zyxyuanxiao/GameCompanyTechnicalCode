//身份证,唯一证明你存在这套系统中的证明,希望每个类都有.
//这个地方可以做成一个树.
//如果不挂载这个类,就属于法外狂徒,黑户,黑户容易被干
public interface IIdentityCard
{
    long InstanceId { get; set; }
}

public interface IManager : IIdentityCard
{
    void Awake();

    void Start();

    void OnDestroy();
}

public interface IUpdate : IIdentityCard
{
    void Update();
}

public interface ILateUpdate : IIdentityCard
{
    void LateUpdate();
}

public interface IFixedUpdate : IIdentityCard
{
    void FixedUpdate();
}


