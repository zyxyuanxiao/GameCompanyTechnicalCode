using UnityEngine;
using System.Collections.Generic;

namespace Best
{

    public interface ILateUpdate
    {
        void LateUpdate();
    }

    public interface IUpdate
    {
        void Update();
    }

    public class UpdatableRunner : MonoBehaviour
    {
        private static UpdatableRunner _inst = null;

        public static void Init()
        {
            var p = UpdatableRunner.Instance;
        }

        public static UpdatableRunner Instance
        {
            get
            {
                if (_inst == null)
                {
                    GameObject runner = new GameObject();
                    runner.name = "UpdatableRunner";
                    if(Application.isPlaying)
                        DontDestroyOnLoad(runner);
                    _inst = runner.AddComponent<UpdatableRunner>();
                }
                return _inst;
            }
        }

        private void OnDestroy()
        {
            _inst = null;
        }

        public void AddLateUpdate(ILateUpdate entry)
        {
            m_lateUpdatables.Add(entry);
        }

        public void RemoveLateUpdate(ILateUpdate entry)
        {
            m_lateUpdatables.Remove(entry);
        }

        public void AddUpdate(IUpdate entry)
        {
            m_updatables.Add(entry);
        }

        public void RemoveUpdate(IUpdate entry)
        {
            m_updatables.Remove(entry);
        }

        #region private
        private void LateUpdate()
        {
            foreach (ILateUpdate entry in m_lateUpdatables)
                entry.LateUpdate();
        }

        private void Update()
        {
            foreach (IUpdate entry in m_updatables)
                entry.Update();
        }

        private HashSet<ILateUpdate> m_lateUpdatables = new HashSet<ILateUpdate>();
        private HashSet<IUpdate> m_updatables = new HashSet<IUpdate>();
        #endregion
    }
}
