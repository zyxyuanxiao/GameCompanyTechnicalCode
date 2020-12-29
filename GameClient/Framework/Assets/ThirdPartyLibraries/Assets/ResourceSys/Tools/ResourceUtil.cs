using UnityEngine;
using System;

using UnityObject = UnityEngine.Object;

namespace Best
{
    namespace ResourceSys
    {
        public static class ResourceUtil
        {
            public static string UnifyPath(string path)
            {
                if (path.Contains("\\"))
                    path = path.Replace("\\", "/");

                return path;
            }

            //GCTODO:后续需要改为针对单个asset加载
            public static void LoadAsset(AssetBundle bundle, bool syncIO, Action<UnityObject[]> onLoaded)
            {
                if(bundle == null)
                {
                    onLoaded(null);
                    return;
                }

                if (syncIO)
                {
                    onLoaded(bundle.LoadAllAssets());
                    LogModule.Instance.Trace(LogModule.LogModuleCode.ResourceSys, string.Format("sync load asset finished: {0}", bundle.name));
                }
                else
                {
                    LogModule.Instance.Trace(LogModule.LogModuleCode.ResourceSys, string.Format("async load asset begin: {0}", bundle.name));
                    bundle.LoadAllAssetsAsync().completed += (req)=>
                    {
                        LogModule.Instance.Trace(LogModule.LogModuleCode.ResourceSys, string.Format("async load asset finished: {0}", bundle.name));
                        onLoaded((req as AssetBundleRequest).allAssets);
                    };
                }
            }

            public static void LoadAssetBundle(string abFullPath, bool syncIO, Action<AssetBundle> onLoaded)
            {
                if (abFullPath == null)
                {
                    onLoaded(null);
                    return;
                }

                if (syncIO)
                {
                    AssetBundle ab = AssetBundle.LoadFromFile(abFullPath);
                    LogModule.Instance.Trace(LogModule.LogModuleCode.ResourceSys, string.Format("sync load assetbundle finished: {0}", abFullPath));
                    onLoaded(ab);
                }
                else
                {
                    LogModule.Instance.Trace(LogModule.LogModuleCode.ResourceSys, string.Format("async load assetbundle begin: {0}", abFullPath));
                    AssetBundle.LoadFromFileAsync(abFullPath).completed += (req) =>
                    {
                        LogModule.Instance.Trace(LogModule.LogModuleCode.ResourceSys, string.Format("async load assetbundle finished: {0}", abFullPath));
                        onLoaded((req as AssetBundleCreateRequest).assetBundle);
                    };
                }
            }

        }
    }
}
