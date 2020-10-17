# 整体架构
* 1. 采用Manager of Managers 以及 ECS 配合使用 
* 2. GameManager 作为 Root节点,可以根据此节点,寻找到游戏内的任意一个管理类
* 3. 
# 框架介绍
* 1:  高性能架构,采用ECS来完成
* 2:  业务采用toLua进行完成
* 3:  网络KCP,TCP,protobuf,借鉴ET
* 4:  节点编程
* 5:  UI使用 New UI Widgets
* 6:  Log系统
* 7:  热更系统
* 9:  AB加载系统
* 10: 配置表系统