using UnityEngine;

namespace GameAssets
{

    /// <summary>
    /// 引用分三层
    /// 第一层是从磁盘上面加载到的 AB 压缩包
    /// 第二层是从 AB 包解压出来的一层对象,这层对象不能直接使用
    /// 第三层是从解压出来的对象,实例化另外一个实用对象,这层对象可以在其他任何地方中使用
    /// </summary>
    public interface Reference
    {
        //AssetBundle的名字
        string abName { get; set; }

        //AssetBundle的对象
        AssetBundle AssetBundle { get; set; }
        
        //加压缩 AB 的引用对象,只能是缓存
        UnityEngine.Object UncompressObject { get; set; }
        
        /// <summary>
        /// 可以在其他任何地方使用,也可以放进资源池子;
        /// 但是当任何其他资源被卸载时,必须要知道本物体中是否有资源也被卸载了;
        /// 这个对象里面的引用的资源,是否已被卸载,是无法立即知道的;
        /// </summary>
        UnityEngine.Object obj { get; set; }

        //增加引用计数
        void Retain();

        //减少引用计数
        void Release();

        //重置
        void Reset();
    }
}