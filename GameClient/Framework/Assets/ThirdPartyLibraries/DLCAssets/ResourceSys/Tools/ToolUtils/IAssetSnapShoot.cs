using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Best
{
    interface IAssetSnapShoot
    {
        /// <summary>
        /// 快照+缓存
        /// </summary>
        /// <param name="tag"></param>
        void SnapShoot(string tag);
        string ComparisonTag(string tag);
    }

    interface IComparisonSnapShoot
    {

    }

    public abstract class AssetSnapShoot : IAssetSnapShoot
    {
        public class SnapShootInfo
        {
            public string Tag;
            public Dictionary<string, Element> Dic;

            public SnapShootInfo(string tag, Dictionary<string, Element> dic)
            {
                Tag = tag;
                Dic = dic;
            }
        }

        public struct Element
        {
            public string Name;
            public int RefCount;
            public bool bDestroy;
        }

        public abstract void SnapShoot(string tag);
        public abstract List<Element> Comparison(SnapShootInfo last, SnapShootInfo curr);
        public abstract string ComparisonTag(string tag);
        public abstract void Start(string tag);
        public abstract void End(string tag);
    }
}
