using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Best
{
    public class AtlasSnapShoot : AssetSnapShoot
    {
        public enum DEBUGTYPE
        {
            Both = 0,
            BDestroy,
            Ref
        }

        public static bool OpenAtlasSnapShoot = false;

        AssetCache<GameObject> assetCache;

        Dictionary<string, Stack<SnapShootInfo>> m_TagDic;

        static AtlasSnapShoot _Instance;
        public static AtlasSnapShoot Instance
        {
            get
            {
                if (_Instance == null)
                    return _Instance = new AtlasSnapShoot();
                return _Instance;
            }
        }

        public AtlasSnapShoot()
        {
            assetCache = GlobalAssetCacheMgr.Instance.CreateGameObjectCache("LuaAtlas", false);
            m_TagDic = new Dictionary<string, Stack<SnapShootInfo>>();
        }

        public override List<Element> Comparison(SnapShootInfo last, SnapShootInfo curr)
        {
            List<Element> list = new List<Element>();
            foreach(var item in curr.Dic)
            {
                Element e;
                if (last != null && last.Dic.TryGetValue(item.Key, out e))
                {
                    Element ee = new Element();
                    ee.Name = item.Key;
                    ee.RefCount = item.Value.RefCount - e.RefCount;
                    ee.bDestroy = !item.Value.bDestroy && e.bDestroy;
                    if (ee.RefCount != 0 || ee.bDestroy)
                        list.Add(ee);
                }
                else
                {
                    Element ee = new Element();
                    ee.bDestroy = item.Value.bDestroy;
                    ee.Name = item.Key;
                    ee.RefCount = item.Value.RefCount;
                    list.Add(ee);
                }
            }
            return list;
        }

        string ToString(List<Element> list)
        {
            string s = "资源名字\t\t\t\t计数变化\t\t资源是否销毁(图集资源不能在业务层被销毁，true需要检查哪里被销毁)";
            foreach (Element e in list)
            {
                s = string.Format("{0}\n{1}\t\t\t\t{2}\t\t{3}", s, e.Name, e.RefCount, e.bDestroy ? "<color=#ff0000>" + e.bDestroy + "</color>" : e.bDestroy.ToString());
            }
            return s;
        }

        string RefToString(List<Element> list)
        {
            string s = "资源名字\t\t\t\t计数变化";
            foreach (Element e in list)
            {
                if (e.RefCount != 0)
                {
                    s = string.Format("{0}\n{1}\t\t\t\t{2}", s, e.Name, e.RefCount);
                }
            }
            return s;
        }

        string DestroyToString(List<Element> list)
        {
            string s = "资源名字\t\t资源是否销毁(图集资源不能在业务层被销毁，true需要检查哪里被销毁)";
            foreach (Element e in list)
            {
                if (e.bDestroy)
                {
                    s = string.Format("{0}\n{1}\t\t<color=#ff0000>true</color>", s, e.Name);
                }
            }
            return s;
        }

        public override void SnapShoot(string tag)
        {
            Dictionary<string, Element> dic = new Dictionary<string, Element>();
            foreach(var item in assetCache.GetClientRefBucket())
            {
                Element e = new Element();
                e.Name = item.Key;
                e.RefCount = item.Value.RefCount;
                e.bDestroy = item.Value.Asset == null || item.Value.Asset.Equals(null);
                dic.Add(e.Name, e);
            }
            Stack<SnapShootInfo> list;
            if (!m_TagDic.TryGetValue(tag, out list))
            {
                list = new Stack<SnapShootInfo>();
                m_TagDic.Add(tag, list);
            }
            SnapShootInfo snapShoot = new SnapShootInfo(tag, dic);
            list.Push(snapShoot);
        }

        public override string ComparisonTag(string tag)
        {
            return ComparisonTag(tag, DEBUGTYPE.Both);
        }

        public string ComparisonTag(string tag, DEBUGTYPE type)
        {
            Stack<SnapShootInfo> list;
            if (m_TagDic.TryGetValue(tag, out list))
            {
                SnapShootInfo curr = list.Pop();
                SnapShootInfo last = null;
                if (list.Count > 0)
                {
                    last = list.Pop();
                }
                List<Element> results = Comparison(last, curr);
                if (results == null || results.Count == 0) return null;
                switch (type)
                {
                    case DEBUGTYPE.Both:
                        return string.Format("{0}->{1}\n{2}", last == null ? "" : last.Tag, curr.Tag, ToString(results));
                    case DEBUGTYPE.Ref:
                        return string.Format("{0}->{1}\n{2}", last == null ? "" : last.Tag, curr.Tag, RefToString(results));
                    case DEBUGTYPE.BDestroy:
                        return string.Format("{0}->{1}\n{2}", last == null ? "" : last.Tag, curr.Tag, DestroyToString(results));
                }
            }
            return null;
        }

        public override void Start(string tag)
        {
            SnapShoot(tag);
        }

        public override void End(string tag)
        {
            End(tag, 0);
        }

        public void End(string tag, int type)
        {
            SnapShoot(tag);
            string s = ComparisonTag(tag, (DEBUGTYPE)type);
            if (string.IsNullOrEmpty(s))
            {
                Debug.Log("正在使用的图集引用无变化");
            }
            else
            {
                Debug.Log(string.Format("{0}-图集引用变化:{1}", tag, s));
            }
        }
    }
}
