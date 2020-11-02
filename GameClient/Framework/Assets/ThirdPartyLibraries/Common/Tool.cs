namespace Common
{
    public static class Tool
    {
        public static string GetPlatform()
        {
#if UNITY_STANDALONE_OSX
            return "OSX";
#elif UNITY_ANDROID
    return "Android";
#elif UNITY_STANDALONE
    return "Windows";
#elif UNITY_IPHONE
    return "iOS";
#else
    Debug.LogError("没有这个平台的设置");
    return "UnKnow";        
#endif
        }
    }
}