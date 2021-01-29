using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

//没有持续执行的命令
public class CommandManager : IManager, IFixedUpdate,ICommandManager
{
    //单个命令
    private Queue<ICommand> _commands = new Queue<ICommand>();
    public long InstanceId { get; set; }

    public void Awake()
    {
        
    }

    public void Start()
    {

    }

    public void OnDestroy()
    {
        _commands.Clear();
        _commands = null;
        InstanceId = 0;
    }
    
    /// <summary>
    /// 添加并且发送命令,发送下级的层级
    /// </summary>
    public void AddCommand(ICommand command)
    {
        _commands.Enqueue(command);
    }


    public void RemoveCommandReceiver(Object obj)
    {
        
    }
    
    public void RemoveCommandReceiver(long id)
    {
        
    }
    
    
    /// <summary>
    /// 命令发出,使用物理发出,是正常命令发出模式
    /// </summary>
    public void FixedUpdate()
    {
        if (_commands.Count <= 0) return;//没有命令,无需发送
        ICommand command = _commands.Dequeue();
        
        //开始执行命令
        if (1 == command.Layer)
        {
            
            foreach (ICommandFirstReceiver cr in GameManager.QueryCommandReceiver())
            {
                cr.ReceiverCommand((ICommandFirstLevel)command);
            }
        }
        else if (2 == command.Layer)
        {
            
        }
        else if (3 == command.Layer)
        {
            
        }
        else if (4 == command.Layer)
        {
            
        }
        else if (5 == command.Layer)
        {
            
        }
        
    }
}