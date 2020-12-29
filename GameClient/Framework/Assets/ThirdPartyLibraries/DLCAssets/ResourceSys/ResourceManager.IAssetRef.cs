using UnityEngine;
using SBC = LuaInterface.StringBuilderCache;
using AssetBundleName = System.String;
using EditorAssetURI = System.String; 
using RuntimeAssetName = System.String;
using UnityObject = UnityEngine.Object;

namespace Best
{
    namespace ResourceSys
    {
        public partial class ResourceManager : MonoBehaviour    
        {
            public interface IAssetRef
            {
                RuntimeAssetName AssetName { get; }
                AssetBundle Bundle { get; }
                UnityObject Asset { get; }

                void IncreaseRef(int count = 1);

                void DecreaseRef(string name = "",bool isRelease = true);

                void Reset();
            }
        }
    }
}