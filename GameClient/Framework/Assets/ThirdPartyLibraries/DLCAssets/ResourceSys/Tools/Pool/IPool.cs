using Best;
using System.Collections.Generic;
using UnityEngine;

namespace Best_Pool
{
    public interface IPool
    {
        void LateUpdate();

        void ClearAll();

        bool IsDebug { get; set; }

        void DebugLog();
    }

    public interface IPool<T> : IPool
    {
        T Spawn();
        bool Despawn(T ins);
        T SpawnNew();
        T CreateInstance();
        void DestroyInstance(T ins);
        void SendMessage(bool isSpawn, T ins);
    }

    public delegate void PreSetter(ref bool ispre, ref int prenum, ref int preperframe, ref float delay);
    public delegate void LimitSetter(ref bool blimit, ref int amount);
    public delegate void CullSetter(ref bool iscull, ref int cullamount, ref int cullperframe, ref float delay);
    public delegate void DeadSetter(ref bool bOpenDead, ref float deadTime, ref float deadIntervalTime, ref int deadRemain);

    public abstract class PoolParent<T> : IPool<T>
    {
        public bool bLogMessage = false;
        protected bool bSendMessage = true;

        /// <summary>
        /// bOpenDead           是否开启死亡
        /// bDead               是否死亡
        /// deadTime            对象池死亡时间
        /// deadIntervalTime    死亡间隔时间
        /// deadRemain          死亡后剩余数量
        /// </summary>
        protected bool bOpenDead = false;
        protected bool bDead = false;
        protected float deadTime = 0;
        protected float deadIntervalTime = 120;
        protected int deadRemain = 1;

        /// <summary>
        /// bPreCreate          是否预创建
        /// preFrames           每帧创建个数
        /// preDelay            开始创建延时
        /// preCreateAmount     预创建个数
        /// </summary>
        protected bool bPreCreate = false;
        protected int preFrames = 1;
        protected float preDelay = 0;
        protected int preCreateAmount = 1;

        /// <summary>
        /// bLimit              是否限制数量
        /// limitAmount         限制池的最大数量
        /// </summary>
        protected bool bLimit = false;
        protected int limitAmount = 30;

        /// <summary>
        /// isCull              是否超出阈值剔除
        /// cullAmount          销毁对象的阈值
        /// cullMaxPerPass      每个时间间隔销毁个数
        /// cullDelay           销毁时间间隔
        /// </summary>
        protected bool isCull = false;
        protected int cullThreshold = 10;
        protected int cullMaxPerPass = 5;
        protected float cullDelay = 5;

        int remainAmount = 0;
        float cullInterval = 0;

        public int totalCount
        {
            get
            {
                int count = 0;     
                count += _spawned.Count;
                count += _despawned.Count;
                return count;
            }
        }

        public string typeName;
        public string poolName;

        protected PooledLinkedList<T> _spawned;
        protected PooledLinkedList<T> _despawned;

        public int SpawnedCount
        {
            get { return _spawned.Count; }
        }

        public int DespawnedCount
        {
            get { return _despawned.Count; }
        }

        float preTime = 0;
        public PoolParent(string tag, LimitSetter limitSetter = null, CullSetter cullSetter = null, PreSetter preSetter = null)
        {
            _spawned = new PooledLinkedList<T>();
            _despawned = new PooledLinkedList<T>();
            typeName = typeof(T).Name;
            poolName = "Pool_" + tag;

            if (!BuildManifestUtility.GetBuildManifest().IsReleaseVer)
            {
                bLogMessage = false;
                if (pool_manager.IsDebugPool(tag))
                    IsDebug = true;
            }

            SetPreCreate(preSetter);
            SetLimit(limitSetter);
            SetCull(cullSetter);
            if (bOpenDead)
            {
                deadTime = Time.time + deadIntervalTime;
                SetDead(false);
            }
        }

        public void SetLimit(LimitSetter limitSetter)
        {
            if (limitSetter != null)
                limitSetter(ref bLimit, ref limitAmount);
        }

        public void SetPreCreate(PreSetter preSetter)
        {
            if (preSetter != null)
                preSetter(ref bPreCreate, ref preCreateAmount, ref preFrames, ref preDelay);
            if (bPreCreate)
            {
                preTime = Time.time + preDelay;
                isPreCreated = false;
            }
        }

        public void SetCull(CullSetter cullSetter)
        {
            if (cullSetter != null)
                cullSetter(ref isCull, ref cullThreshold, ref cullMaxPerPass, ref cullDelay);
        }

        public void SetDead(DeadSetter deadSetter)
        {
            if (deadSetter != null)
                deadSetter(ref bOpenDead, ref deadTime, ref deadIntervalTime, ref deadRemain);
        }

        public virtual bool Despawn(T ins)
        {
            LinkedListNode<T> node = _spawned.First;
            while (node != null)
            {
                if (node.Value.Equals(ins))
                {
                    _spawned.Remove(node);
                    break;
                }
                node = node.Next;
            }
                
            if (bLimit && totalCount >= limitAmount)
            {
                if (IsDebug)
                {
                    poolDebug.LimitTimes++;
                    poolDebug.DebugLog();
                }
                if (bLogMessage)
                    Debug.LogWarning(string.Format("Pool {0} is Full! Will be not despawn the instance typeof {1}.", poolName, typeName));
                DestroyInstance(ins);
                return false;
            }
            //if (bLogMessage)
            //    Debug.LogFormat("Pool {0} : despawn the instance typeof {1}", poolName, typeName);
            _despawned.AddFirst(ins);

            if (IsDebug)
            {
                poolDebug.DespawnTimes++;
            }

            if (bSendMessage)
                SendMessage(false, ins);
            return true;
        }

