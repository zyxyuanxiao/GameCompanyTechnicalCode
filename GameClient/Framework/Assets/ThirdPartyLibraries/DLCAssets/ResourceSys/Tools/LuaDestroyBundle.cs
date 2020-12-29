/********************************************************************
	created:	2016/06/28  21:03
	file base:	LuaDestroyBundle
	file ext:	cs
	author:		zjw
	
	purpose:	卸掉相关的资源
*********************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Best.ResourceSys;
using LuaInterface;
using UnityEngine.Profiling;

namespace Best
{
    public class LuaDestroyBundle : MonoBehaviour
    {
        public uint resUID = 0;

        protected void OnDestroy()
        {
#if UNITY_PROFILER
            Profiler.BeginSample("destroy lua bundle, name=" + gameObject.name);
#endif
            if(Best.ResourceSys.ResourceManager.Instance()!=null)
                ResourceManager.Instance().Unload(ref resUID);
#if UNITY_PROFILER
            Profiler.EndSample();
#endif
        }

    }
}