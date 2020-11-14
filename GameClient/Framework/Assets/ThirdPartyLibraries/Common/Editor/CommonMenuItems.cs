using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Common;

namespace Common
{
    public class CommonMenuItems
    {
        [MenuItem("Tools/Security/CRC", priority = 20001)]
        private static void GetCRC()
        {
            var path = EditorUtility.OpenFilePanel("OpenFile", Environment.CurrentDirectory, "");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            using (var fs = File.OpenRead(path))
            {
                var crc = SecurityTools.GetCRC32Hash(fs);
                Debug.Log(crc);
            }
        }

        [MenuItem("Tools/Security/MD5", priority = 20002)]
        private static void GetMD5()
        {
            var path = EditorUtility.OpenFilePanel("OpenFile", Environment.CurrentDirectory, "");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            using (var fs = File.OpenRead(path))
            {
                var crc = SecurityTools.GetMD5Hash(fs);
                Debug.Log(crc);
            }
        }

        [MenuItem("Tools/截屏", priority = 40003)]
        private static void Screenshot()
        {
            var path = EditorUtility.SaveFilePanel("截屏", null, "screenshot_", "png");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            ScreenCapture.CaptureScreenshot(path);
        }
    }
}