        void CullDespawn()
        {
            if (IsDebug)
                poolDebug.CullTimes++;
            for (int i = 0; i < cullMaxPerPass; i++)
            {
                if (totalCount <= remainAmount || _despawned.Count == 0)
                {
                    isCullActive = false;
                    break;
                }
                if (IsDebug)
                    poolDebug.CullCount++;
                T ins = _despawned.First.Value;
                _despawned.RemoveFirst();
                DestroyInstance(ins);
            }
        }

        public virtual T Spawn()
        {
            if (bLimit && _spawned.Count >= limitAmount)
            {
                if (bLogMessage)
                    Debug.LogWarning(string.Format("Pool {0} is Full! Create a instance who typeof {1} will be not add to spawned.", poolName, typeName));
                if (IsDebug)
                    poolDebug.DebugLog();
                return CreateInstance();
            }
            T ins;
            if (_despawned.Count == 0)
            {
                ins = SpawnNew();
            }
            else
            {              
                ins= _despawned.First.Value;
                _despawned.RemoveFirst();
                if (ins == null || ins.Equals(null))
                {
                    //目前是为了处理出现在有人物模型的界面(如背包)中被服务器强制下线回到登录界面，会存在其模型放入到缓存池后被销毁的情况
                    ins = SpawnNew();
                }           
                _spawned.AddFirst(ins);
            }

            if (bOpenDead)
            {
                deadTime = Time.time + deadIntervalTime;
                SetDead(false);
            }

            if (bSendMessage && ins != null)
                SendMessage(true, ins);

            return ins;
        }

        void PreCreateInstance()
        {
            if (bLogMessage)
                Debug.Log(string.Format("Pool {0}: Pre Spawned instance typeof {1}.",
                                        poolName,
                                        typeName));
            for (int i = 0; i < preFrames; i++)
            {
                if (totalCount >= preCreateAmount)
                {
                    break;
                }
                T ins = SpawnNew();
                _spawned.RemoveFirst();
                _despawned.AddFirst(ins);
            }
        }

        public abstract T SpawnNew();
        public abstract T CreateInstance();
        public abstract void DestroyInstance(T ins);
        public virtual void SendMessage(bool isSpawn, T ins) { }

        bool isPreCreated = true;
        bool isCullActive = false;
        float cullTime = 0;
        public virtual void LateUpdate()
        {
            if (!isPreCreated && Time.time > preTime)
            {
                if (totalCount >= preCreateAmount)
                    isPreCreated = true;
                else
                {
                    PreCreateInstance();
                }
            }
            if (bOpenDead && Time.time > deadTime && _spawned.Count == 0)
            {
                SetDead(true);
            }
            if (isCull && !isCullActive && totalCount > cullThreshold)
            {
                isCullActive = true;
                cullTime = Time.time + cullDelay;
            }
            if (isCullActive && cullTime < Time.time)
            {
                CullDespawn();
                cullTime = Time.time + cullInterval;
            }
        }

        void SetDead(bool isdead)
        {
            if (isdead == bDead) return;
            bDead = isdead;
            if (isdead)
            {
                if (IsDebug)
                {
                    poolDebug.DeadTimes++;
                }
                if (bLogMessage)
                    Debug.LogFormat("Pool {0} is dead, type is {1}.", poolName, typeName);
                remainAmount = deadRemain;
                isCullActive = true;
                isPreCreated = true;
                cullInterval = 0;
            }
            else
            {
                if (IsDebug)
                    poolDebug.ActivateTimes++;
                isCullActive = false;
                remainAmount = cullThreshold;
                cullInterval = cullDelay;
            }
            cullTime = Time.time + cullInterval;
        }

        public virtual void ClearAll()
        {
            T ins;
            LinkedListNode<T> firstDespawn = _despawned.First;
            while (firstDespawn != null)
            {
                ins = firstDespawn.Value;
                DestroyInstance(ins);
                firstDespawn = firstDespawn.Next;
            }
          
            _despawned.Clear();

            LinkedListNode<T> firstSpawn = _spawned.First;
            while (firstSpawn != null)
            {
                ins = firstSpawn.Value;
                DestroyInstance(ins);
                firstSpawn = firstSpawn.Next;
            }
           
            _spawned.Clear();
            if (IsDebug && bLogMessage)
                poolDebug.DebugLog();
            poolDebug = null;
        }

        protected bool m_isDebug = false;
        public virtual bool IsDebug
        {
            get { return m_isDebug; }
            set
            {
                if (m_isDebug == value) return;
                m_isDebug = value;
                if (m_isDebug)
                {
                    poolDebug = new PoolDebug<T>(this);
                }
                else
                    poolDebug = null;
            }
        }
        public PoolDebug<T> poolDebug;

        public virtual void DebugLog()
        {
            if (IsDebug)
                poolDebug.DebugLog();
        }
    }
}