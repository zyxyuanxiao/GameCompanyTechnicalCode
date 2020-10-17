/********************************************************************
 Date: 2020-09-23
 Name: Sample
 author:  zhuzizheng
*********************************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LogSystem
{
    //样本
    public class Sample
    {

        public float  time;
        public string sceneName;
        public float  memory;
        public float  fps;
        public string fpsText;
        public float  graphMemUsage;


        public static float MemSize()
        {
            float s = sizeof(float) + sizeof(byte) + sizeof(float) + sizeof(float);
            return s;
        }


        public static Sample SaveASample(float fps = 0, string fpsText = "")
        {
            LogCacheData CacheData = LogManager.GetLogCacheData();
            Sample       sample    = new Sample();
            sample.fps           = fps;
            sample.fpsText       = fpsText;
            sample.sceneName     = LogManager.GetLogCacheData().currentScene;
            sample.time          = Time.realtimeSinceStartup;
            sample.memory        = CacheData.GCTotalMemory;
            sample.graphMemUsage = (CacheData.samples.Count * Sample.MemSize()) / 1024 / 1024;
            CacheData.samples.Add(sample);
            return sample;
        }
    }
}