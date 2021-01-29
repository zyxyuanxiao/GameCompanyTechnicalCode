# 整体架构
* 1. 采用Manager of Managers 以及 ECS 配合使用 
* 2. GameManager 作为 Root节点,可以根据此节点,寻找到游戏内的任意一个管理类
* 3. 打包规则是以BuildAssets文件夹为主,Editor Default Resources放的是BuildAssets文件夹中文件的依赖文件.具体细节请看代码
# 框架介绍
* 1:  高性能架构代码,采用ECS来完成
* 2:  业务采用toLua进行完成
* 3:  网络KCP,TCP,Protobuf 作为协议包传输
* 4:  节点编程
* 5:  UI使用 New UI Widgets
* 6:  Log系统
* 7:  热更系统
* 9:  AB加载系统
* 10: 配置表系统,打表配置文件放在 ToLua 核心代码里面


# 整个系统功能职责

* 1: GameManager --> BaseManager 皇上 --> 太上皇,权力最大 ,可以通过皇上,找到游戏内任何一个东西
* 2: 下面一堆的   Manager  属于中央官员处理城镇级别的事情
* 3: 第三方库,大型的,稳定的,核心的,模块属于城镇级别的东西
* 4: 村 属于开发人员编写的代码(小型功能) 
* 5: 人员交互(随时被推翻的东西,测验的东西)
* 6: 底层交互使用 命令+事件 (设计模式)制度,将整个帝国串联起来.

# 核心插件
* 1:tolua
* 2:playermaker
* 3:Behavior Designer全套
* 4:A Pathfinding Project Pro
* 5:DOTween Pro
* 6:Animancer Pro
