using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

namespace Common
{
    public class AppBuild : Editor
    {

        //在这里找出你当前工程所有的场景文件，假设你只想把部分的scene文件打包 那么这里可以写你的条件判断 总之返回一个字符串数组。
        static string[] GetBuildScenes()
        {
            List<string> names = new List<string>();

            foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
            {
                if (e == null)
                    continue;
                if (e.enabled)
                    names.Add(e.path);
            }

            return names.ToArray();
        }

        //得到项目的名称
        public static string projectName
        {
            get
            {
                //在这里分析shell传入的参数， 还记得上面我们说的哪个 project-$1 这个参数吗？
                //这里遍历所有参数，找到 project开头的参数， 然后把-符号 后面的字符串返回，
                //这个字符串就是 appStore 了。。
                foreach (string arg in System.Environment.GetCommandLineArgs())
                {
                    if (arg.StartsWith("project"))
                    {
                        return arg.Split("-"[0])[1];
                    }
                }

                return "Default";
            }
        }

        public static string GetParmByKey(string key)
        {
            foreach (string arg in System.Environment.GetCommandLineArgs())
            {
                if (arg.StartsWith(key))
                {
                    return arg.Split("-"[0])[1];
                }
            }

            if (key.Equals("bundleVersion"))
            {
                return Application.version;
            }

            if (key.Equals("bundleID"))
            {
                return "com.default.game.cn";
            }

            if (key.Equals("productName"))
            {
                return "游戏";
            }

            if (key.Equals("releaseType"))
            {
                return "Debug"; //内网包
                // return "FinalRelease";//外网包
            }

            return "";
        }

        public static BuildOptions GetBuildOptions(string option)
        {
            BuildOptions op = BuildOptions.None;
            string[] ops = option.Split('|');
            foreach (var opt in ops)
            {
                if (!string.IsNullOrEmpty(opt) && !opt.Equals(" "))
                {
                    op |= (BuildOptions) Enum.Parse(typeof(BuildOptions), opt);
                    Debug.Log("===>BuildOptions:" + op);
                }
            }

            return op;
        }

        
        [MenuItem("Builder/Build Android.apk", priority = 2000)]
        static void BuildForAndroid()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);

            PlayerSettings.Android.keystorePass = "default123";
            PlayerSettings.Android.keyaliasPass = "default123";
            string path = Application.dataPath;
            path = path.Replace("Assets", "") + projectName + ".apk";
            DeleteExit(path);
            BuildOptions bo = GetBuildOptions(GetParmByKey("buildOptions"));
            if (bo == BuildOptions.AcceptExternalModificationsToPlayer)
            {
                EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
                EditorUserBuildSettings.exportAsGoogleAndroidProject = true;
            }

