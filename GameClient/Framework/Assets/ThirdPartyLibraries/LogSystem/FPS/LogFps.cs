using UnityEngine;

namespace LogSystem
{
    public class LogFps : ILoggerInterface
    {
        // For FPS Counter
        private       int   frames         = 0;
        private       bool  firstTime      = true;
        private       float lastUpdate     = 0f;
        private const int   requiredFrames = 10;
        private const float updateInterval = 0.25f;

        public string fpsText;
        public float  fps; //每秒帧速率


        public void Initialize()
        {

        }



        /// <summary>
        /// 主线程
        /// </summary>
        public void Update()
        {
            fpsText = fps.ToString("0.000");
            
            
            
            // FPS Counter
            if (firstTime)
            {
                firstTime  = false;
                lastUpdate = Time.realtimeSinceStartup;
                frames     = 0;
                return;
            }
            frames++;
            float dt = Time.realtimeSinceStartup - lastUpdate;
            if (dt > updateInterval && frames > requiredFrames)
            {
                fps        = (float) frames / dt;
                frames     = 0;
                lastUpdate = Time.realtimeSinceStartup;
            }
        }

        public void UnInitialize()
        {

        }
    }
}