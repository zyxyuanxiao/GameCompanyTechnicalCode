/********************************************************************
 Date: 2020-09-23
 Name: LogEntry
 author:  zhuzizheng
*********************************************************************/

using System.Text;
using UnityEngine;

namespace LogSystem
{
    /// <summary>
    /// Log实体,记录每条 Log 信息日志
    /// </summary>
    public class LogEntity
    {
        public int     count = 1;
        public LogType logType;
        public string  condition;
        public string  stacktrace;
        public int     sampleId;
        //public string   objectName="" ;//object who send error
        //public string   rootName =""; //root of object send error

        public LogEntity CreateCopy()
        {
            return (LogEntity)this.MemberwiseClone(); //创建当前的浅拷贝
        }
        public float GetMemoryUsage()
        {
            return (float)(sizeof(int)                      +
                           sizeof(LogType)                  +
                           condition.Length  * sizeof(char) +
                           stacktrace.Length * sizeof(char) +
                           sizeof(int));
        }

        /// <summary>
        /// 获取当前场景内的一级游戏对象名字
        /// </summary>
        /// <returns></returns>
        public string GetAllSceneGameObjectName()
        {
            StringBuilder allName = new StringBuilder();
            allName.Append("当前场景内所有游戏物体的名字:\n");
            Transform[] ts = UnityEngine.Object.FindObjectsOfType<Transform>();
            foreach (var VARIABLE in ts)
            {
                allName.Append("--" + VARIABLE.name + "--");
            }
            return allName.ToString();
        }
    }
}