            BuildPipeline.BuildPlayer(GetBuildScenes(), path, BuildTarget.Android, bo);
            //恢复成原来的状态
            EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
        }

        /// <summary>
        /// 打包android
        /// </summary>
        [MenuItem("Builder/Build Android_DEV.apk", priority = 2001)]
        static void BuildForAndroid_DEV()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);

            PlayerSettings.Android.keystorePass = "xlcw123";
            PlayerSettings.Android.keyaliasPass = "xlcw123";

            string path = Application.dataPath;
            path = path.Replace("Assets", "") + projectName + ".apk";
            DeleteExit(path);
            EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
            BuildPipeline.BuildPlayer(GetBuildScenes(), path, BuildTarget.Android, BuildOptions.Development);
        }

        /// <summary>
        /// 打包android
        /// </summary>
        [MenuItem("Builder/Build Android.project", priority = 2002)]
        static void BuildForAndroidProject()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            PlayerSettings.Android.keystorePass = "xlcw123";
            PlayerSettings.Android.keyaliasPass = "xlcw123";
            // string path = Application.dataPath;
            // path = path.Replace("Assets", "") + projectName + ".apk";
            // DeleteExit(path);
            EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
            EditorUserBuildSettings.exportAsGoogleAndroidProject = true;
            BuildPlatformPlayer(BuildTarget.Android, BuildOptions.AcceptExternalModificationsToPlayer);
            // BuildPipeline.BuildPlayer(GetBuildScenes(), path, BuildTarget.Android,
            //     BuildOptions.AcceptExternalModificationsToPlayer);
            EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
        }

        /// <summary>
        /// 打包ipa
        /// </summary>
        [MenuItem("Builder/Build Phone.xcode", priority = 2002)]
        static void BuildForIPhone()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
            string build = GetParmByKey("buildOptions");
            BuildPipeline.BuildPlayer(GetBuildScenes(), projectName, BuildTarget.iOS, GetBuildOptions(build));
        }

        /// <summary>
        /// 打包ipa_dev
        /// </summary>
        [MenuItem("Builder/Build Phone_DEV.xcode", priority = 2003)]
        static void BuildForIPhone_DEV()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
            string build = GetParmByKey("buildOptions");
            BuildPipeline.BuildPlayer(GetBuildScenes(), projectName, BuildTarget.iOS, BuildOptions.Development);
        }

        /// <summary>
        /// 打包win
        /// </summary>
        [MenuItem("Builder/Build Windows_DEV.exe", priority = 2004)]
        public static void BuildForWindows_DEV()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone,
                BuildTarget.StandaloneWindows64);
            BuildPlatformPlayer(BuildTarget.StandaloneWindows64, BuildOptions.Development);

            // string path = Application.dataPath;
            // path = path.Replace("Assets", "UnityWindows/main.exe");
            // BuildPipeline.BuildPlayer(GetBuildScenes(), path, BuildTarget.StandaloneWindows64,
            //     BuildOptions.Development);
        }

        /// <summary>
        /// 打包MacOS
        /// </summary>
        [MenuItem("Builder/Build MacOS_DEV.app", priority = 2005)]
        public static void BuildForMacOS_DEV()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX);
            BuildPlatformPlayer(BuildTarget.StandaloneOSX, BuildOptions.Development);

            
            // string path = Application.dataPath;
            // path = path.Replace("Assets", "UnityMacOS/main.app");
            // BuildPipeline.BuildPlayer(GetBuildScenes(), path, BuildTarget.StandaloneOSX, BuildOptions.Development);
        }


        /// <summary>
        /// 打包基础函数
        /// </summary>
        public static void BuildPlatformPlayer(BuildTarget target,BuildOptions options)
        {
            string path = "Assets/_BuildAsset/Config/ABManifest.asset";
            if (!File.Exists(path))
            {
                Debug.LogError("ABManifest文件不存在");
                return;
            }
            BuildPlayerOptions bpo = new BuildPlayerOptions()
            {
                scenes = GetBuildScenes(),
                locationPathName = GetBuildTargetPath(target),
                target = EditorUserBuildSettings.activeBuildTarget,
                options = options,
                assetBundleManifestPath = path,
            };
            BuildPipeline.BuildPlayer(bpo);
        }

        private static string GetBuildTargetPath(BuildTarget target)
        {
            string time = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            string name = PlayerSettings.productName + "_" + PlayerSettings.bundleVersion;
            string prefix = Application.dataPath.Replace("Assets", "");
            switch (target)
            {
                case BuildTarget.Android:
                    return prefix + string.Format("{0}_{1}_{2}.apk", name, SVNHelper.GetSvnVersion(), time);
                case BuildTarget.StandaloneWindows64:
                    return prefix + string.Format("{0}_{1}_{2}/UnityWindows.exe", name, SVNHelper.GetSvnVersion(),
                        time);
                case BuildTarget.StandaloneOSX:
                    return prefix + "UnityMacOS/" + PlayerSettings.productName + ".app";
                case BuildTarget.iOS:
                    return prefix + "UnityXcode/";
                default:
                    Debug.Log("Target not support.");
                    return null;
            }
        }

        static void DeleteExit(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }
    }
}