using Best.ResourceSys;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Best_Pool
{
    /** <summary>
     * InstancePool用于资源实例对象, ResUID如果交由实例池保存，实例池会负责unload和destroy资源asset，否则自己管理unload
     **/
    public sealed class InstancePool<T> : PoolParent<T> where T : UnityObject
    {
        T _asset;
        public uint ResUID = 0;     
        Transform m_parent;

        public InstancePool(string tag, T asset, Transform parent = null, LimitSetter limitSetter = null, CullSetter cullSetter = null, PreSetter preSetter = null) 
            : base(tag, limitSetter, cullSetter, preSetter)
        {
            if (asset == null)
            {
                if (bLogMessage)
                    Debug.LogErrorFormat("InstancePool {0} : Please create pool with non-empty asset!", poolName);
            }
            _asset = asset;
            if (typeName == "GameObject")
            {
                m_parent = new GameObject(poolName).transform;
                m_parent.position = Vector3.zero;
                if (parent != null) m_parent.parent = parent;
            }
        }

        public override T Spawn()
        {
            T ins = base.Spawn();
            if (ins == null)
            {
                Debug.LogErrorFormat("InstancePool {0} : The instance is null!", poolName);
            }
            if (typeName == "GameObject")
            {
                GameObject o = ins as GameObject;
                if (o == null)
                {
                    Debug.LogErrorFormat("InstancePool {0} : ins can not convert to GameObject! Need check it!", poolName);
                }
                o.SetActive(true);
                if (m_parent == null)
                    Debug.LogErrorFormat("InstancePool {0} : parent = null, check it!", poolName);
                o.transform.parent = m_parent;
                o.transform.localPosition = Vector3.zero;
                o.transform.localScale = Vector3.one;
            }
            return ins;
        }

        public override bool Despawn(T ins)
        {
            if (ins == null)
            {
                Debug.LogErrorFormat("InstancePool {0}-{1} can not despawn the null instance!", poolName, typeName);
                return false;
            }
            if (!base.Despawn(ins)) return false;

            if (bLogMessage)
                Debug.Log(string.Format("InstancePool {0}: Despawned the instance type of {1}. _spawned count = {2}, _despawned count = {3}.",
                                        poolName, typeName, _spawned.Count, _despawned.Count));

            if (typeName == "GameObject")
            {
                GameObject o = ins as GameObject;
                o.SetActive(false);
                o.transform.parent = m_parent;
                o.transform.localScale = Vector3.one;
                o.transform.localPosition = Vector3.zero;
            }
            return true;
        }

        public override T SpawnNew()
        {
            if (bLimit && totalCount > limitAmount)
            {
                Debug.LogWarningFormat(
                            "InstancePool {0} ({1}): LIMIT REACHED! Not creating new instances! (Returning null)",
                                poolName, typeName);
                return null;
            }

            T ins = CreateInstance();
            nameInstance(ins);
            _spawned.AddFirst(ins);
            if (typeName == "GameObject")
            {
                GameObject o = ins as GameObject;
                o.transform.parent = m_parent;
                o.transform.localPosition = Vector3.zero;
            }

            if (bLogMessage)
                Debug.Log(string.Format("InstancePool {0}: Spawned new instance type of {1}. _spawned count = {2}, _despawned count = {3}.",
                                        poolName, typeName, _spawned.Count, _despawned.Count));
            return ins;
        }

        public override T CreateInstance()
        {
            if (IsDebug)
                poolDebug.CreateTimes++;
            if (_asset == null)
                Debug.LogErrorFormat("InstancePool {0} : The _asset is null! Please Check it!", poolName);
            return UnityObject.Instantiate<T>(_asset);
        }

        void nameInstance(T instance)
        {
            instance.name += (this.totalCount + 1).ToString("#000");
        }

        public override void DestroyInstance(T ins)
        {
            UnityObject.Destroy(ins);
            if (bLogMessage)
            {
                poolDebug.DestroyTimes++;
                Debug.Log(string.Format("InstancePool {0}: destory the instance type of {1}." +
                                        "_spawned count = {2}, _despawned count = {3}.",
                                    poolName, typeName, _spawned.Count, _despawned.Count));
            }
        }

        public override void SendMessage(bool isSpawn, T ins = null)
        {
            if (ins == null) return;
            if (typeName != "GameObejct") return;

            GameObject o = null;
            o = ins as GameObject;
            if (isSpawn)
                o.BroadcastMessage("OnDespawned", SendMessageOptions.DontRequireReceiver);
            else
                o.BroadcastMessage("OnSpawned", SendMessageOptions.DontRequireReceiver);
        }

        public override void ClearAll()
        {
            base.ClearAll();
            if (_asset != null)
            {
                if (ResUID != 0) ResourceManager.Instance().Unload(ref ResUID);
                _asset = null;
            }
            if (m_parent != null) UnityObject.Destroy(m_parent.gameObject);
            m_parent = null;
        }
    }
}

