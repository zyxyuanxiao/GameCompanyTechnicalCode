using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Common
{
    public static class BuildTools
    {
        public static string GetPlatform(BuildTarget target)
        {
            if (target == BuildTarget.Android)
            {
                return "Android";
            }
            else if (target == BuildTarget.iOS)
            {
                return "iOS";
            }
            else if (target == BuildTarget.WebGL)
            {
                return "WebGL";
            }
            else if (target == BuildTarget.StandaloneWindows || target == BuildTarget.StandaloneWindows64)
            {
                return "Windows";
            }
            else if (target == BuildTarget.StandaloneOSX || target == BuildTarget.StandaloneOSXIntel64)
            {
                return "OSX";
            }

            Debug.LogError("没有这个平台的设置");
            return "UnKnow";
        }
    }
}